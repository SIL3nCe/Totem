using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject PlayerTarget;

    public float YDistance = 10.0f;
    public float ZDistance = -10.0f;
    public float XRotation = 35.0f;

    void Start()
    {
    }

    void Update()
    {
        if (!PlayerTarget)
            return;

        Vector3 vCameraLoc = PlayerTarget.transform.position;
        vCameraLoc.y += YDistance;
        vCameraLoc.z += ZDistance;

        transform.position = vCameraLoc;
        transform.rotation = Quaternion.AngleAxis(35.0f, Vector3.right);
    }
}
