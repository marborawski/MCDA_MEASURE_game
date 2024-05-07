using System.Collections;
using System.Collections.Generic;
using CG_Base;
using UnityEngine;
using UnityEngine.Events;
using static CostMap;

// Module that moves the enemy
public class MoveEnemy : CollectionEvents
{

// Module determining the cost of the distance traveled by the object
    public ManagementCost managementCost;

// Cash manager
    public ManagementCash managementCash;

// Definition of the types of tiles on which different types of enemies can move (each could move on a different type of tile)
    public enum TileType { water };

// Determine what type of tile the enemy can move on
    public TileType tileType = TileType.water;

// The tile from which the enemy starts his route
    public TileCharacter tileBegin;

// The tile on which the enemy should end his route
    public TileCharacter tileEnd;

// Enemy's speed
    public float speed = 0.1f;

// Move the enemy in the y axis relative to the tile over which it is created
    public float shiftAxisY = 0;

// Methods called when the enemy reaches the tile where he was supposed to end his journey
    public UnityEvent EndMove;

// Methods called when the enemy is created
    public UnityEvent generate;

    private Vector3 moveVector = new Vector3(float.NaN, float.NaN, float.NaN);

    private CostMap costMap;

    private CostRoad actual;

    private CostRoad next;

    private bool start = true;

    private List<CostRoad> path;

    private int pathPosition;

    private bool runEndMove = false;

    private TileCharacter actualTile;

    private EnemyCharacter enemyCharacter;

    private Health health;

    [HideInInspector]
// Cash transferred to the enemy manager when the enemy reaches the end of the path
    public float cash;

// Setting the tile the enemy is on
// tile - the tile on which the enemy is located
    public void SetActualTile(TileCharacter tile)
    {
        string txt = "";
        if (enemyCharacter != null)
            txt = enemyCharacter.type;
        RunEvent("ChangeActualTile", txt, null);
        actualTile = tile;
    }

    private void Move()
    {
        if (!float.IsNaN(moveVector.x))
        {
            Vector3 m2 = next.GetPosition() - transform.position;
            m2.y = 0;
            float a = Vector3.Dot(moveVector, m2);
            if (a >= 0)
            {
                transform.position += moveVector * speed * Time.deltaTime;
            }
            else
            {
                next.GetPosition();
                Vector3 v = next.GetPosition();
                v.y = 0;
                pathPosition++;
                if (pathPosition < path.Count)
                {
                    actual = next;
                    next = path[pathPosition];
                    moveVector = next.GetPosition() - actual.GetPosition();
                    moveVector = moveVector.normalized;
                    moveVector.y = 0;
                }
                else
                {
                    if (!runEndMove)
                    {
                        if (managementCash != null)
                        {
                            managementCash.AddEnemyCoins(cash);
                        }
                        EndMove.Invoke();
                        runEndMove = true;
                        string txt = "";
                        if (enemyCharacter != null)
                            txt = enemyCharacter.type;
                        EventFloatData eventFloatData = null;
                        if (health != null)
                        {
                            eventFloatData = new EventFloatData(health.GetHealth());
                        }
                        RunEvent("EnemyInEnd",txt, eventFloatData);
                        gameObject.SetActive(false);

                        Destroy(gameObject, 3);
                    }
                }
            }
        }
    }

    private void Init()
    {
        if (managementCost != null)
        {
            if (tileType == TileType.water)
            {
                costMap = managementCost.GetCostMap(tileBegin);
                if (costMap != null)
                {
                    actual = costMap.GetBeginTile();
                    if (tileEnd == null)
                    {
                        path = costMap.GetPathRandomEnd();
                    }
                    else
                    {
                        Vector2Int pos = managementCost.GetPositionTile(tileEnd);
                        path = costMap.GetPath(pos.x, pos.y);
                    }
                    if (actual != null && path != null && path.Count >= 2)
                    {
                        next = path[1];
                        pathPosition = 1;
                        moveVector = next.GetPosition() - actual.GetPosition();
                        moveVector = moveVector.normalized;
                    }
                }
            }
        }
    }

    protected override void Start()
    {
        base.Start();
        enemyCharacter = GetComponent<EnemyCharacter>();
        health = GetComponent<Health>();
        generate.Invoke();
    }

    protected override void Update()
    {
        base.Update();
        if (start)
        {
            start = false;
            Init();
        }
        else
        {
            Move();
        }
    }

}
