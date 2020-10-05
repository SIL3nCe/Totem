using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class BaseCharacterManager : NetworkBehaviour
{
	[Header("Properties")]
	public string m_strName;

	[Header("Spawn")]
	public List<Vector2> m_aSpawnLocations;
	public float m_fSpawnRadius = 0.0f;

	[Header("Character prefabs")]
	[Tooltip(	"0 = none\n" +
				"1 = player\n" +
				"2 = enemy_zombie")
	]
	public GameObject[] m_mapCharacterToPrefab = new GameObject[System.Enum.GetNames(typeof(ECharacterType)).Length];

	protected List<BaseCharacter> m_aCharacters = new List<BaseCharacter>();

	[Server]
	protected bool GetSpawnParameters(ref Vector3 vPosition, ref Quaternion qOrn)
	{
		if(m_aSpawnLocations.Count == 0)
		{
			return false;
		}

		Vector2 vXZLocation = m_aSpawnLocations[Random.Range(0, m_aSpawnLocations.Count - 1)];
		Vector2 vXZLocationRandom = Random.insideUnitCircle;
		vXZLocationRandom.Normalize();
		float fSpawnRadiusRandom = m_fSpawnRadius * Random.Range(0.0f, 1.0f);
		vXZLocationRandom *= fSpawnRadiusRandom;
		vXZLocation += vXZLocationRandom;
		vPosition.Set(vXZLocation.x, 0.0f, vXZLocation.y);
		qOrn = Quaternion.Euler(0.0f, Random.Range(0.0f, 1.0f) * 360.0f, 0.0f);

		return true;
	}

	[Server]
	public bool SpawnCharacters(ECharacterType eType, int iCharactersCount, bool bSpawnOnServer, ref List<BaseCharacter> aBaseCharacters)
	{
		aBaseCharacters.Clear();

		for(int iCharacterIndex = 0; iCharacterIndex < iCharactersCount; ++iCharacterIndex)
		{
			//
			// Retrieve transform to random spawn location
			Vector3 vPosition = new Vector3();
			Quaternion qOrn = new Quaternion();
			if (!GetSpawnParameters(ref vPosition, ref qOrn))
			{
				return false;
			}

			//
			// Instantiate prefab
			GameObject prefab = m_mapCharacterToPrefab[(int)(eType)];
			if (null == prefab)
			{
				Debug.Log("Could not spawn character type '" + eType + "' from manager '" + m_strName + "'");
				continue;
			}
			
			GameObject newCharacterObject = Instantiate(prefab, vPosition, qOrn);
			BaseCharacter character = newCharacterObject.GetComponent<BaseCharacter>();
			character.m_characterManager = this;
			character.SetCharacterType(eType);

			//
			// Notify sub-class
			OnCharacterAboutToBeSpawned(eType, ref character);

			//
			// Spawn character on server
			if(bSpawnOnServer)
			{
				NetworkServer.Spawn(newCharacterObject);
			}

			//
			// Add character to list
			aBaseCharacters.Add(character);
			m_aCharacters.Add(character);
		}

		return true;
	}

	protected abstract void OnCharacterAboutToBeSpawned(ECharacterType eType, ref BaseCharacter baseCharacter);

	protected abstract void OnCharacterAboutToBeDestroyed(BaseCharacter baseCharacter);

	[Server]
	public bool DestroyCharacter(BaseCharacter character)
	{
		//
		// Remove object from list
		if(m_aCharacters.Remove(character))
		{
			//
			// Notify Character destruction
			OnCharacterAboutToBeDestroyed(character);

			//
			// Destroy gameobject
			NetworkServer.Destroy(character.gameObject);

			return true;
		}

		return false;
	}

	public List<BaseCharacter> GetCharacters()
	{
		return m_aCharacters;
	}

	public List<Vector2> GetSpawLocations()
	{
		return m_aSpawnLocations;
	}
}
