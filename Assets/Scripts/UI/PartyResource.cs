/**
 *  Author : Kranck
 *  Date : 2020/10/03
 *  Description : Manages the inventory of the party
 **/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyResource : MonoBehaviour
{

	[Tooltip("The item that will be used to display the image and name of the resource")]
	public ScriptableObjects.Item m_item;
	
	// Start is called before the first frame update
	void Start()
	{
		GetComponent<Image>().sprite = m_item.m_icon;
	}

	public void UpdateQuantity(int _quantity)
	{
		GetComponentInChildren<TMP_Text>().text = "" + m_item.m_name + " : " + _quantity.ToString();
	}
}
