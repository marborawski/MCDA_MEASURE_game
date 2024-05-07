using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Support for attacking the enemy's bullet through the tower (the BulletAtack module is attached to the bullet)
public class BulletAtack : MonoBehaviour
{

// Bullet strength (amount of damage dealt)
    public float strength = 2;

// Tag of the attacked object
    public string tagAttackObject;

    private TowerCharacter towerCharacter;

// Remembering the tower that fired the bullet
    public void AddTowerCharacter(TowerCharacter tc)
    {
        towerCharacter = tc;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == tagAttackObject)
        {
            Health h = other.gameObject.GetComponent<Health>();
            if (h != null)
            {
                h.SetHealth(strength, towerCharacter);
                h.HitObject();
            }
        }
    }
}
