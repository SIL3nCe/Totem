using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIBackToMenuWhenOffline : MonoBehaviour
{
	public void Execute()
	{
		SceneManager.LoadScene("BaseNetworkScene");
	}
}
