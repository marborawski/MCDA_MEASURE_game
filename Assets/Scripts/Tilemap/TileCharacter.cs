using System.Collections;
using System.Collections.Generic;
using CG_Base;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

// The object that the TileCharacter module is attached to is seen as a tile. Manages tower information
public class TileCharacter : MonoBehaviour
{
    [System.Serializable]
// The cost of moving from a tile to an adjacent tile
    public class CostRoad
    {

// The cost of moving forward
        public float front;

// The cost of moving backward
        public float back;

// Cost of moving left
        public float left;

// Cost of moving right
        public float right;

        public CostRoad()
        {
        }

// Data assignment
// f - cost of moving forward
// r - cost of moving to the right
// b - cost of moving backward
// l - cost of moving left
        public CostRoad(float f, float r, float b, float l)
        {
            front = f;
            right = r;
            back = b;
            left = l;
        }
    }

// Functions performed by the tile
    public enum Type
    {
        Begin,
        End,
        Normal,
        None
    }

// Information about enemies killed by a specific tower type
    public class TowerKillEnemies
    {

// Tower type
        public string towerType;

// Enemies killed
        public int kill = 0;

// Total health levels lost by enemies
        public float lossHealth = 0;
    }

// Information about killed enemies
    public class KillEnemies
    {

// Enemy type
        public string enemyType;

// Number of enemies who passed through without being killed
        public int passEnemies = 0;

// Total health levels lost by enemies
        public float endHealth = float.NaN;

// List of tower types with information about killed enemies
        public List<TowerKillEnemies> killEnemies = new List<TowerKillEnemies>();

// Search for information about killed enemies by a given tower type
// towerType - tower type
// Returns information about enemies killed by a specific tower type
        public TowerKillEnemies FindTower(string towerType)
        {
            foreach (TowerKillEnemies item in killEnemies)
            {
                if (item != null && item.towerType == towerType)
                {
                    return item;
                }
            }

            return null;
        }

// Adding information about the enemy killed by the tower
// towerType - tower type
        public void AddKill(string towerType)
        {
            TowerKillEnemies towerKillEnemy = FindTower(towerType);
            if (towerKillEnemy == null)
            {
                towerKillEnemy = new TowerKillEnemies();
                towerKillEnemy.towerType = towerType;
                killEnemies.Add(towerKillEnemy);
            }
            towerKillEnemy.kill++;
        }

// Adding information about the health level lost by the enemy
// towerType - tower type
// value - lost standard of living
        public void AddLoss(string towerType, float value)
        {
            TowerKillEnemies towerKillEnemy = FindTower(towerType);
            if (towerKillEnemy == null)
            {
                towerKillEnemy = new TowerKillEnemies();
                towerKillEnemy.towerType = towerType;
                killEnemies.Add(towerKillEnemy);
            }
            towerKillEnemy.lossHealth += value;
        }

// Retrieving information about killed enemies by a given tower type
// towerType - tower type
// Returns the number of enemies killed by a specific tower type
        public int GetKillEnemies(string towerType)
        {
            foreach (TowerKillEnemies item in killEnemies)
            {
                if (item != null && item.towerType == towerType)
                {
                    return item.kill;
                }
            }

            return 0;
        }

// Downloading information about killed enemies
// Returns the number of enemies killed
        public int GetKillEnemies()
        {
            int s = 0;
            foreach (TowerKillEnemies item in killEnemies)
            {
                if (item != null)
                {
                    s += item.kill;
                }
            }

            return s;
        }

        // Retrieving information about the simultaneous level of health lost as a result of shots fired by a specific type of tower
        // towerType - tower type
        // Returns the simultaneous health loss caused by shots fired by a specific turret type
        public float GetLossHealth(string towerType)
        {
            foreach (TowerKillEnemies item in killEnemies)
            {
                if (item != null && item.towerType == towerType)
                {
                    return item.lossHealth;
                }
            }

            return 0;
        }

        // Get information about the number of enemies who have reached the end point
        // Returns the number of enemies who have reached the endpoint
        public int GetPassEnemies()
        {

            return passEnemies;
        }

// Get information about the total health level of enemies who have reached the end point
// Returns the total health level of enemies who have reached the end point
        public float GetEndHealth()
        {

            return endHealth;
        }

    }

//Tile type
    public string tileType = "None";

// Function performed by the tile
    public Type tileFunction = Type.None;

    // Determine different versions of costs for traversing a tile. Each cost version includes the cost of the dice in four directions
    public CostRoad[] costRoad;

// Tower tags
    public string[] towerTags;

// Enemy tags
    public string[] enemyTags;

// Bullet tags
    public string[] bulletTags;

    private int shotAt = 0;

    private int numberOfBullets = 0;

    private float lossHealth = 0;

    private List<TowerCharacter> shootAtTowers = new List<TowerCharacter>();

    private List<KillEnemies> killEnemies = new List<KillEnemies>();

    private KillEnemies FindEnemy(string enemyType)
    {
        foreach (KillEnemies item in killEnemies)
        {
            if (item != null && item.enemyType == enemyType)
            {
                return item;
            }
        }

        return null;
    }

    private void AddKill(string enemyType,string towerType)
    {
        KillEnemies killEnemy = FindEnemy(enemyType);
        if (killEnemy == null)
        {
            killEnemy = new KillEnemies();
            killEnemy.enemyType = enemyType;
            killEnemies.Add(killEnemy);
        }
        killEnemy.AddKill(towerType);
    }

    private void AddLossHealth(string enemyType, string towerType, float value)
    {
        KillEnemies loss = FindEnemy(enemyType);
        if (loss == null)
        {
            loss = new KillEnemies();
            loss.enemyType = enemyType;
            killEnemies.Add(loss);
        }
        loss.AddLoss(towerType, value);
    }

    private void AddPass(string enemyType)
    {
        KillEnemies killEnemy = FindEnemy(enemyType);
        if (killEnemy == null)
        {
            killEnemy = new KillEnemies();
            killEnemy.enemyType = enemyType;
            killEnemies.Add(killEnemy);
        }
        killEnemy.passEnemies++;
    }

    private void AddEndHealth(string enemyType, float health)
    {
        KillEnemies killEnemy = FindEnemy(enemyType);
        if (killEnemy == null)
        {
            killEnemy = new KillEnemies();
            killEnemy.enemyType = enemyType;
            killEnemies.Add(killEnemy);
        }
        if (float.IsNaN(killEnemy.endHealth))
        {
            killEnemy.endHealth = health;
        }
        else
            killEnemy.endHealth += health;
    }

    private void eventFunction(string type, string param, EventData data, RootClass ob)
    {
        float value;
        switch(type)
        {
            case "DestroyEnemy":
                if (data != null && data is Event4StringData)
                {
                    AddKill((data as Event4StringData).data1, (data as Event4StringData).data2);
                }
                
                break;
            case "ChangeActualTile":
                if (ob != null)
                {
                    AddPass(param);
                    Health h = ob.gameObject.GetComponent<Health>();
                    if (h != null)
                        h.SafeRemoveEvent(eventFunction);
                    MoveEnemy me = ob.gameObject.GetComponent<MoveEnemy>();
                    if (me != null)
                        me.SafeRemoveEvent(eventFunction);
                }
                break;
            case "EnemyInEnd":
                AddPass(param);
                if (data != null && data is EventFloatData)
                {
                    AddEndHealth(param,(data as EventFloatData).data);
                }
                break;
            case "ChangeHealth":
                if (data != null && data is Event4StringData)
                {
                    try
                    {
                        float.TryParse(param, out value);
                    }
                    catch
                    {
                        value = 0;
                    }
                    AddLossHealth((data as Event4StringData).data1, (data as Event4StringData).data2, value);
                    lossHealth += value;
                }
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach(string item in towerTags)
        {
            if (other.tag == item)
            {
                TowerCharacter tower = other.gameObject.GetComponent<TowerCharacter>();
                if (tower != null)
                {
                    shootAtTowers.Add(tower);
                }
                shotAt += 1;
                break;
            }
        }

        foreach (string item in enemyTags)
        {
            if (other.tag == item)
            {
                Health h = other.gameObject.GetComponent<Health>();
                if (h != null)
                {
                    h.runEvent = eventFunction;
                }
                MoveEnemy me = other.gameObject.GetComponent<MoveEnemy>();
                if (me != null)
                {
                    me.SetActualTile(this);
                    me.runEvent = eventFunction;
                }
                break;
            }
        }

        foreach (string item in bulletTags)
        {
            if (other.tag == item)
            {
                numberOfBullets++;
                break;
            }
        }

    }

    // Number of directions in which you cannot move from the tile
    // no - transition cost version
    // Returns the number of directions in which you cannot move from the tile
    public int GetSumNoMoveNeighbors(int no)
    {
        if (costRoad != null && costRoad.Length > no)
        {
            int s = 0;
            if (costRoad[no].left < 0)
                s++;
            if (costRoad[no].right < 0)
                s++;
            if (costRoad[no].back < 0)
                s++;
            if (costRoad[no].front < 0)
                s++;

            return s;
        }

        return -1;
    }

// Determine whether the tile area is within the turret's firing range
// Returns true if the tile is within shooting range or false if the tile is beyond shooting range
    public int GetShotAtTile()
    {

        return shotAt;
    }

// Provides a list of towers for which the tile is within shooting range
// List of towers
    public List<TowerCharacter> GetTowers()
    {

        return shootAtTowers;
    }

// Provides the number of enemies of a specific type killed by the selected tower type
// enemyType - enemy type
// towerType - tower type
// Returns the number of enemies killed
    public int GetKillEnemies(string enemyType, string towerType)
    {
        foreach (KillEnemies item in killEnemies)
        {
            if (item != null && item.enemyType == enemyType)
            {
                return item.GetKillEnemies(towerType);
            }
        }

        return 0;
    }

// Provides the ratio of captured enemies to all enemies (within the tile)
// enemyType - enemy type
// towerType - tower type
// Returns the ratio of captured enemies to all enemies
    public float GetKillPassEnemies(string enemyType, string towerType)
    {
        foreach (KillEnemies item in killEnemies)
        {
            if (item != null && item.enemyType == enemyType)
            {
                int kill = item.GetKillEnemies(towerType);
                int pass = item.GetPassEnemies();
                return (float)kill/(float)(pass + kill);
            }
        }
        return 0;
    }

// Gives the ratio of the life level lost to the number of enemies who managed to pass through the tile
// enemyType - enemy type
// towerType - tower type
// Returns the ratio of health level lost to the number of enemies who managed to pass the tile
    public float GetLossPassHealth(string enemyType, string towerType)
    {

        foreach (KillEnemies item in killEnemies)
        {
            if (item != null && item.enemyType == enemyType)
            {
                float loss = item.GetLossHealth(towerType);
                int pass = item.GetPassEnemies();
                return loss/(float)(pass);
            }
        }

        return 0;
    }

// Provides the total life level of enemies on the tile
// enemyType - enemy type
// towerType - tower type
// Returns the total health lost by enemies on the tile
    public float GetLossHealth(string enemyType, string towerType)
    {

        foreach (KillEnemies item in killEnemies)
        {
            if (item != null && item.enemyType == enemyType)
            {
                return item.GetLossHealth(towerType);
            }
        }

        return 0;
    }

// Provides the total health level of the enemies after passing the tile
// enemyType - enemy type
// Returns the total health level that enemies have left after passing the tile
    public float GetPassHealth(string enemyType)
    {

        foreach (KillEnemies item in killEnemies)
        {
            if (item != null && item.enemyType == enemyType)
            {
                return item.GetPassEnemies();
            }
        }

        return 0;
    }

// Shows the number of enemies who reached the tile (and were killed or moved on)
// enemyType - enemy type
// Returns the number of enemies who reached the tile
    public int GetEnemies(string enemyType)
    {

        foreach (KillEnemies item in killEnemies)
        {
            if (item != null && item.enemyType == enemyType)
            {
                return item.GetPassEnemies() + item.GetKillEnemies();
            }
        }

        return 0;
    }

// Shows the number of enemies who passed through the tile (they were not killed)
// enemyType - enemy type
// Returns the number of enemies who passed through the tile
    public int GetPassEnemies(string enemyType)
    {

        foreach (KillEnemies item in killEnemies)
        {
            if (item != null && item.enemyType == enemyType)
            {
                return item.GetPassEnemies();
            }
        }

        return 0;
    }

// Gives the ratio of the remaining life of enemies (after passing through the tile) to the number of enemies who passed through the tile
// enemyType - enemy type
// Returns the ratio of the remaining life of enemies (after passing through the tile) to the number of enemies who passed through the tile
    public float GetEndMeanHealth(string enemyType)
    {

        foreach (KillEnemies item in killEnemies)
        {
            if (item != null && item.enemyType == enemyType)
            {
                float endHealth = item.GetEndHealth();
                int pass = item.GetPassEnemies();

                return endHealth / (float)pass;
            }
        }

        return 0;
    }

// Updating information about the costs of moving in four directions after rotating a tile
    public void ActualizeRotation()
    {
        if (costRoad != null)
        {
            Vector3 rot = transform.rotation.eulerAngles;
            CostRoad cr;
            int i;

            for(i = 0;i < costRoad.Length;i++)
            {
                if (rot.y >= 45 && rot.y < 135)
                {
                    cr = new CostRoad();
                    cr.right = costRoad[i].front;
                    cr.back = costRoad[i].right;
                    cr.left = costRoad[i].back;
                    cr.front = costRoad[i].left;
                    costRoad[i] = cr;
                    continue;
                }
                if (rot.y >= 135 && rot.y < 225)
                {
                    cr = new CostRoad();
                    cr.right = costRoad[i].left;
                    cr.back = costRoad[i].front;
                    cr.left = costRoad[i].right;
                    cr.front = costRoad[i].back;
                    costRoad[i] = cr;
                    continue;
                }
                if (rot.y >= 225 && rot.y < 315)
                {
                    cr = new CostRoad();
                    cr.right = costRoad[i].back;
                    cr.back = costRoad[i].left;
                    cr.left = costRoad[i].front;
                    cr.front = costRoad[i].right;
                    costRoad[i] = cr;
                    continue;
                }
            }

        }
    }

// The number of bullets that landed on the tile
// Returns the number of bullets that landed on the tile
    public int GetNuberOfBullets()
    {
        return numberOfBullets;
    }

    // total life lost per tile
    // Returns total life lost per tile
    public float GetLossHelath()
    {
        return lossHealth;
    }

}
