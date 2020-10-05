using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using Mirror;

public class NetworkManagerCustom : NetworkManager
{
	public bool SeedIsInit = false;
	public Random.State RandomSeed;

	public PlayerManager m_PlayerManager;

	public override void OnServerAddPlayer(NetworkConnection conn)
	{
		List<BaseCharacter> aCharacters = new List<BaseCharacter>();
		if(m_PlayerManager.SpawnCharacters(ECharacterType.player, 1, false, ref aCharacters))
		{
			Assert.IsTrue(aCharacters.Count == 1);

			NetworkServer.AddPlayerForConnection(conn, aCharacters[0].gameObject);
		}
		else
		{
			Assert.IsTrue(false);
		}
	}

	public override void OnServerSceneChanged(string sceneName)
	{
		if(sceneName.Equals("Assets/Scenes/SampleScene.unity"))
		{
			EnemyManager enemyManager = FindObjectOfType<EnemyManager>();
			Assert.IsNotNull(enemyManager);

			enemyManager.m_playerManager = m_PlayerManager;
		}
	}
}
