using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static CostMap;
using static CostMap.CostRoad;

// Determining the cost of the distance traveled by the object
public class ManagementCost : MonoBehaviour
{
    private CostMap[] costMap;

    private TileCharacter[] tileBegin;

    private TileCharacter[] tileEnd;

// An array containing objects with tile models
    public TileCharacter[,] tilemap;

    private bool startTileMap = false;

// Calculate the sum of tiles of specific types that are neighbors of the path
// begin    - the starting tile of the path
// end      - end tile of the path
// no       - data version number with costs of transitions between paths
// tileType - tile types
// Returns the number of tile neighbors of the path with the specified type
    public int GetSumNeigboursTileTypePath(TileCharacter begin, TileCharacter end, int no, string[] tileType)
    {
        List<CostRoad> path = GetPath(begin, end);
        List<CostRoad> neigboursPath = GetNeigboursPath(begin, path);
        TileCharacter tileCharacter;

        if (path != null && tileType != null && tileType.Length > 0)
        {
            int s = 0;
            foreach (CostRoad item in neigboursPath)
            {
                if (item != null)
                {
                    tileCharacter = item.GetTileCharacter();
                    if (tileCharacter != null)
                    {
                        foreach(string item2 in tileType)
                        {
                            if (item2 != null)
                            {
                                if (item2 == tileCharacter.tileType)
                                {
                                    s++;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return s;
        }

        return 0;
    }

    // Calculate the number of bullets that entered the area of the tiles belonging to the path
    // begin - beginning of the path
    // end   - end of path
    // Returns the number of bullets
    public int GetShotAtTiles(TileCharacter begin, TileCharacter end)
    {

        List<CostRoad> path = GetPath(begin, end);
        TileCharacter tileCharacter;

        if (path != null)
        {
            int s = 0;
            foreach (CostRoad item in path)
            {
                if (item != null)
                {
                    tileCharacter = item.GetTileCharacter();
                    if (tileCharacter != null && tileCharacter.GetShotAtTile())
                    {
                        s++;
                    }
                }
            }

            return s;
        }

        return 0;
    }

// Designate a list of unique tiles
// data - list of tiles
// Returns a list of unique tiles
    public List<TowerCharacter> ListToUnique(List<TowerCharacter> data)
    {
        if (data != null)
        {
            List<TowerCharacter> res = new List<TowerCharacter>();
            foreach (TowerCharacter item in data)
            {
                if (item != null)
                {
                    if (res.IndexOf(item) < 0)
                    {
                        res.Add(item);
                    }
                }
            }

            return res;
        }

        return null;
    }

// Determine the list of tiles belonging to the path
// begin - the beginning of the path
// end   - the end of the path
// Returns the list of tiles belonging to the path
    public List<TowerCharacter> GetTowers(TileCharacter begin, TileCharacter end)
    {
        List<CostRoad> path = GetPath(begin, end);
        TileCharacter tileCharacter;
        List<TowerCharacter> tmp,towers = new List<TowerCharacter>();

        if (path != null)
        {
            foreach (CostRoad item in path)
            {
                if (item != null)
                {
                    tileCharacter = item.GetTileCharacter();
                    tmp = tileCharacter.GetTowers();
                    if (tileCharacter != null && tmp != null)
                    {
                        foreach (TowerCharacter item2 in tmp)
                        {
                            if (item2 != null)
                            {
                                towers.Add(item2);
                            }
                        }
                    }
                }
            }

            return ListToUnique(towers);
        }

        return null;
    }

// Determine the ratio of enemies killed by a given tower type to all enemies
// begin     - beginning of the path
// end       - end of path
// enemyType - enemy type
// towerType - tower type
// Returns the ratio of enemies killed by a given tower type to all enemies
    public float GetKillPassEnemies(TileCharacter begin, TileCharacter end, string enemyType, string towerType)
    {

        List<CostRoad> path = GetPath(begin, end);
        TileCharacter tileCharacter;

        if (path != null)
        {
            float sKill = 0,sPass = 0;
            foreach (CostRoad item in path)
            {
                if (item != null)
                {
                    tileCharacter = item.GetTileCharacter();
                    if (tileCharacter != null)
                    {
                        sKill += tileCharacter.GetKillEnemies(enemyType, towerType);
                        sPass += tileCharacter.GetPassEnemies(enemyType);
                    }
                }
            }

            return sKill/(sPass + sKill);
        }

        return 0;
    }

// Determining the ratio of life lost to the number of enemies that crossed the path
// begin     - beginning of the path
// end       - end of path
// enemyType - enemy type
// towerType - tower type
// Returns the ratio of life lost to the number of enemies that passed through the path
    public float GetLossPathHealth(TileCharacter begin, TileCharacter end, string enemyType, string towerType)
    {

        List<CostRoad> path = GetPath(begin, end);
        TileCharacter tileCharacter;

        if (path != null)
        {
            float sLoss = 0;
            float sPass = 0;
            foreach (CostRoad item in path)
            {
                if (item != null)
                {
                    tileCharacter = item.GetTileCharacter();
                    if (tileCharacter != null)
                    {
                        sLoss += tileCharacter.GetLossHealth(enemyType, towerType);
                        sPass += tileCharacter.GetPassHealth(enemyType);
                    }
                }
            }

            return sLoss/ sPass;
        }

        return 0;
    }

// Determine the position of the tiles in the tile array
// tile - tile
// Returns the x,y coordinates of the tile's position in the tile array
    public Vector2Int GetPositionTile(TileCharacter tile)
    {
        Vector2Int res = new Vector2Int(-1,-1);
        if (tile != null && tilemap != null)
        {
            int i, j;
            for(i = 0;i < tilemap.GetLength(0);i++)
            {
                for (j = 0; j < tilemap.GetLength(1); j++)
                {
                    if (tilemap[i,j] == tile)
                    {

                        return new Vector2Int(i,j);
                    }
                }
            }
        }

        return res;
    }

// Get the number of starting tiles
// Returns the number of starting tiles
    public int GetTileBeginCount()
    {
        if (tileBegin != null)
        {

            return tileBegin.Length;
        }

        return 0;
    }

// Get the table with the starting tiles
// Returns an array of starting tiles
    public TileCharacter[] GetTileBegin()
    {
        return tileBegin;
    }

// Get the end tile array
// Returns an array of end tiles
    public TileCharacter[] GetTileEnd()
    {
        return tileEnd;
    }

// Get the number of end tiles
// Returns the number of end tiles
    public int GetTileEndCount()
    {
        if (tileEnd != null)
        {

            return tileEnd.Length;
        }

        return 0;
    }

// Change tilemap
// tilemap_ - new tilemap board
    public void ChangeTilemap(TileCharacter[,] tilemap_)
    {
        startTileMap = true;
        tilemap = tilemap_;

        if (tilemap != null)
        {
            tileBegin = FindTiles(TileCharacter.Type.Begin);
            if (tileBegin != null && tileBegin.Length > 0)
            {
                tileEnd = FindTiles(TileCharacter.Type.End);
                if (tileEnd != null && tileEnd.Length > 0)
                {
                    costMap = new CostMap[tileBegin.Length];
                    int i;
                    for (i = 0; i < tileBegin.Length; i++)
                    {
                        costMap[i] = new CostMap(tileBegin[i], tileEnd, tilemap, 0);
                    }
                }

            }
        }
    }

// Transferring information about the enemy hitting the tile
// tile - tile
    public void AddHit(TileCharacter tile)
    {
        if (tile != null)
        {
            if (tile.gameObject.TryGetComponent<Health>(out var h))
            {
                Vector2Int pos = GetPositionTile(tile);
                if (costMap != null)
                {
                    foreach(CostMap item in costMap)
                    {
                        if (item != null)
                        {
                            item.AddSubHit((int)pos.x,(int)pos.y, true, true);
                        }
                    }
                }
            }
        }
    }

// Providing information about the enemy's failure to hit the tile
// tile - tile
    public void AddNoHit(TileCharacter tile)
    {
        if (tile != null)
        {
            Vector2Int pos = GetPositionTile(tile);
            if (costMap != null && pos.x >= 0)
            {
                foreach (CostMap item in costMap)
                {
                    if (item != null)
                    {
                        item.AddSubHit((int)pos.x, (int)pos.y, false, true);
                    }
                }
            }            
        }
    }

// Passing information about the enemies miss to all tiles
// tile - tile
    public void AddNoHits(TileCharacter tile)
    {
        if (tile != null)
        {
            if (costMap != null)
            {
                foreach (CostMap item in costMap)
                {
                    if (item != null)
                    {
                        item.AddSubHit(false, true);
                    }
                }
            }
        }
    }

    private int CountTiles(TileCharacter.Type tileFunction)
    {
        int n = 0;
        foreach (TileCharacter item in tilemap)
        {
            if (item != null && item.tileFunction == tileFunction)
            {
                n++;
            }
        }

        return n;
    }


    private TileCharacter[] FindTiles(TileCharacter.Type tileFunction)
    {
        TileCharacter[] tile;
        int n = CountTiles(tileFunction);
        if (n <= 0)
        {

            return null;
        }
        tile = new TileCharacter[n];
        int i = 0;
        foreach (TileCharacter item in tilemap)
        {
            if (item != null && item.tileFunction == tileFunction)
            {
                tile[i] = item;
                i++;
            }
        }

        return tile;
    }

// Get the cost of traversing the path
// begin - beginning of the path
// end   - end of the path
// Returns the cost of traversing the path
    public float GetCost(TileCharacter begin, TileCharacter end)
    {
        CostMap map = GetCostMap(begin);
        if (map != null)
        {
            Vector2Int pos = GetPositionTile(end);

            return map.CalculateCost(pos.x,pos.y);
        }

        return float.NaN;
    }

// Get the list of path neighbors
// begin - beginning of the path
// path  - list of tiles belonging to the path
// Returns a list of tile neighbors of the path
    public List<CostRoad> GetNeigboursPath(TileCharacter begin, List<CostRoad> path)
    {
        CostMap map = GetCostMap(begin);
        if (map != null)
        {

            return map.GetNeigboursPath(path);
        }

        return null;
    }

// Get the list of tiles belonging to the path
// begin - beginning of the path
// end - end of the path
// Returns a list of tiles belonging to the path
    public List<CostRoad> GetPath(TileCharacter begin, TileCharacter end)
    {
        CostMap map = GetCostMap(begin);
        if (map != null)
        {
            Vector2Int pos = GetPositionTile(end);

            return map.GetPath(pos.x, pos.y);
        }

        return null;
    }

// Gets an object specifying the route the moving object should follow
// tileBeginCharacter - the starting tile of the path
// Returns an object specifying the route the moving object should follow
    public CostMap GetCostMap(TileCharacter tileBeginCharacter)
    {
        int i;
        if (tileBegin != null)
        {
            for (i = 0; i < tileBegin.Length; i++)
            {
                if (tileBegin[i] == tileBeginCharacter && costMap != null && costMap.Length > i)
                {
                    return costMap[i];
                }
            }
        }
        return null;
    }

// Determines the risk of traversing the path
// begin - beginning of the path
// end   - end of the path
// Returns the risk of traversing the path
    public float GetRisk(TileCharacter begin, TileCharacter end)
    {
        CostMap map = GetCostMap(begin);
        if (map != null)
        {
            Vector2Int pos = GetPositionTile(end);

            return map.GetRiskPath(pos.x, pos.y);
        }

        return float.NaN;
    }

// Determine the statistics of hits and misses in an object for a given path
// begin - the beginning of the path
// end   - the end of the path
// Returns the number of hits and misses in the object
    public HitEnemyInfo GetHits(TileCharacter begin, TileCharacter end)
    {
        CostMap map = GetCostMap(begin);
        if (map != null)
        {
            Vector2Int pos = GetPositionTile(end);

            return map.GetHits(pos.x, pos.y);
        }

        return null;
    }

    void Start()
    {
        if (tilemap != null)
        {
            if (!startTileMap)
            {
                ChangeTilemap(tilemap);
            }
        }
    }

}
