/**
 *  Author : Kranck
 *  Date : 2020/10/03
 *  Description : Manages the UI of the player
 **/

using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class ServerConnectGUI : MonoBehaviour
{

	public NetworkManager m_networkManager;

	public TMP_InputField m_inputFieldIP;
	public TMP_Dropdown m_connectTypeDropDown;

	public void Connect()
	{
		if (m_connectTypeDropDown.value == 0)
		{
			StartClient();
		}
		else
		{
			StartHost();
		}
	}


	public void StartClient()
	{
		m_networkManager.networkAddress = m_inputFieldIP.text;
		m_networkManager.StartClient();
	}

	public void StartHost()
	{
		m_networkManager.StartHost();
	}

	public void Exit()
	{
		Application.Quit();
	}
}
