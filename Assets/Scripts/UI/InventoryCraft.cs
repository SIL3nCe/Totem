/**
 *  Author : Kranck
 *  Date : 2020/10/03
 *  Description : Manages the craft inventory
 **/

using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryCraft : MonoBehaviour
{
	#region Variables
	
		[Tooltip("The text that is used to display the description of the item to craft")]
		public TMP_Text m_descriptionText;

	#endregion
	
	#region Methods
	
		// Start is called before the first frame update
		void Start()
		{
			
		}

		// Update is called once per frame
		void Update()
		{
			//
			// 
			bool bTextSet = false;
			//
			// First we ensure there is a gameobject selected
			GameObject selected = EventSystem.current.currentSelectedGameObject;
			if (null != selected)
			{
				//
				// We ensure the selected gameobject has an inventory blueprint
				InventoryBlueprint inventoryBlueprint = selected.GetComponentInChildren<InventoryBlueprint>();
				if (null != inventoryBlueprint)
				{
					//
					// We can retrieve the blueprint
					ScriptableObjects.Blueprint selectedBlueprint = inventoryBlueprint.m_blueprint;
					
					//
					// If the blueprint is not null we can set its description
					if (null != selectedBlueprint)
					{
						//
						// update the description text depending on the current selected blueprint
						// If the blueprint is not unlocked, we simply display "???", otherwise we display the description
						// of the blueprint (which in fact is the description of the produced item)
						if (selectedBlueprint.StartLocked && !PartyInventory.Instance.IsBlueprintUnlocked(selectedBlueprint))
						{
							m_descriptionText.text = "???";
							bTextSet = true;
						}
						else
						{
							m_descriptionText.text = EventSystem.current.currentSelectedGameObject
								.GetComponentInChildren<InventoryBlueprint>().m_blueprint.Description;
							bTextSet = true;
						}
					}
				}
			}
			
			//
			// Set the text to null in case there is a problem and we don't set the text
			if (!bTextSet)
			{
				// 
				// If we are in debug we display an error, otherwise we display nothing
				if (Debug.isDebugBuild)
				{
					m_descriptionText.text = "THERE IS A PROBLEM IN THE CRAFT INVENTORY";	// In debug we indicate there is an error
				}
				else
				{
					m_descriptionText.text = "";	//< Not in debug we put nothing
				}
			}
		}
	
	#endregion 
}
