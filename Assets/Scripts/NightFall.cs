using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NightFall : MonoBehaviour
{
	public GameManager m_gameManager;
	// public float m_timeBeforeEndInSeconds = 60;
	public float m_finalLightIntensity;
	public Color m_finalSkyTint;
	public Color m_finalSkyGround;
	public float m_finalSkyExposure;
	public float m_finalFogDensity;
	public Color m_finalFogColor;

	public Color m_finalLightColor;
	private float m_startTime;
	private float m_startLightIntensity;
	private Color m_startLightColor;

	private Color m_startSkyTint;
	private Color m_startSkyGround;
	[Range(0f, 1f)]
	private float m_startSkyExposure;
	private float m_startFogDensity;
	private Color m_startFogColor;

	private Gradient m_gradient;
	
	
	private bool m_executing = false;
	private Material m_originalMaterial;

	private void Start()
	{
		//
		//
		Material newMat = Instantiate(RenderSettings.skybox);
		m_originalMaterial = RenderSettings.skybox;
		RenderSettings.skybox = newMat;
		
		//
		//
		m_startLightIntensity = GetComponent<Light>().intensity;
		m_startLightColor = GetComponent<Light>().color;

		//
		//
		m_startSkyTint = RenderSettings.skybox.GetColor("_SkyTint");
		m_startSkyExposure = RenderSettings.skybox.GetFloat("_Exposure");

		//
		//
		m_startFogDensity = RenderSettings.fogDensity;
		m_startFogColor = RenderSettings.fogColor;
		
		Begin();
	}

	// Start is called before the first frame update
	public  void Begin()
	{
		//
		// We start everything only when we are not in hard mode
		m_startTime = Time.time;
		m_executing = true;
	}

	// Update is called once per frame
	void Update()
	{
		if (m_executing)
		{
			// float timeSinceStart = Time.time - m_startTime;
			float percentage = 1 - m_gameManager.LoopTime / m_gameManager.LoopDuration;

			if (percentage >= 1f)
			{
				return;
			}
			
			//
			//
			SetValues(percentage);
		}
	}

	private void SetValues(float percentage)
	{
		GetComponent<Light>().intensity = Mathf.Lerp(m_startLightIntensity, m_finalLightIntensity, percentage);
		GetComponent<Light>().color = Color.Lerp(m_startLightColor, m_finalLightColor, percentage);
			
		RenderSettings.skybox.SetColor("_SkyTint", Color.Lerp(m_startSkyTint, m_finalSkyTint, percentage));
		RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(m_startSkyExposure, m_finalSkyExposure, percentage));

		RenderSettings.fogDensity = Mathf.Lerp(m_startFogDensity, m_finalFogDensity, percentage);
		RenderSettings.fogColor = Color.Lerp(m_startFogColor, m_finalFogColor, percentage);
	}

	private void OnApplicationQuit()
	{
		RenderSettings.skybox = m_originalMaterial;
	}
}
