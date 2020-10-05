using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Assertions;

public class CharacterPlayer : BaseCharacter
{
    public List<Texture> aSkins;
    public SkinnedMeshRenderer CharacterSkin;

    public PlayerController playerController;

    [SyncVar]
    public int skinId = 0;

    new public void Start()
    {
        base.Start();

        if (isServer)
        {
            skinId = Random.Range(0, aSkins.Count);
        }

        CharacterSkin.material.mainTexture = aSkins[skinId];
    }


	[Server]
	protected override void OnCharacterDying()
	{
		base.OnCharacterDying();

		//
		// Notify
		((PlayerManager)(m_characterManager)).OnPlayerDying(this);
        playerController.OnDeath();
    }

	[Server]
	protected override void OnCharacterDeath()
	{
		// todo ghost cam
	}
}
