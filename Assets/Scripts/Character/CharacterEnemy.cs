using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using Mirror;

public class CharacterEnemy : BaseCharacter
{
	public enum Status
	{
		idle = 0,
		walking = 1,
		tracking = 2,
		attacking = 3,
		dying = 4,
		dead = 5,
	}

	private string[] m_astrStatusString = new string[System.Enum.GetNames(typeof(Status)).Length];

	[Header("Enemy properties")]
	public float m_fAggroRadius = 10.0f;
	public bool m_bShowAggroRadius = false;
	public bool m_bShowAttackRadius = true;
	public bool m_bShowMoveDirection = true;
	public int m_iMaxIdleToWalkThreshold = 8;
	public int m_iMaxWalkToIdleThreshold = 4;

	[SyncVar]
	private Status m_eStatus;
	[SyncVar]
	private Status m_eStatusToProcess;

	private BaseCharacter m_characterAggroed;

	private Animator m_animator;

	private Rigidbody m_body;
	
	private Vector3 m_vDirection;
	private float m_fMaxIdleToWalkThresholdRandom;
	private float m_fDurationIdleToWalk;
	private float m_fDurationWalkToIdle;
	private NavMeshAgent m_agent;

	private bool m_bHasAttacked = false;

	private new void Start()
	{
		base.Start();

		m_astrStatusString[(int)(Status.idle)] = "Not Handled";
		m_astrStatusString[(int)(Status.walking)] = "Walk";
		m_astrStatusString[(int)(Status.tracking)] = "Track";
		m_astrStatusString[(int)(Status.attacking)] = "Attack";
		m_astrStatusString[(int)(Status.dying)] = "Death";
		m_astrStatusString[(int)(Status.dead)] = "Not Handled";

		m_eStatus = Status.idle;
		m_eStatusToProcess = Status.idle;
		m_characterAggroed = null;

		m_body = GetComponent<Rigidbody>();
		Assert.IsNotNull(m_body);
		m_vDirection = Vector3.forward;

		m_agent = GetComponent<NavMeshAgent>();
		Assert.IsNotNull(m_agent);
		m_agent.updateRotation = true;
		m_agent.acceleration = 60.0f;
		m_fMaxIdleToWalkThresholdRandom = Random.Range(m_iMaxIdleToWalkThreshold * 0.5f, m_iMaxIdleToWalkThreshold);

		m_animator = GetComponent<Animator>();
		Assert.IsNotNull(m_animator);
}

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		if (m_bShowAggroRadius)
		{
			UnityEditor.Handles.color = Color.red;
			UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, m_fAggroRadius);
		}

		if (m_bShowAttackRadius)
		{
			UnityEditor.Handles.color = Color.green;
			UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, GetCurrentAttackRange());
		}

		if(m_bShowMoveDirection)
		{
			UnityEditor.Handles.color = Color.blue;
			UnityEditor.Handles.DrawLine(transform.position, transform.position + m_vDirection * 3.0f);
		}
	}
#endif // UNITY_EDITOR

	private void FixedUpdate()
	{
		if (!isServer)
			return;

		//
		// Only server below

		if (m_eStatusToProcess == Status.dead && m_eStatus == Status.dead)
		{
			return;
		}

		//
		// First handle custom SetStatus (Idle->Walking, Walking->Idle, Dying->Death)
		if (m_eStatus == m_eStatusToProcess)
		{
			if (Status.walking == m_eStatus)
			{
				m_fDurationWalkToIdle += Time.fixedDeltaTime;
				if (m_fDurationWalkToIdle >= m_iMaxWalkToIdleThreshold)
				{
					m_animator.SetBool(m_astrStatusString[(int)(Status.walking)], false);
					SetStatus(Status.idle);
					m_fDurationWalkToIdle = 0.0f;
					m_agent.velocity = Vector3.zero;
				}
			}
			else if (Status.idle == m_eStatus)
			{
				m_fDurationIdleToWalk += Time.fixedDeltaTime;
				if (m_fDurationIdleToWalk >= m_fMaxIdleToWalkThresholdRandom)
				{
					m_animator.SetBool(m_astrStatusString[(int)(Status.walking)], true);
					SetStatus(Status.walking);
					m_fDurationIdleToWalk = 0.0f;
					m_fMaxIdleToWalkThresholdRandom = Random.Range(m_iMaxIdleToWalkThreshold * 0.5f, m_iMaxIdleToWalkThreshold);
				}
			}
			else if(Status.dying == m_eStatus && m_animator.GetCurrentAnimatorStateInfo(0).IsName(m_astrStatusString[(int)(m_eStatus)]) && m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
			{
				SetStatus(Status.dead);
			}
			else if(Status.dead == m_eStatusToProcess)
			{
				OnCharacterDeath();
			}
		}
		else
		{
			m_animator.SetBool(m_astrStatusString[(int)(m_eStatus)], false);
			m_animator.SetBool(m_astrStatusString[(int)(m_eStatusToProcess)], true);

			m_fDurationIdleToWalk = 0.0f;
			m_fDurationWalkToIdle = 0.0f;
		}

		//
		// Walking
		if (Status.walking == m_eStatusToProcess)
		{
			m_agent.isStopped = false;

			if(Status.walking != m_eStatus)
			{
				Vector2 vDirectionXZ = Random.insideUnitCircle;
				m_vDirection = new Vector3(vDirectionXZ.x, 0.0f, vDirectionXZ.y);
				transform.rotation = Quaternion.FromToRotation(Vector3.forward, m_vDirection);
			}

			m_agent.velocity = m_vDirection * GetCurrentSpeed();

		} // Tracking
		else if(Status.tracking == m_eStatusToProcess)
		{
			m_agent.isStopped = false;

			m_vDirection = (m_characterAggroed.gameObject.transform.position - transform.position).normalized;
			transform.rotation = Quaternion.FromToRotation(Vector3.forward, m_vDirection);
			m_agent.SetDestination(m_characterAggroed.gameObject.transform.position);

			m_agent.speed = GetCurrentSpeed() * 3.0f;
			m_agent.angularSpeed = GetCurrentSpeed() * 3.0f;
		}
		// Attacking
		else if(Status.attacking == m_eStatusToProcess)
		{
			if(m_characterAggroed.GetCurrentHP() > 0.0f)
			{
				AnimatorStateInfo info = m_animator.GetCurrentAnimatorStateInfo(0);
				if (info.IsName(m_astrStatusString[(int)(m_eStatusToProcess)]))
				{
					float fDistance = 0.0f;
					if(info.normalizedTime >= 0.5f && !m_bHasAttacked && ResolveAggroDetection(m_characterAggroed, ref fDistance))
					{
						m_characterAggroed.TakeDamage(m_fCurrentAttackStrength);
						m_bHasAttacked = true;
					}
					else if (info.normalizedTime < 0.2f && m_bHasAttacked)
					{
						m_bHasAttacked = false;
					}
				}
			}
		}
		else 
		{
			m_body.velocity = Vector3.zero;
			m_agent.isStopped = true;
		}

		//
		// Update status
		m_eStatus = m_eStatusToProcess;
	}

	public Status GetStatus()
	{
		return m_eStatus;
	}

	[Server]
	public void SetStatus(Status eStatus)
	{
		m_eStatusToProcess = eStatus;
	}

	public bool IsAggroing()
	{
		return m_eStatus == Status.attacking || m_eStatus == Status.tracking;
	}

	public BaseCharacter GetAggroedCharacter()
	{
		return m_characterAggroed;
	}

	[Server]
	public void SetAggroedCharacter(BaseCharacter character)
	{
		m_characterAggroed = character;
		if(null == m_characterAggroed)
		{
			m_agent.ResetPath();
		}
	}

	public bool ResolveAggroDetection(BaseCharacter character, ref float fDistance)
	{
		if (!character)
			return false;

		fDistance = Vector3.Distance(character.gameObject.transform.position, transform.position);

		return fDistance <= m_fAggroRadius;
	}


	[Server]
	protected override void OnCharacterDying()
	{
		base.OnCharacterDying();

		//
		// Notify
		((EnemyManager)(m_characterManager)).OnEnemyDying(this);
	}
}
