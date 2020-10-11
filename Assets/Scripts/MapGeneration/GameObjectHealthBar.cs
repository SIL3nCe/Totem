using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameObjectHealthBar : MonoBehaviour
{
    public GameObject LifeBar;

    private float fBaseXScale = 1.0f;
    private bool bInit = true;

    public void SetLifeBarScale(float fPercent)
    {
        if (bInit)
        {
            // X Scale init has been moved here since Start never had a valid LifeBar
            Assert.IsNotNull(LifeBar);
            fBaseXScale = LifeBar.transform.localScale.x;
            bInit = false;
        }

        Vector3 vScale = LifeBar.transform.localScale;
        vScale.x = fBaseXScale * fPercent;
        LifeBar.transform.localScale = vScale;
    }
}
