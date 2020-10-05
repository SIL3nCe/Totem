/**
 *  Author : Kranck
 *  Date : 2020/10/03
 *  Description : Represent an item in the action bar
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ActionBarItem : MonoBehaviour
{
	
	#region Variables

		[SerializeField]
		[Tooltip("The item that is stored in this action slot")]
		private ScriptableObjects.Item m_item;
		
		private Image m_image;	//< The image that is used to display the item icon
	
	#endregion
	// Start is called before the first frame update
	void Start()
	{
		//
		// Retrieve the image
		m_image = GetComponent<Image>();
	}

	public void SetItem(ScriptableObjects.Item item)
	{
		m_item = item;
	}

	// Update is called once per frame
	void Update()
	{
		//
		//
		if (null == m_item)
		{
			m_image.color = new Color(m_image.color.r, m_image.color.g, m_image.color.b, 0.0f);
		}
		else
		{
			m_image.color = new Color(m_image.color.r, m_image.color.g, m_image.color.b, 1.0f);
			m_image.sprite = m_item.m_icon;
		}
	}

	public void Drop(Vector3 _position)
	{
		m_item.Spawn(_position);
		m_item = null;
	}
	
	public bool IsOccupied()
	{
		return null != m_item;
	}

	public ScriptableObjects.Item GetItem()
	{
		return m_item;
	}
}
