/**
 *  Author : Kranck
 *  Date : 2020/10/03
 *  Description : Manages the UI of the player
 **/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public class PlayerUI : MonoBehaviour
{
	
	#region Variables

		public enum State
		{
			ingame,
			inventory,
			menu
		}

		private State m_currentState = State.ingame;	//< By default the current state is the ingame state

		public State CurrentState
		{
			get
			{
				return m_currentState;
			}
		}
		private GameObject m_lastSelectedCraftBlueprint;	//< The last selected element in the inventory craft (to re-set correctly when changing UI)
		private GameObject m_lastSelectedActionBar;	//< The last selected element in the action bar (to re-set correctly when changing UI)

		[Header("GUIs")] 
		[Tooltip("The craft GUI")]
		public GameObject m_inventoryCraftGUI;
		[Tooltip("The action bar GUI")]
		public GameObject m_actionBarGUI;

		[Header("Selection")] 
		[Tooltip("The gameobject that is selected in first place in the action bar")]
		public GameObject m_firstSelectedActionBar;
		[Tooltip("The gameobject that is selected in first place in the craft inventory")]
		public GameObject m_firstSelectedCraftBlueprint;

		[Serializable]
		public class ItemToTextQuantity
		{
			public ScriptableObjects.Item m_item;
			public PartyResource m_partyResource;
		}
		public List<ItemToTextQuantity> m_perResourceQuantityText = new List<ItemToTextQuantity>();

		[SerializeField]
		private TMP_Text m_informationText;

		public TMP_Text m_hpText;
		
	#endregion 
	
	#region Methods
	
		// Start is called before the first frame update
		void Start()
		{
			//
			// We ensure the GUIs are not null
			Assert.IsNotNull(m_inventoryCraftGUI, "The inventory craft GUI is null (PlayerUI)");
			Assert.IsNotNull(m_actionBarGUI, "The action bar GUI is null (PlayerUI)");
			
			//
			// Store the first selected objects as the last selected objects
			// this way the first selected objects will be selected correclty when starting
			m_lastSelectedActionBar = m_firstSelectedActionBar;
			m_lastSelectedCraftBlueprint = m_firstSelectedCraftBlueprint;
			
			//
			// We call change state to change to the current state
			ChangeState(m_currentState);
			
			//
			// Reset the information text
			m_informationText.text = "";
		}

		// Update is called once per frame
		void Update()
		{
			//
			// TODO: Move for optimization ?
			UpdateResourcesCount();
			
			//
			// Update the information text depending on the selected element
			if (null != EventSystem.current.currentSelectedGameObject.GetComponentInChildren<InventoryBlueprint>())
			{
				Blueprint blueprint = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<InventoryBlueprint>().m_blueprint;
				if (null == blueprint)
				{
					if (Debug.isDebugBuild)
					{
						m_informationText.text = "(DEBUG) : INVALID BLUEPRINT";
					}
					else
					{
						m_informationText.text = "";
					}
				}
				else
				{
					if (blueprint.StartLocked && !PartyInventory.Instance.IsBlueprintUnlocked(blueprint))
					{
						m_informationText.text = "You don't know how to craft this for now !";
					}
					else
					{
						//
						//
						if (!PartyInventory.Instance.CanCraft(blueprint))
						{
							m_informationText.text = "You don't have enough materials to craft this !";
						}
						else
						{
							m_informationText.text = "Press <sprite=1>/<sprite=2> to craft this item !";
						}
					}
				}
			}


		}
		
		private void UpdateResourcesCount()
		{
			if (!PartyInventory.Instance)
			{
				return;
			}
			
			foreach (var itemToTextQuantity in m_perResourceQuantityText) // loop through both
			{
				if (PartyInventory.Instance.Resources.ContainsKey(itemToTextQuantity.m_item.name))
				{
					itemToTextQuantity.m_partyResource.UpdateQuantity(PartyInventory.Instance.Resources[itemToTextQuantity.m_item.name]);
				}
				else
				{
					Debug.LogError("Can't update the quantity of the resource \"" + itemToTextQuantity.m_item.name + "\" because it has not been added to the " +
						" available resource in PlayerUI");
				}
			}
		}

		public void ChangeState(State _newState)
		{
			//
			// Store the lastly selected elements depending on the current GUI state
			// We do this only if there is a current selection to avoid problems (it is mandatory for the first call to this (in Start) to work correctly)
			if (null != EventSystem.current.currentSelectedGameObject)
			{
				switch (m_currentState)
				{
					case State.ingame:
					{
						m_lastSelectedActionBar = EventSystem.current.currentSelectedGameObject;
					} break;
					case State.inventory:
					{
						m_lastSelectedCraftBlueprint = EventSystem.current.currentSelectedGameObject;
					} break;
				}
			}

			//
			// Activate and deactivate the GUIs depending on the new state
			switch (_newState)
			{
				case State.ingame:
				{
					//
					// When in game we disable the craft UI and the menu and enable the action bar
					m_inventoryCraftGUI.SetActive(false);
					m_actionBarGUI.SetActive(true);
					// TODO: Disable menu GUI
					
					//
					// Re-set the previously selected object
					EventSystem.current.SetSelectedGameObject(m_lastSelectedActionBar);
				} break;
				case State.inventory:
				{
					//
					// When in inventory we enable the craft UI and enable the action bar, we also disable the menu
					m_inventoryCraftGUI.SetActive(true);
					m_actionBarGUI.SetActive(true);
					// TODO: Disable menu GUI
					
					//
					// Re-set the previously selected object
					EventSystem.current.SetSelectedGameObject(m_lastSelectedCraftBlueprint);
				} break;
				case State.menu:
				{
					//
					// When in menu we disable everything except the menu
					m_inventoryCraftGUI.SetActive(false);
					m_actionBarGUI.SetActive(false);
					// TODO: Enable menu GUI
				} break;
				default:
					throw new ArgumentOutOfRangeException(nameof(_newState), _newState, null);
			}
			
			//
			// Store the new state as the current state
			m_currentState = _newState;
		}

		public void Craft()
		{
			//
			// Retrieve the currently selected blueprint
			Blueprint blueprint = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<InventoryBlueprint>().m_blueprint;
			
			//
			// We check if the blueprint is unlocked
			if (blueprint.StartLocked && !PartyInventory.Instance.IsBlueprintUnlocked(blueprint))
			{
				// TODO: Send error message
				Debug.LogWarning("This blueprint is locked ! ");
				return;
			}
			
			//
			// Then we check we can craft the object
			if (!PartyInventory.Instance.CanCraft(blueprint))
			{
				// TODO: Send error message
				Debug.LogWarning("Can't craft " + blueprint.name + " !");
				return;
			}
			
			//
			// Okay so we can craft it.
			// First we uncredit all the recipe elements
			foreach (var recipeItem in blueprint.RecipeItems)
			{
				PartyInventory.Instance.UnCreditResource(recipeItem.m_item.name, recipeItem.m_quantity);
			}
			
			//
			// Now the recipe elements have been uncredited, there are two solutions
			// Solution 1 : The player still have available slots in its action bar, int this case we simply add the result item in the action bar
			// in the first available slot
			// Solution 2 : The player don't have available slots in its action bar, in this case we spawn the object at the player's feet
			GameObject freeSlot = m_actionBarGUI.GetComponent<ActionBar>().GetFreeSlot();
			if (null != freeSlot)
			{
				//
				// We have a free slot, we can simply put the item in it
				freeSlot.GetComponentInChildren<ActionBarItem>().SetItem(blueprint.Product);
			}
			else
			{
				// TODO: Spawn at the right position
				blueprint.Product.Spawn(Vector3.zero);
			}


		}

		public void SetHP(float _hp, float _max)
		{
			m_hpText.text = "Life : " + _hp.ToString() + " / " + _max.ToString();
		}
		
	#endregion
}
