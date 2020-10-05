using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum ECharacterType
{
	none,

	player,
	enemy_zombie,
};

public class BaseCharacter : NetworkBehaviour
{
	[Header("Manager")]
	public BaseCharacterManager m_characterManager;

	[Header("Characteristics")]
	public int m_iMaxHP = 5;
	public float m_fSpeed = 10;
	public float m_fAttackStrength = 1;
	public float m_fAttackRange = 3;

	[Header("Loot")]
	public int m_iLootCountMin = 1;
	public int m_iLootCountMax = 1;
	public ScriptableObjects.Item m_iLootItem;

	[Header("Characteristics overriden")]
	[SyncVar]
	public float m_fCurrentHP;
	[SyncVar]
	public float m_fCurrentSpeed;
	[SyncVar]
	public float m_fCurrentAttackStrength;
	[SyncVar]
	public float m_fCurrentAttackRange;
	[SyncVar]
	private string m_strName;
	[SyncVar]
	private ECharacterType m_eType = ECharacterType.none;

	// Start is called before the first frame update
	protected void Start()
    {
		ResetCharacteristicsToInitialValues();
	}

	[Server]
	public void ResetCharacteristicsToInitialValues()
	{
		//
		// Current characteristic = initial characteristic
		m_fCurrentHP = m_iMaxHP;
		m_fCurrentSpeed = m_fSpeed;
		m_fCurrentAttackStrength = m_fAttackStrength;
		m_fCurrentAttackRange = m_fAttackRange;
	}

	[Server]
	protected virtual void OnCharacterDying()
	{
		// 
		// Drop item
		DropItem();
	}

	[Server]
	protected virtual void OnCharacterDeath() 
	{
		//
		// Call associated manager for destruction
		m_characterManager.DestroyCharacter(this);
	}

	[Server]
	public void TakeDamage(float fDamage)
	{
		m_fCurrentHP = Mathf.Max(m_fCurrentHP - fDamage, 0.0f);
		if(m_fCurrentHP <= 0.0f)
		{
			OnCharacterDying();
		}
	}

	protected void DropItem()
	{
		// todo
	}
	
	public float GetCurrentHP()
	{
		return m_fCurrentHP;
	}
	
	public float GetCurrentSpeed()
	{
		return m_fCurrentSpeed;
	}
	
	public float GetCurrentAttackStrength()
	{
		return m_fCurrentAttackStrength;
	}
	
	public float GetCurrentAttackRange()
	{
		return m_fCurrentAttackRange;
	}

	public void SetCharacterType(ECharacterType eType)
	{
		m_eType = eType;
	}

	public ECharacterType GetCharacterType()
	{
		return m_eType;
	}
}
