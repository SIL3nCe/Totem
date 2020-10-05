/**
 *  Author : Kranck
 *  Date : 2020/10/03
 *  Description : Manages the inputs of the player for the UI
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

[RequireComponent(typeof(PlayerInput))]
public class PlayerControllerUI : MonoBehaviour
{
	
	#region Variables

		[Header("UI")]
		[Tooltip("The player UI that contains all the UI parts for the player")]
		private PlayerUI m_playerUI;
	
		[Header("Input Actions")]
		[SerializeField]
		[Tooltip("The action map that is used when the player is in game")]
		private string m_inGameInputActionMap;
		
		[SerializeField]
		[Tooltip("The action map that is used when the player is in the inventory")]
		private string m_inInventoryInputActionMap;
		
		[SerializeField]
		[Tooltip("The action map that is used when the player is in the menu")]
		private string m_inMenuInputActionMap;
		
		private PlayerInput m_playerInput;	//< The player input to access the inputs of the player
		private bool m_canSwitchState = true;	//< True when the player can switch state (ingame/menu/inventory), false otherwise. Auto-resetting via an invoke

		[Header("Input customisation")]
		private InputSystemUIInputModule m_inputSystemUIInputModule;
		public InputActionReference m_navigationInGameReference;
		public InputActionReference m_navigationInInventoryReference;

		private bool m_canCraft = true;
	
	#endregion
	
	
	// Start is called before the first frame update
	void Start()
	{
		//
		// Retrieve the player UI
		m_playerUI = GameObject.FindWithTag("PlayerUI").GetComponent<PlayerUI>();
		Assert.IsNotNull(m_playerUI, "Can't find the PlayerUI ! ");
		
		//
		// Retrieve the input system ui input module that is on the EventSystem object
		m_inputSystemUIInputModule = GameObject.FindWithTag("EventSystem").GetComponent<InputSystemUIInputModule>();
		Assert.IsNotNull(m_playerUI, "Can't find the EventSystem (so the InputSystemUIInputModule is null) ! ");
			
		//
		// Retrieve the player input
		m_playerInput = GetComponent<PlayerInput>();
		
		//
		// Replace the input reference in the input navigation
		m_inputSystemUIInputModule.enabled = false;
		m_inputSystemUIInputModule.move = m_navigationInGameReference;
		m_inputSystemUIInputModule.enabled = true;
	}

	// Update is called once per frame
	void Update()
	{
		if (m_playerInput.actions["Craft"].triggered)
		{
			Craft();
		}
		
		m_playerUI.SetHP(GetComponent<PlayerController>().CharacterPlayer.m_fCurrentHP, GetComponent<PlayerController>().CharacterPlayer.m_iMaxHP);
	}

	private void OnOpenInventory()
	{
		if (m_canSwitchState)
		{
			//
			// Disable the game action map and set the inventory action map
			m_playerInput.SwitchCurrentActionMap(m_inInventoryInputActionMap);
			m_playerUI.ChangeState(PlayerUI.State.inventory);
			
			//
			// Replace the input reference in the input navigation
			m_inputSystemUIInputModule.enabled = false;
			m_inputSystemUIInputModule.move = m_navigationInInventoryReference;
			m_inputSystemUIInputModule.enabled = true;

			//
			//
			m_canSwitchState = false;
			Invoke(nameof(ResetCanSwitchState), 1.0f);
		}
	}
	
	private void OnCloseInventory()
	{
		if (m_canSwitchState)
		{
			//
			// Disable the inventory action map and set the game action map
			m_playerInput.SwitchCurrentActionMap(m_inGameInputActionMap);
			m_playerUI.ChangeState(PlayerUI.State.ingame);
			
			//
			// Replace the input reference in the input navigation
			m_inputSystemUIInputModule.enabled = false;
			m_inputSystemUIInputModule.move = m_navigationInGameReference;
			m_inputSystemUIInputModule.enabled = true;

			
			//
			//
			m_canSwitchState = false;
			Invoke(nameof(ResetCanSwitchState), 1.0f);
		}
	}

	public bool IsInInventory()
	{
		return m_playerUI.CurrentState == PlayerUI.State.inventory;
	}

	public void Craft()
	{
		if (m_canCraft)
		{
			m_playerUI.Craft();
			m_canCraft = false;
			Invoke(nameof(ResetCanCraftDirty), 0.4f);
		}
	}

	public void ResetCanCraftDirty()
	{
		m_canCraft = true;
	}

	private void ResetCanSwitchState()
	{
		m_canSwitchState = true;
	}
}
