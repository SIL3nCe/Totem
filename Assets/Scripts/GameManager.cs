using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
	[SyncVar]
	public float LoopDuration = 300.0f;
	[SyncVar]
	public float LoopTime = 300.0f;

	[Server]
	void Start()
	{
		LoopTime = LoopDuration;
	}

	[Server]
	void Update()
	{
		LoopTime -= Time.deltaTime;

		if (LoopTime <= 0.0f)
		{
			NetworkManager manager = GameObject.FindObjectOfType<NetworkManager>();
			manager.ServerChangeScene(NetworkManager.networkSceneName);
		}
	}
}
