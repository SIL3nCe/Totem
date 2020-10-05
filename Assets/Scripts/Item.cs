/**
 *  Author : Kranck
 *  Date : 2020/10/03
 *  Description : Represent an item in the game
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

	[Header("Data")] 
	public ScriptableObjects.Item m_item;	//< The item data of this item

	private bool m_isInInteractFrame = false;
	
	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	public void SetIsInInteractFrame(bool isInInteractFrame)
	{
		m_isInInteractFrame = isInInteractFrame;
	}

	public bool IsInInteractFrame()
	{
		return m_isInInteractFrame;
	}
}
