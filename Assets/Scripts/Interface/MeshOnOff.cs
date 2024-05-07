using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Activating and deactivating objects
public class MeshOnOff : MonoBehaviour
{

// List of objects activated and deactivated by MeshOnOff
    private List<MeshObject> meshObjects = new List<MeshObject>();

    private bool on;

// Adding an object to the list of objects that can be activated and deactivated
    public void Add(MeshObject meshObject)
    {
        if (meshObject != null)
        {
            meshObjects.Add(meshObject);
            meshObject.gameObject.SetActive(on);
        }
    }

// Activate and deactivate objects on the list
// YN - false deactivate, true activate objects
    public void OnOff(bool YN)
    {
        if (meshObjects != null)
        {
            foreach (MeshObject item in meshObjects)
            {
                if (item != null)
                {
                    item.gameObject.SetActive(YN);
                }
            }
        }
    }

// Change the state of objects on the list when active to inactive, and when inactive to active
    public void Change()
    {
        on = !on;
        OnOff(on);
    }

    void Start()
    {
        OnOff(false);
        on = false;
    }
}
