using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public ScriptableObjects.Item resourceDescriptor;
    public int resourceCount;
    
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
