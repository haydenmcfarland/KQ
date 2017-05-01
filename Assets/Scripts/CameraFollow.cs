using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform target;
    public Vector3 offsetPosition;
    private Space offsetPositionSpace = Space.Self;
    private bool lookAt = true;
    private bool death = false;

    private void Update()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (!death)
        {
            if (offsetPositionSpace == Space.Self)
                transform.position = target.TransformPoint(offsetPosition);
            else
                transform.position = target.position + offsetPosition;

            if (lookAt)
                transform.LookAt(target);
            else
                transform.rotation = target.rotation;
        }
    }

    public void setDeath()
    {
        death = true;
    }
}