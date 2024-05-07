using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Module that causes the object to rotate facing the camera
public class Billboard : MonoBehaviour
{

    // The camera towards which the object will rotate
    public new Transform camera;

    void LateUpdate()
    {
        if (camera != null)
        {
            transform.LookAt(camera.position + camera.forward);
        }
    }
}
