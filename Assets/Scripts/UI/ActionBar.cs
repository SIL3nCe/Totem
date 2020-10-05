/**
 *  Author : Kranck
 *  Date : 2020/10/03
 *  Description : Represent the action bar of the player
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBar : MonoBehaviour
{
	
	[Tooltip("The inventory slots")]
	public GameObject[] m_slots = new GameObject[6];

	public delegate void DelegateActiveItemChanged(ActionBarItem item);
	public DelegateActiveItemChanged m_delegateActiveItemChanged;


	private void Start() 
	{
		foreach (var slot in m_slots)
		{
			slot.GetComponent<ActionBarSlot>().SetActionBar(this);
		}		
	}

	public GameObject GetFreeSlot()
	{
		foreach (var slot in m_slots)
		{
			if (!slot.GetComponentInChildren<ActionBarItem>().IsOccupied())
			{
				return slot;
			}
		}

		return null;	//< there is no available slot
	}

	public void NotifyActiveItemChanged(ActionBarItem item)
	{
		if (m_delegateActiveItemChanged != null)
		{
			m_delegateActiveItemChanged(item);
		}
	}
}
