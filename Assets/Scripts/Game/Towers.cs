using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyInfo;

// Towers generating module
public class Towers : MonoBehaviour
{
    [System.Serializable]
// Tower information
    public class TowerParams
    {
// 3D model of the tower
        public GameObject model;
// Tower rotation speed
        public float speed = 500;
// Tower rate of fire
        public float rateOfFire = 0.5f;
// Tower bullet power (amount of damage dealt)
        public float bulletStrength = 2;
// The force with which the bullet is fired
        public float force = 1000;
// Cost of building a tower
        public float cost = 30;
// Number of towers that can be placed (-1 means unlimited number of towers)
        public int count = -1;
// Tower enemy's tag
        public string tag = "Enemy";
// Tower enemy type
        public string type = "";
        [HideInInspector]
// Number of currently placed towers
        public int actualGenerateCount = 0;
// Setting the tower pattern parameters
// tower - an object storing the parameters of the tower pattern
        public void SetData(TowerParams tower)
        {
            speed = tower.speed;
            rateOfFire = tower.rateOfFire;
            bulletStrength = tower.bulletStrength;
            force = tower.force;
            cost = tower.cost;
            count = tower.count;
            tag = tower.tag;
            type = tower.type;
        }
    }

// An array storing patterns defining towers parameters
    public TowerParams towers;

// Pointer to the object creating the tilemap
    public CreateTilemap createTilemap;

// Cash manager
    public ManagementCash managementCash;

// Get the table with tower patterns
// Short-circuits the array with tower patterns
    public TowerParams[] GetTowerParams()
    {
        TowerParams[] res = new TowerParams[1];
        res[0] = towers;
        return res;
    }

// Set the tower's pattern parameters
// tower - an object storing the tower's parameters
// no    - tower's pattern number
// Returns information whether the tower's pattern parameters have been set successfully
    public bool SetTowerParams(TowerParams tower, int no)
    {
        if (no < 0 || towers == null || no >= 1)
        {

            return false;
        }
        towers.SetData(tower);
        ActualizeParams();

        return true;
    }

// Update towers parameters based on patterns (when patterns change)
    public void ActualizeParams()
    {
        if (towers != null)
        {
            EnemyAttack ea;
            BulletAtack ba;
            TrackingEnemy te;
            TowerParams item = towers;
            {
                if (item != null && item.model != null)
                {
                    ea = item.model.GetComponent<EnemyAttack>();
                    if (ea != null)
                    {
                        ea.rateOfFire = item.rateOfFire;
                        ea.force = item.force;
                        ea.waveTag = item.tag;
                        if (ea.bullet != null)
                        {
                            ba = ea.bullet.GetComponent<BulletAtack>();
                            if (ba != null)
                            {
                                ba.strength = item.bulletStrength;
                                ba.tagAttackObject = item.tag;
                            }
                        }
                    }
                    te = item.model.GetComponent<TrackingEnemy>();
                    if (ea != null)
                    {
                        te.speed = item.speed;
                        te.enemyTag = item.tag;
                    }
                }
            }
        }
    }

    // Create a new tower
    // no  - tower's pattern number
    // pos - coordinates of the tile on which the tower is placed
    // Returns information whether the tower was successfully created
    public bool Generate(int no, Vector2Int pos)
    {
        int n = 1;
        if (towers != null && n > 0 && n > no && createTilemap != null)
        {
            //            int no = Random.Range(0, n);
            GameObject go = towers.model;
            if (go != null)
            {
                if (managementCash != null)
                {
                    if (managementCash.GetCoins() < towers.cost)
                        return false;

                    managementCash.SetCoins(managementCash.GetCoins() - towers.cost);
                }
                Vector3 v = createTilemap.positionTilemap
                    + new Vector3(pos.x*createTilemap.tileSize.x,0, pos.y * createTilemap.tileSize.y);
                GameObject newGo = Instantiate(go, v, createTilemap.transform.rotation);
                if (newGo != null)
                {
                    newGo.SetActive(true);

                    return true;
                }
            }
        }

        return false;

    }

    void Start()
    {
        ActualizeParams();
    }
}
