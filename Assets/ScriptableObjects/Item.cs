/**
 *  Author : Kranck
 *  Date : 2020/10/03
 *  Description : Base item scriptable object
 **/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ScriptableObjects
{
	[CreateAssetMenu(menuName = "La Pantera/Items/Create new item", fileName = "New Item", order = 1)]
	public class Item : ScriptableObject
	{
		
#region Variables

		/// <summary>
		/// The type of resources
		/// </summary>
		public enum Type
		{
			resource,	//< A resource. Can be gathered and is shared across all players
			placable,	//< An object that can be placed. Not shared across players
			weapon	//< A weapon that can attack ennemies or gather resources
		}

		[Header("General")] 
		[Tooltip("The name of the item")]
		public string m_name;

		[Tooltip("The description of the item that is displayed")]
		public string m_description;

		[Tooltip("The type of this item")] 
		public Type m_type;

		[Tooltip("Damage if the item is a weapon")]
		public int m_damage;

		[Tooltip("Life if the item is a resource")]
		public int m_life;

		[Header("Visual")]
		[Tooltip("The icon of the item")]
		public Sprite m_icon;

		[Tooltip("The \"in-game\" representation of the item : the prefab of the item")]
		public GameObject m_prefab;
		
#endregion

#region Methods

		/// <summary>
		/// Spawns the item at the given position in _amount copies
		/// </summary>
		/// <param name="_position">The position to spawn at. There is a small delta that is applied</param>
		/// <param name="_amount">The amount of items to spawn</param>
		public List<GameObject> Spawn(Vector3 _position, int _amount = 1)
		{
			//
			// Ensure the prefab is not null, otherwise we assert
			Assert.IsNotNull(m_prefab, "Tried to spawn an item that has a null prefab (" + m_name + ")");
			
			List<GameObject> spawnedObjects = new List<GameObject>();
			
			//
			// We instantiate the prefab
			if (null != m_prefab)
			{
				//
				// Spawn the amount of 
				for (int count = 0; count < _amount; count++)
				{
					//
					// Spawn an object by randomizing the x and z 
					float xDelta = Random.Range(-0.4f, 0.4f);
					float zDelta = Random.Range(-0.4f, 0.4f);
					spawnedObjects.Add(GameObject.Instantiate(m_prefab,
								new Vector3(_position.x + xDelta, _position.y + 0.2f, _position.z + zDelta), Quaternion.identity));
				}
			}

			return spawnedObjects;
		}

#endregion 

	}
}

