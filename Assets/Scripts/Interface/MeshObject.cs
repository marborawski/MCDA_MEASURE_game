using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach the object to the group of objects managed by MeshOnOff. The MeshOnOff component will be able to activate and deactivate the object
public class MeshObject : MonoBehaviour
{
    void Start()
    {
        MeshOnOff meshOnOff = GetComponentInParent<MeshOnOff>();
        if (meshOnOff != null)
        {
            meshOnOff.Add(this);
        }
    }
}
