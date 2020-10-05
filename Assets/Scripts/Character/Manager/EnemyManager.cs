using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.AI;
using Mirror;

public class EnemyManager : BaseCharacterManager
{
	[Header("EnemyManager custom properties")]
	public PlayerManager m_playerManager;

	private List<CharacterEnemy> m_aEnemiesNotAggroed = new List<CharacterEnemy>();
	private List<CharacterEnemy> m_aEnemiesAggroed = new List<CharacterEnemy>();
	
	private void FixedUpdate()
	{
		if(!isServer)
		{
			return;
		}

		//
		// First check for dead enemies
		int iEnemyIndex = 0;
		while (iEnemyIndex < m_aCharacters.Count)
		{
			CharacterEnemy enemy = m_aCharacters[iEnemyIndex] as CharacterEnemy;
			CharacterEnemy.Status status = enemy.GetStatus();
			if (status == CharacterEnemy.Status.dead)
			{
				base.DestroyCharacter(enemy);
				--iEnemyIndex;
			}
			++iEnemyIndex;
		}

		//
		// Then check validity of aggroed status
		List<CharacterEnemy> aEnemiesTemp = new List<CharacterEnemy>();
		foreach(CharacterEnemy enemy in m_aEnemiesAggroed)
		{
			BaseCharacter aggroedCharacter = enemy.GetAggroedCharacter();
			if (null != aggroedCharacter)
			{
				if(aggroedCharacter.GetCurrentHP() > 0.0f)
				{
					float fDistance = 0.0f;
					if (enemy.ResolveAggroDetection(aggroedCharacter, ref fDistance))
					{
						if (fDistance < enemy.GetCurrentAttackRange())
						{
							if (enemy.GetStatus() != CharacterEnemy.Status.attacking)
							{
								enemy.SetStatus(CharacterEnemy.Status.attacking);
							}
						}
						else
						{
							if (enemy.GetStatus() != CharacterEnemy.Status.tracking)
							{
								enemy.SetStatus(CharacterEnemy.Status.tracking);
							}
						}
					}
					else
					{
						enemy.SetAggroedCharacter(null);
						if (enemy.GetStatus() != CharacterEnemy.Status.idle || enemy.GetStatus() != CharacterEnemy.Status.walking)
						{
							enemy.SetStatus(CharacterEnemy.Status.idle);
							aEnemiesTemp.Add(enemy);
						}
					}
				}
				else
				{
					enemy.SetAggroedCharacter(null);
					enemy.SetStatus(CharacterEnemy.Status.idle);
					aEnemiesTemp.Add(enemy);
				}
			}
		}
		foreach (CharacterEnemy enemy in aEnemiesTemp)
		{
			m_aEnemiesAggroed.Remove(enemy);
			m_aEnemiesNotAggroed.Add(enemy);
		}

		//
		// Then check for idle/walking
		aEnemiesTemp.Clear();
		foreach (CharacterEnemy enemy in m_aEnemiesNotAggroed)
		{
			foreach(BaseCharacter character in m_playerManager.GetPlayersAlive())
			{
				float fDistance = 0.0f;
				if(enemy.ResolveAggroDetection(character, ref fDistance))
				{
					enemy.SetAggroedCharacter(character);

					//
					// Set status according to distance
					if (fDistance < enemy.GetCurrentAttackRange())
					{
						enemy.SetStatus(CharacterEnemy.Status.attacking);
					}
					else
					{
						enemy.SetStatus(CharacterEnemy.Status.tracking);
					}
					aEnemiesTemp.Add(enemy);
				}
			}
		}
		foreach (CharacterEnemy enemy in aEnemiesTemp)
		{
			m_aEnemiesNotAggroed.Remove(enemy);
			m_aEnemiesAggroed.Add(enemy);
		}
	}

	[Server]
	protected override void OnCharacterAboutToBeSpawned(ECharacterType eType, ref BaseCharacter baseCharacter)
	{
		switch(eType)
		{
			case ECharacterType.enemy_zombie:
			{
				CharacterEnemy enemy = baseCharacter as CharacterEnemy;
				if(null != enemy)
				{
					m_aEnemiesNotAggroed.Add(enemy);

					enemy.gameObject.transform.parent = gameObject.transform;
				}
			}
			break;

			case ECharacterType.none:
			case ECharacterType.player:
			default:
			{
				Assert.IsTrue(false);
			}
			break;
		}
	}

	[Server]
	protected override void OnCharacterAboutToBeDestroyed(BaseCharacter baseCharacter)
	{
		switch(baseCharacter.GetCharacterType())
		{
			case ECharacterType.enemy_zombie:
			{
				CharacterEnemy enemy = baseCharacter as CharacterEnemy;
				if(null != enemy)
				{
					//
					// Remvoe enemy from no/aggroed array
					m_aEnemiesAggroed.Remove(enemy);
					m_aEnemiesNotAggroed.Remove(enemy);
				}
			}
			break;

			case ECharacterType.none:
			case ECharacterType.player:
			default:
			{
				Assert.IsTrue(false);
			}
			break;
		}
	}

	public void OnEnemyDying(CharacterEnemy enemy)
	{
		m_aEnemiesAggroed.Remove(enemy);
		m_aEnemiesNotAggroed.Remove(enemy);
	}
}
