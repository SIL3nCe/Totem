/**
 *  Author : Kranck
 *  Date : 2020/10/03
 *  Description : Base blueprint (craft) scriptable object
 **/

using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace ScriptableObjects
{
	[CreateAssetMenu(menuName = "La Pantera/Blueprints/Create new blueprint", fileName = "New Blueprint", order = 2)]
	public class Blueprint : ScriptableObject
	{
		
	#region Variables
	
		/// <summary>
		/// A recipe element represent a resource we need for a blueprint's recipe
		/// Each element consists of an item and a quantity needed
		/// </summary>
		[Serializable]
		public class RecipeElement
		{
			public Item m_item;		//< The item for this element
			public int m_quantity;	//< The amount of this item needed to satisfy this element
		}
	
		[Header("General")] 
		[SerializeField]
		[Tooltip("The name of the blueprint")]
		private string m_name;
		/// <summary>
		/// The description of this blueprint, we simply return the description of the produced item
		/// </summary>
		public string Description
		{
			get
			{
				Assert.IsNotNull(m_produced, "Trying to get the description of a blueprint that has no prorduced item (" + name + ")");
				return m_produced.m_description;
			}
		}

		[SerializeField]
		[Tooltip("Is this blueprint available from the beginning or is it locked (something to do to discover it)")]
		private bool m_availableAtStart = true;
		public bool StartLocked
		{
			get { return !m_availableAtStart;  }
		}

		[Header("Visual")]
		[SerializeField]
		[Tooltip("The icon of the blueprint. If nothing is set, the sprite used is the one of the produced item")]
		private Sprite m_icon;
	
		[Header("Recipe")] 
		[SerializeField]
		[Tooltip("The elements needed for the recipe")]
		private RecipeElement[] m_recipeElements;
		/// <summary>
		/// All the elements needed for the recipe
		/// </summary>
		public RecipeElement[] RecipeItems
		{
			get { return m_recipeElements; }
		}
	
		[Header("Product")] 
		[SerializeField]
		[Tooltip("The produced item")]
		private Item m_produced;
		/// <summary>
		/// The item produced by this blueprint
		/// </summary>
		public Item Product
		{
			get { return m_produced;  }
		}
	
	#endregion

	#region Methods

	/// <summary>
		/// Return the icon of this blueprint
		/// </summary>
		/// <returns>The icon of this blueprint. If the blueprint has no icon, the icon is the one of the produced item</returns>
		public Sprite GetIcon()
		{
			//
			// If the icon is null
			if (null == m_icon)
			{
				// We return the icon of the produced item
				return m_produced.m_icon;
			}
			else
			{
				// We return the icon of the blueprint
				return m_icon;
			}
		}
	
	#endregion
	
	}
}
