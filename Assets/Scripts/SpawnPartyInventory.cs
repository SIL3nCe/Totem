using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class SpawnPartyInventory : NetworkBehaviour
{
    public GameObject m_partyInventory;
    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<NetworkIdentity>().isServer)
        {
            
            NetworkServer.Spawn(GameObject.Instantiate(m_partyInventory, Vector3.zero, Quaternion.identity));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
