using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
	public GameObject CharacterModel;
	public CharacterPlayer CharacterPlayer;

	public AudioListener PlayerAudioListener;
	public PlayerInput Inputs;
	
	public List<GameObject> equipableObjects;

	private Rigidbody rb;
	private Animator animator;
	
	private Transform handBone;
	
	[SyncVar(hook = nameof(OnEquippedObjectChanged))]
	private GameObject equippedObject;

	private Vector2 InputMoveValues;

	private bool bWalking = false;
	private bool bInteracting = false;

    private bool bResetInputs = true;
    private GameObject oldEquippedObject = null;

	bool bIsDying = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        
        handBone = animator.GetBoneTransform(HumanBodyBones.RightHand);
        Assert.IsNotNull(handBone);
        
        if (!hasAuthority || !isLocalPlayer)
        {
            Inputs.DeactivateInput();

            return;
        }

		Camera.main.gameObject.GetComponent<CameraManager>().PlayerTarget = gameObject;
		
		CmdEquipObject("");

		GameObject.Find("Action Bar").GetComponent<ActionBar>().m_delegateActiveItemChanged += OnActionBarItemChanged;
	}

	[Client]
	public void OnMove(InputValue value)
	{
		if (!hasAuthority || !isLocalPlayer)
			return;

		if (GetComponent<PlayerControllerUI>().IsInInventory())
		{
			InputMoveValues = Vector2.zero;
		}
		else
		{
			InputMoveValues = value.Get<Vector2>();
		}
	}

	[Client]
	public void OnAction(InputValue value)
	{
		if (!hasAuthority || !isLocalPlayer)
			return;

		if (bInteracting)
		{
			return;
		}

        bInteracting = true;
        animator.SetBool("Interact", true);
    }

	[ClientRpc]
	public void OnDeath()
    {
		if (!hasAuthority || !isLocalPlayer)
			return;

		rb.velocity = Vector3.zero;
		animator.SetBool("Walking", false);
		bWalking = false;

		animator.SetBool("Dying", true);
		bIsDying = true;
	}

	[Client]
	void FixedUpdate()
    {
		//
		// Do not update if dead
		if(CharacterPlayer.GetCurrentHP() <= 0)
		{
			rb.velocity = Vector3.zero;
			return;
		}
	    
        if (!hasAuthority || !isLocalPlayer)
        {
            if (oldEquippedObject != equippedObject)
            {
                OnEquippedObjectChanged(oldEquippedObject, equippedObject);
            }
            
            return;
        }

		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Interact"))
		{
			float normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
		
			if (equippedObject != null)
			{
				equippedObject.GetComponent<Item>().SetIsInInteractFrame(normalizedTime >= 0.33f && normalizedTime <= 0.80f);
			}   
		}

		float fHorizontal = InputMoveValues.x;
		float fVertical = InputMoveValues.y;
		if (fHorizontal != 0.0f || fVertical != 0.0f)
		{
			float fSpeed = CharacterPlayer.GetCurrentSpeed() * Time.deltaTime;

			rb.velocity = new Vector3(fHorizontal * fSpeed, rb.velocity.y, fVertical * fSpeed);

			float fAngle = 0.0f;
			if (fHorizontal >= 0.1f)
			{
				fAngle = 90.0f;
				if (fVertical > 0.0f)
				{
					fAngle -= 45.0f;
				}
				else if (fVertical < -0.0f)
				{
					fAngle += 45.0f;
				}
			}
			else if (fHorizontal < -0.0f)
			{
				fAngle = -90.0f;
				if (fVertical > 0.0f)
				{
					fAngle += 45.0f;
				}
				else if (fVertical < -0.0f)
				{
					fAngle -= 45.0f;
				}
			}
			else if(fVertical < 0.0f)
			{
				fAngle = -180.0f;
			}

			Quaternion rota = Quaternion.identity;
			transform.rotation = Quaternion.Euler(0.0f, fAngle, 0.0f);

			bWalking = true;
			animator.SetBool("Walking", true);
		}
		else if (bWalking)
		{
			rb.velocity = Vector3.zero;
			animator.SetBool("Walking", false);
			bWalking = false;
		}

		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Interact") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
		{
			bInteracting = false;
			animator.SetBool("Interact", false);
		}
	}

	[Client]
	void Update()
	{
		if (!hasAuthority || !isLocalPlayer)
		{
			if (bResetInputs)
			{
				Inputs.enabled = false;
			}
			return;
		}
		else
		{
			if (bResetInputs)
			{
				//Inputs.SwitchCurrentActionMap(Inputs.defaultActionMap);
				Inputs.SwitchCurrentControlScheme(Inputs.defaultControlScheme);
				Inputs.ActivateInput();
				bResetInputs = false;
			}

			if (bIsDying)
            {
				if (animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
				{
					// Need to reset parameter state since we use "Any State"
					animator.SetBool("Dying", false);
					bIsDying = false;
				}
			}
		}
	}

	[Command]
	private void CmdEquipObject(string name)
	{
		if (equippedObject != null)
		{
			Destroy(equippedObject);
			equippedObject = null;
		}

		if (name.Length == 0)
		{
			name = "Hand";
		}

		foreach(GameObject item in equipableObjects)
		{
			if (item.GetComponent<Item>().m_item.m_name == name)
			{
				GameObject spawnedObject = Instantiate(item, handBone.position, handBone.rotation);
				spawnedObject.transform.SetParent(handBone);
				
				NetworkServer.Spawn(spawnedObject);

				equippedObject = spawnedObject;

				break;
			}
		}
	}

    [Client]
    private void OnEquippedObjectChanged(GameObject oldEquippedObject, GameObject newEquippedObject)
    {
        if (!GetComponent<NetworkIdentity>().isServer && equippedObject != null)
        {
	        equippedObject.transform.SetParent(null);
	        
            equippedObject.transform.position = handBone.position;
            equippedObject.transform.rotation = handBone.rotation;
            equippedObject.transform.localScale = Vector3.one;
            equippedObject.transform.SetParent(handBone);   
        }
    }

	[Client]
	private void OnActionBarItemChanged(ActionBarItem item)
	{
		if (item.GetItem() == null)
		{
			CmdEquipObject("");
		}
		else
		{
			CmdEquipObject(item.GetItem().m_name);
		}
	}
}
