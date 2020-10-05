using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public ScriptableObjects.Item resourceDescriptor;
    public int resourceCount;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!GetComponent<NetworkIdentity>().isServer)
        {
            return;
        }

        if (other.gameObject.GetComponent<PlayerController>())
        {
            PartyInventory.Instance.CreditResource(resourceDescriptor, resourceCount);
            Destroy(this.gameObject);
        }
    }
}
