using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Enemies generating module
public class EnemyInfo : MonoBehaviour
{
    [System.Serializable]
// Information about enemy
    public class EnemyParams
    {
// 3D model of the enemy
        public GameObject model;
// Enemy speed
        public float speed = 1;
// Enemy's starting health level
        public float startHealth = 5;
// Enemy's armor determines the level of resistance to projectile attacks
        public float armour = 1;
// Cost of creating an enemy
        public float coins = 10;
// Profit for towers when killing an enemy
        public float destroyCoins = 20;
// Gain for the enemy manager if the enemy reaches the end of the path
        public float coinsToEnd = 15;
// Number of available enemies (-1 means unlimited number of available enemies)
        public int count = -1;
// Enemy tag
        public string tag = "Enemy";
// Enemy type
        public string type = "";
        [HideInInspector]
// Number of enemies already generated
        public int actualGenerateCount = 0;
        // Setting the enemy pattern parameters
        // enemy - an object storing the parameters of the enemy pattern
        public void SetData(EnemyParams enemy)
        {
            speed = enemy.speed;
            startHealth = enemy.startHealth;
            armour = enemy.armour;
            destroyCoins = enemy.destroyCoins;
            coins = enemy.coins;
            count = enemy.count;
            tag = enemy.tag;
            type = enemy.type;
            coinsToEnd = enemy.coinsToEnd;
        }
    }

// An array storing patterns defining enemies parameters
    public EnemyParams enemies;

// Cash manager
    public ManagementCash managementCash;

// Get the table with enemy patterns
// Short-circuits the array with enemy patterns
    public EnemyParams[] GetEnemyParams()
    {
        EnemyParams[] res = new EnemyParams[1];
        res[0] = enemies;
        return res;
    }

    // Set the enemy's pattern parameters
    // enemy - an object storing the enemy's parameters
    // no    - enemy's pattern number
    // Returns information whether the enemy's pattern parameters have been set successfully
    public bool SetEnemyParams(EnemyParams enemy,int no)
    {
        if (no < 0 || enemies == null)
        {

            return false;
        }
        enemies.SetData(enemy);
        ActualizeParams();

        return true;
    }

// Create a new enemy
// no        - enemy's pattern number
// beginTile - enemy's starting tile
// endTile   - target tile that the enemy should reach
// Returns information whether the enemy was successfully created
    public bool Generate(int no, TileCharacter beginTile, TileCharacter endTile)
    {
        int n = 1;
        if (enemies != null && n > 0 && n > no && beginTile != null && endTile != null)
        {
            GameObject go = enemies.model;
            if (go != null)
            {
                MoveEnemy moveEnemy = go.GetComponent<MoveEnemy>();
                if (moveEnemy != null)
                {
                    if (managementCash != null)
                    {
                        if (managementCash.GetEnemyCoins() < enemies.coins)
                            return false;

                        managementCash.SetEnemyCoins(managementCash.GetEnemyCoins() - enemies.coins);
                    }

                    GameObject newGo = Instantiate(go, beginTile.transform.position + new Vector3(0,moveEnemy.shiftAxisY,0),
                        beginTile.transform.rotation);
                    if (newGo != null)
                    {
                        newGo.SetActive(true);
                        moveEnemy = newGo.GetComponent<MoveEnemy>();
                        if (moveEnemy != null)
                        {
                            moveEnemy.tileBegin = beginTile;
                            moveEnemy.tileEnd = endTile;
                        }

                        return true;
                    }
                }
            }
        }

        return false;

    }

// Update enemies parameters based on patterns (when patterns change)
    public void ActualizeParams()
    {
        if (enemies != null)
        {
            MoveEnemy me;
            Health h;
            EnemyParams item = enemies;
            {
                if (item != null && item.model != null)
                {
                    me = item.model.GetComponent<MoveEnemy>();
                    if (me != null)
                    {
                        me.speed = item.speed;
                        me.tag = item.tag;
                        me.cash = item.coinsToEnd;
                    }
                    h = item.model.GetComponent<Health>();
                    if (h != null)
                    {
                        h.startHealth = item.startHealth;
                        h.armour = item.armour;
                        h.coins = item.destroyCoins;
                        h.enemyTag = item.tag;
                    }
                }
            }
        }
    }

    void Start()
    {
        ActualizeParams();
    }

}
