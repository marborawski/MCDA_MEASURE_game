using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Component containing the unique object identifier
public class IdObject : MonoBehaviour
{
    private static int counter = 0;

    private int id;

// Get the unique object identifier
// Returns the unique identifier of the object
    public int GetId()
    {
        return id;
    }

    void Start()
    {
        counter++;
        id = counter;
    }

}
