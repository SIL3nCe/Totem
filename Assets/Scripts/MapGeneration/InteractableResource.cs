using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class InteractableResource : NetworkBehaviour
{
    public ScriptableObjects.Item resourceDescriptor;

    private int currentLife;

    // Start is called before the first frame update
    void Start()
    {
        currentLife = resourceDescriptor.m_life;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    [Command(ignoreAuthority = true)]
    public void CmdGather(int damage)
    {
        currentLife -= damage;

        if (currentLife <= 0)
        {
            List<GameObject> spawnedObjects = resourceDescriptor.Spawn(this.transform.position, 1);

            foreach (GameObject spawnedObject in spawnedObjects)
            {
                NetworkServer.Spawn(spawnedObject);
            }
            
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!GetComponent<NetworkIdentity>().isClient)
        {
            return;
        }
        
        Item item = other.gameObject.GetComponent<Item>();

        if (item == null || !item.IsInInteractFrame())
        {
            return;
        }

        PlayerController playerController = item.GetComponentInParent<PlayerController>();

        if (playerController == null || !playerController.isLocalPlayer)
        {
            return;
        }
        
        CmdGather(item.m_item.m_damage);
    }
}
