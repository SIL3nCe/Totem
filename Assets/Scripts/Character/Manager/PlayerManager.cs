using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : BaseCharacterManager
{

	private List<BaseCharacter> m_aPlayersAlive = new List<BaseCharacter>();
	private List<BaseCharacter> m_aPlayersDead = new List<BaseCharacter>();

	[Server]
	protected override void OnCharacterAboutToBeSpawned(ECharacterType eType, ref BaseCharacter baseCharacter)
	{
		m_aPlayersAlive.Add(baseCharacter);
	}

	[Server]
	protected override void OnCharacterAboutToBeDestroyed(BaseCharacter baseCharacter)
	{
		// TODO
	}

	public List<BaseCharacter> GetPlayersAlive()
	{
		return m_aPlayersAlive;
	}

	public List<BaseCharacter> GetPlayersDead()
	{
		return m_aPlayersDead;
	}

	public void OnPlayerDying(CharacterPlayer player)
	{
		m_aPlayersAlive.Remove(player);
		m_aPlayersDead.Add(player);
	}
}