using System.Collections;
using System.Collections.Generic;
using CG_Base;
using UnityEngine;
using UnityEngine.Events;
using static UnityServer;

[System.Serializable]
// Represents tiles on which an enemy can be hit
public class UnityEventHealthHitObject : UnityEvent<TileCharacter>
{
}

// Information about the enemy's health level
public class Health : CollectionEvents
{

// Enemy's starting health level
    public float startHealth = 5;

    private float health;

// Enemy's armor determines the level of resistance to projectile attacks
    public float armour = 1;

// Cost of creating an enemy
    public float coins = 20;

// Enemy tag
    public string enemyTag;

    private HealthBarInterface healthBarInterface;

// Cash manager
    public ManagementCash managementCash;

// Indicating methods related to the tile the enemy is standing on, which will be called after hitting the enemy
    public UnityEventHealthHitObject hit;

    private float lastHit = 0;

    private TileCharacter actualTile;

    private bool destruction = false;

    private EnemyCharacter enemyCharacter;

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == enemyTag)
        {
            TileCharacter tc =  other.gameObject.GetComponent<TileCharacter>();
            if (tc != null)
            {
                actualTile = tc;
            }
        }
    }

// Passing information about hitting the correct tile
    public void HitObject()
    {
        hit.Invoke(actualTile);
    }

// Method called when the enemy is hit by the projectile
// h              - bullet strength (amount of damage dealt)
// towerCharacter - the tower that hit the enemy
    public void SetHealth(float h, TowerCharacter towerCharacter)
    {
        if (!destruction)
        {
            lastHit = h - armour;
            if (lastHit > 0)
            {
                Event4StringData event4StringData = new Event4StringData();
                if (enemyCharacter != null)
                    event4StringData.data1 = enemyCharacter.type;
                if (towerCharacter != null)
                    event4StringData.data2 = towerCharacter.type;
                health -= lastHit;
                RunEvent("ChangeHealth", h.ToString(), event4StringData);
                if (healthBarInterface == null)
                    healthBarInterface = GetComponentInChildren<HealthBarInterface>();
                if (healthBarInterface != null)
                {
                    healthBarInterface.SetHealth(health / startHealth);
                }
                if (health <= 0)
                {
                    if (managementCash != null)
                    {
                        managementCash.AddCoins(coins);
                    }
                    RunEvent("DestroyEnemy", "", event4StringData);
                    destruction = true;
                    Destroy(gameObject, 0.5f);
                }
            }
            else
            {
                lastHit = 0;
            }
        }
    }

// Retrieving information about the enemy's life level
// Returns the current health level
    public float GetHealth()
    {
        return health;
    }

    protected override void Start()
    {
        base.Start();
        healthBarInterface = GetComponentInChildren<HealthBarInterface>();
        health = startHealth;
        if (managementCash == null)
        {
            managementCash = FindObjectOfType<ManagementCash>();
        }
        enemyCharacter = GetComponent<EnemyCharacter>();
    }

    protected override void Update()
    {
        base.Update();
    }
}
