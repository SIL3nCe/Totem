using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class ActionBarSlot : MonoBehaviour, ISelectHandler
{
    private ActionBar m_actionBar;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActionBar(ActionBar actionBar)
    {
        m_actionBar = actionBar;
    }
    
    public void OnSelect(BaseEventData eventData)
    {
        m_actionBar.NotifyActiveItemChanged(GetComponentInChildren<ActionBarItem>());
    }
}
