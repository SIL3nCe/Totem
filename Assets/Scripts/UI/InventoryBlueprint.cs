/**
 *  Author : Kranck
 *  Date : 2020/10/03
 *  Description : A blueprint on the inventory. It can be selected to craft an item
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class InventoryBlueprint : MonoBehaviour
{
	
	#region Variables
	
		[Tooltip("The data of the blueprint")]
		public ScriptableObjects.Blueprint m_blueprint;

		[Tooltip("The sprite when the blueprint is locked")]
		public Sprite m_lockedSprite;

		private bool m_isUnlocked = false;
		[SerializeField]
		private InventoryCraftRequiredResource[] m_inventoryRequiredResources = new InventoryCraftRequiredResource[4];

		/// <summary>
		/// The array of InventoryCraftRequiredResource that are used to display the required resources for a blueprint
		/// </summary>
		public InventoryCraftRequiredResource[] RequiredResourcesDisplay
		{
			get
			{
				return m_inventoryRequiredResources;
			}
		}

	#endregion
	
	#region Methods
		
	// Start is called before the first frame update
	void Start()
	{
		//
		// We ensure the blueprint is set
		Assert.IsNotNull(m_blueprint, "A blueprint in the inventory has not blueprint data ! (" + transform.name + ")");

		//
		// Set the blueprint icon
		SetIcon();
	}

	// Update is called once per frame
	void Update()
	{
		//
		// Set the blueprint icon
		if (!m_isUnlocked)
		{
			SetIcon();
		}
	}
	
	// Private

	/// <summary>
	/// Set the icon of the blueprint in the inventory. Sets a lock sprite if the blueprint has not been discovered
	/// </summary>
	private void SetIcon()
	{
		if (!PartyInventory.Instance)
		{
			return;
		}
		//
		// Set the blueprint icon
		if (null != m_blueprint)
		{
			// TODO: Optimize this since it is called in a critical section (update)
			//
			// If the blueprint is not available at the beginning and id it is not unlocked
			// in the party inventory, we put the lock sprite instead of the blueprint icon
			if (m_blueprint.StartLocked && !PartyInventory.Instance.IsBlueprintUnlocked(m_blueprint))
			{
				GetComponent<Image>().sprite = m_lockedSprite;	// display the locked sprite
			}
			else
			{
				GetComponent<Image>().sprite = m_blueprint.GetIcon();	// display the blueprint icon
				m_isUnlocked = true;	//< Unlock the blueprint (this will prevent from updating the icon every frame)
				
				//
				//
				if (m_blueprint.RecipeItems.Length > 4)
				{
					Debug.LogError("WARNING : The recipe (" + m_blueprint.name + ") has more than 4 recipe elements !");
				}
				
				//
				// Call all the InventoryCraftRequiredResource to det the required resources
				for (int requiredResourceIndex = 0; requiredResourceIndex < m_blueprint.RecipeItems.Length; ++requiredResourceIndex)
				{
					if (requiredResourceIndex >= 4)
					{
						break;	// just in case there are more than 4 ingredients, we do not access the 5th element because there are only 5 requiredResource element
					}
					RequiredResourcesDisplay[requiredResourceIndex].SetContent(m_blueprint.RecipeItems[requiredResourceIndex]
						, PartyInventory.Instance.HasEnoughResources(m_blueprint.RecipeItems[requiredResourceIndex].m_item, m_blueprint.RecipeItems[requiredResourceIndex].m_quantity)
						);	// Todo: 
				}
			}
		}
	}
	
	#endregion
}
