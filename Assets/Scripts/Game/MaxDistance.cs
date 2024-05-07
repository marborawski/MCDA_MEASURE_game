using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A component that determines the maximum distance the bullet can cover. After crossing this distance, it is destroyed
public class MaxDistance : MonoBehaviour
{

// Maximum possible distance that the bullet can cover
    public float max = 1;

    private Vector3 start;

    void Start()
    {
        start = transform.position;
    }

    void Update()
    {
        if (Vector3.Distance(start, transform.position) >= max)
        {
            Destroy(gameObject);
        }
    }
}
