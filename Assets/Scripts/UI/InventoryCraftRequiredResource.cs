/**
 *  Author : Kranck
 *  Date : 2020/10/03
 *  Description : Takes care of the required resources part in the invetory craft display
 **/

using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCraftRequiredResource : MonoBehaviour
{

	private Blueprint.RecipeElement m_neededElement;
	private bool m_satisfied = false;
	
	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		if (!m_satisfied && null != m_neededElement)
		{
			if (PartyInventory.Instance.HasEnoughResources(m_neededElement.m_item, m_neededElement.m_quantity))
			{
				SetContent(m_neededElement, true);
			}
		}

		if (m_satisfied)
		{
			if (!PartyInventory.Instance.HasEnoughResources(m_neededElement.m_item, m_neededElement.m_quantity))
			{
				SetContent(m_neededElement, false);
			}
		}
	}

	public void SetContent(Blueprint.RecipeElement _recipeElement, bool _satisfied)
	{
		//
		//
		m_satisfied = _satisfied;
		m_neededElement = _recipeElement;
		
		//
		// Set the icon (reset the alpha so it is visible)
		GetComponent<Image>().sprite = _recipeElement.m_item.m_icon;
		GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		
		//
		// Set the text (also reset the alpha and set the color depending on if the item is satisfied or not for the recipe)
		GetComponentInChildren<TMP_Text>().text = _recipeElement.m_quantity.ToString();
		if (_satisfied)
		{
			GetComponentInChildren<TMP_Text>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		}
		else
		{
			GetComponentInChildren<TMP_Text>().color = new Color(1.0f, 0.3f, 0.3f, 1.0f);
		}
	}
}
