﻿using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class InteractableResource : NetworkBehaviour
{
    public ScriptableObjects.Item resourceDescriptor;

    public GameObjectHealthBar HealthBar;

    private int currentLife;

    // Start is called before the first frame update
    void Start()
    {
        currentLife = resourceDescriptor.m_life;
        if (HealthBar)
        {
            HealthBar.gameObject.SetActive(false);
        }
    }
    
    [Command(ignoreAuthority = true)]
    public void CmdGather(int itemDamage, ScriptableObjects.Item.WeaponType weaponType)
    {
        int damage = 1;

        if (resourceDescriptor.m_resourceType == ScriptableObjects.Item.ResourceType.wood && weaponType == ScriptableObjects.Item.WeaponType.axe        ||
            resourceDescriptor.m_resourceType == ScriptableObjects.Item.ResourceType.stone && weaponType == ScriptableObjects.Item.WeaponType.pickaxe   ||
            resourceDescriptor.m_resourceType == ScriptableObjects.Item.ResourceType.emerald && weaponType == ScriptableObjects.Item.WeaponType.pickaxe   )
        {
            damage = itemDamage;        
        }

        currentLife -= damage;

        if (currentLife <= 0)
        {
            List<GameObject> spawnedObjects = resourceDescriptor.Spawn(this.transform.position, 1);

            foreach (GameObject spawnedObject in spawnedObjects)
            {
                NetworkServer.Spawn(spawnedObject);
            }
            
            Destroy(gameObject);
        }
        else if (HealthBar)
        {
            UpdateHealthBar(currentLife);
        }
    }

    [ClientRpc]
    public void UpdateHealthBar(int currLife)
    {
        HealthBar.gameObject.SetActive(true);
        // Lerp in [0;max life]
        HealthBar.SetLifeBarScale(currLife / (float)resourceDescriptor.m_life);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isClient)
        {
            return;
        }
        
        Item item = other.gameObject.GetComponent<Item>();

        if (item == null || !item.IsInInteractFrame())
        {
            return;
        }

        PlayerController playerController = item.GetComponentInParent<PlayerController>();

        if (playerController == null || !playerController.isLocalPlayer || item.m_item.m_type != ScriptableObjects.Item.Type.weapon)
        {
            return;
        }
        
        CmdGather(item.m_item.m_damage, item.m_item.m_weaponType);
    }
}
