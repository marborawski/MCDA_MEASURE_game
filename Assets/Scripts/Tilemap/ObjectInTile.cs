using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Detecting opponents entering the tile
public class ObjectInTile : MonoBehaviour
{

// Opponent tag
    public string tagObjectEnemy;

// Tower tag
    public string tagObjectTower;

// Methods called when the opponent enters the tile area
    public UnityEvent objectInTile;

// An object specifying the cost of the distance traveled by the object
    public ManagementCost managenentCost;

    private TileCharacter tileCharacter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != null && other.tag == tagObjectEnemy)
        {
            objectInTile.Invoke();
            if (managenentCost != null)
            {
                managenentCost.AddNoHit(tileCharacter);
            }
        }
    }

    void Start()
    {
        tileCharacter = GetComponent<TileCharacter>();
    }
}
