using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalTotem : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		Invoke(nameof(StartLight), 1.0f);
	}

	public void StartLight()
	{
		var lights = GetComponentsInChildren<Light>();
		foreach (var light in lights)
		{
			light.enabled = true;
		}
	}
}
