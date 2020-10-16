using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class ActionBarSlot : MonoBehaviour, ISelectHandler
{
    private ActionBar m_actionBar;

    public void SetActionBar(ActionBar actionBar)
    {
        m_actionBar = actionBar;
    }
    
    public void OnSelect(BaseEventData eventData)
    {
        m_actionBar.NotifyActiveItemChanged(GetComponentInChildren<ActionBarItem>());
    }
}
