using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using static CostMap.CostRoad;

// Determine the route the object is to follow
public class CostMap
{

// A tile intended to determine the cost of moving through one tile
    public class CostRoad
    {
        private static int count = 0;

// Unique identifier of the object specifying the cost of traversing the tile
        public int id = 0;

// Cost of moving forward through a tile
        public float frontCost;

// Cost of moving backwards through a tile
        public float backCost;

// Cost of moving left through a tile
        public float leftCost;

// Cost of moving right through a tile
        public float rightCost;

// Cost of traversing the tile for the currently determined best path
        public float cost = float.NaN;

// The front neighbor of the tile
        public CostRoad frontNeighbor;

// The back neighbor of the tile
        public CostRoad backNeighbor;

// The left neighbor of the tile
        public CostRoad leftNeighbor;

// The right neighbor of the tile
        public CostRoad rightNeighbor;

// The tile from which the opponent entered this tile
        public CostRoad lastTile;

// Is the start tile?
        public bool beginTile = false;

// Is the end tile?
        public bool endTile = false;

        private Vector3 position;

        private TileCharacter tileCharacter;

// Risk of walking through a tile
        public float risk = 0;

// Information about hit and miss enemies
        public class HitEnemyInfo
        {
// Number of enemies hit
            public int nHit = 0;
// Number of missed enemies
            public int nNoHit = 0;
        }

// Information about hit and miss opponents
        public HitEnemyInfo hitEnemyInfo = new HitEnemyInfo();

// Assign information to calculate the cost of traversing a tile
// f              - cost of moving forward through the tile
// b              - cost of moving backwards through the tile
// l              - cost of moving left through the tile
// r              - cost of moving right through the tile
// _position      - coordinates of the tile position
// _tileCharacter - pointer to the object containing the tile model
        public CostRoad(float f, float b, float l, float r, Vector3 _position, TileCharacter _tileCharacter)
        {
            frontCost = f;
            backCost = b;
            leftCost = l;
            rightCost = r;
            position = _position;
            tileCharacter = _tileCharacter;
            count++;
            id = count;
        }

// Get a pointer to the object containing the tile model
// Returns a pointer to the object containing the tile model
        public TileCharacter GetTileCharacter()
        {

            return tileCharacter;
        }

// Get the coordinates of the tile location
// Returns the coordinates of the tile's location
        public Vector3 GetPosition()
        {
            return position;
        }

// Calculate the cost of traversing the tile from the starting point to the current tile
// Returns the total cost of walking from the starting point to the current tile
        public float CalculateCost()
        {
            if (beginTile)
            {

                return 0;
            }
            if (lastTile == null)
            {

                return float.NaN;
            }

            return lastTile.CalculateCost() + cost;
        }

// Designate a list of tiles to jump to from the current tile
// Returns an array containing a list of tiles
        public CostRoad[] Next()
        {
            CostRoad[] tmp = new CostRoad[4];
            int n = 0;
            if (frontNeighbor != null && frontNeighbor.lastTile == this)
            {
                tmp[0] = frontNeighbor;
                n++;
            }
            if (backNeighbor != null && backNeighbor.lastTile == this)
            {
                tmp[1] = backNeighbor;
                n++;
            }
            if (leftNeighbor != null && leftNeighbor.lastTile == this)
            {
                tmp[2] = leftNeighbor;
                n++;
            }
            if (rightNeighbor != null && rightNeighbor.lastTile == this)
            {
                tmp[3] = rightNeighbor;
                n++;
            }
            if (n > 0)
            {
                int i, j;
                CostRoad[] result = new CostRoad[n];
                for (i = 0, j = 0; i < 4; i++)
                {
                    if (tmp[i] != null)
                    {
                        result[j] = tmp[i];
                        j++;
                    }
                }

                return result;
            }

            return null;
        }

// Determine the path from the starting tile to the ending one. The method is called recursively, saving the path by changing the lastTile argument
// last      - previous tile
// beginTile - starting tile
        public void CalculatePath(CostRoad last, CostRoad beginTile)
        {
            float cost1, cost2;
            lastTile = last;
            if (frontNeighbor != null && frontCost >= 0 && beginTile != frontNeighbor)
            {
                if (!float.IsNaN(frontNeighbor.cost))
                {
                    cost1 = CalculateCost();
                    cost2 = frontNeighbor.CalculateCost();
                    if (cost1 < cost2)
                    {
                        frontNeighbor.CalculatePath(this, beginTile);
                    }
                }
                if (float.IsNaN(frontNeighbor.cost))
                {
                    frontNeighbor.cost = frontCost;
                    frontNeighbor.CalculatePath(this, beginTile);
                }
            }
            if (backNeighbor != null && backCost >= 0 && beginTile != backNeighbor)
            {
                if (!float.IsNaN(backNeighbor.cost))
                {
                    cost1 = CalculateCost();
                    cost2 = backNeighbor.CalculateCost();
                    if (cost1 < cost2)
                    {
                        backNeighbor.CalculatePath(this, beginTile);
                    }
                }
                if (float.IsNaN(backNeighbor.cost))
                {
                    backNeighbor.cost = backCost;
                    backNeighbor.CalculatePath(this, beginTile);
                }
            }
            if (leftNeighbor != null && leftCost >= 0 && beginTile != leftNeighbor)
            {
                if (!float.IsNaN(leftNeighbor.cost))
                {
                    cost1 = CalculateCost();
                    cost2 = leftNeighbor.CalculateCost();
                    if (cost1 < cost2)
                    {
                        leftNeighbor.CalculatePath(this, beginTile);
                    }
                }
                if (float.IsNaN(leftNeighbor.cost))
                {
                    leftNeighbor.cost = leftCost;
                    leftNeighbor.CalculatePath(this, beginTile);
                }
            }
            if (rightNeighbor != null && rightCost >= 0 && beginTile != rightNeighbor)
            {
                if (!float.IsNaN(rightNeighbor.cost))
                {
                    cost1 = CalculateCost();
                    cost2 = rightNeighbor.CalculateCost();
                    if (cost1 < cost2)
                    {
                        rightNeighbor.CalculatePath(this, beginTile);
                    }
                }
                if (float.IsNaN(rightNeighbor.cost))
                {
                    rightNeighbor.cost = rightCost;
                    rightNeighbor.CalculatePath(this, beginTile);
                }
            }
        }

// Adding information about whether an intersect is hit (or not) in the tile area
// hit - false is a miss and true is a hit
// add - false is to decrease and true is to increase the number of hits/misses
        public void AddSubHit(bool hit, bool add)
        {
            if (hit)
            {
                if (add)
                {
                    hitEnemyInfo.nHit++;
                }else
                {
                    hitEnemyInfo.nHit--;
                }
            }else
            {
                if (add)
                {
                    hitEnemyInfo.nNoHit++;
                }
                else
                {
                    hitEnemyInfo.nNoHit--;
                }
            }
        }
    }

    private CostRoad beginTile;

    private CostRoad[,] costRoad;

    private CostRoad[] endTiles;

// Gets the start tile of the path
// Returns the start tile
    public CostRoad GetBeginTile()
    {
        return beginTile;
    }

    private Vector2 FindPositionTile(TileCharacter _beginTileCharacter, TileCharacter[,] tileCharacters)
    {
        if (tileCharacters != null && tileCharacters.GetLength(0) > 0 &&
            tileCharacters.GetLength(1) > 0)
        {
            int n = tileCharacters.GetLength(0);
            int m = tileCharacters.GetLength(1);
            int i, j;
            for (i = 0; i < n; i++)
            {
                for (j = 0; j < m; j++)
                {
                    if (_beginTileCharacter == tileCharacters[i,j])
                    {

                        return new Vector2(i, j);
                    }
                }
            }
        }

        return new Vector2(float.NaN, float.NaN);
    }

    private Vector2 FindPositionCostRoad(CostRoad road)
    {
        if (road != null && costRoad != null && costRoad.GetLength(0) > 0 &&
            costRoad.GetLength(1) > 0)
        {
            int n = costRoad.GetLength(0);
            int m = costRoad.GetLength(1);
            int i, j;
            for (i = 0; i < n; i++)
            {
                for (j = 0; j < m; j++)
                {
                    if (road == costRoad[i, j])
                    {

                        return new Vector2(i, j);
                    }
                }
            }
        }

        return new Vector2(float.NaN, float.NaN);
    }

    private void SetBeginTile(TileCharacter _beginTileCharacter, int no, TileCharacter[,] tileCharacters)
    {
        if (_beginTileCharacter != null && no >= 0)
        {
            if (_beginTileCharacter.costRoad.Length > no)
            {
                Vector2 pos = FindPositionTile(_beginTileCharacter,tileCharacters);
                if (!float.IsNaN(pos.x) && !float.IsNaN(pos.x))
                {
                    beginTile = costRoad[(int)pos.x, (int)pos.y];
                    beginTile.beginTile = true;
                }
            }
        }
    }

    private void SetEndtile(TileCharacter[] _endTileCharacter, int no, TileCharacter[,] tileCharacters)
    {
        if (_endTileCharacter != null && _endTileCharacter.Length > 0 && no > 0)
        {
            int i = 0;
            endTiles = new CostRoad[_endTileCharacter.Length];
            foreach (TileCharacter item in _endTileCharacter)
            {
                Vector2 pos = FindPositionTile(_endTileCharacter[i], tileCharacters);
                if (!float.IsNaN(pos.x) && !float.IsNaN(pos.x))
                {
                    endTiles[i] = costRoad[(int)pos.x, (int)pos.y];
                    endTiles[i].endTile = true;
                }
                i++;
            }
        }
    }

// Creates tiles that will later allow you to mark a path
// _beginTileCharacter - starting tile
// _endTileCharacter   - list of end tiles
// tileCharacters      - an array containing objects with tile models
// no                  - data version number with costs of transitions between paths
    public CostMap(TileCharacter _beginTileCharacter,
        TileCharacter[] _endTileCharacter, TileCharacter[,] tileCharacters, int no)
    {

        if (_beginTileCharacter != null && tileCharacters != null && tileCharacters.GetLength(0) > 0 &&
            tileCharacters.GetLength(1) > 0)
        {
            int n = tileCharacters.GetLength(0);
            int m = tileCharacters.GetLength(1);
            int i, j;
            newMap(n, m);
            for(i = 0;i < n;i++)
            {
                for (j = 0; j < m; j++)
                {
                    AddElement(new Vector2(i,j),
                        tileCharacters[i, j].costRoad[no].front, tileCharacters[i, j].costRoad[no].back,
                        tileCharacters[i, j].costRoad[no].left, tileCharacters[i, j].costRoad[no].right,
                        tileCharacters[i, j].transform.position, tileCharacters[i, j]);
                }
            }
            SetBeginTile(_beginTileCharacter, no, tileCharacters);
            SetEndtile(_endTileCharacter, no, tileCharacters);
            SetNeighbors();
            CalculatePath(beginTile);
        }
    }

    private void newMap(int width, int heigt)
    {
        costRoad = new CostRoad[width,heigt];
    }

    private void AddElement(Vector2 xy, float front, float back, float left, float right, Vector3 position, TileCharacter _tileCharacter)
    {
        if (costRoad != null)
        {
            if (xy.x >= 0 && xy.x < costRoad.GetLength(0) && !float.IsNaN(xy.x) &&
                xy.y >= 0 && xy.y < costRoad.GetLength(1) && !float.IsNaN(xy.y))
            {
                costRoad[(int)xy.x, (int)xy.y] = new CostRoad(front, back, left, right, position, _tileCharacter);
            }

        }
    }

    private void SetNeighbors()
    {
        if (costRoad != null)
        {
            int i, j;
            for (i = 0;i < costRoad.GetLength(0);i++)
            {
                for (j = 0; j < costRoad.GetLength(1); j++)
                {
                    if (costRoad[i, j] != null)
                    {
                        if (j >= 1)
                        {
                            costRoad[i, j].backNeighbor = costRoad[i, j - 1];
                        }
                        if (j < costRoad.GetLength(1) - 1)
                        {
                            costRoad[i, j].frontNeighbor = costRoad[i, j + 1];
                        }
                        if (i >= 1)
                        {
                            costRoad[i, j].leftNeighbor = costRoad[i - 1, j];
                        }
                        if (i < costRoad.GetLength(0) - 1)
                        {
                            costRoad[i, j].rightNeighbor = costRoad[i + 1, j];
                        }
                    }
                }
            }
        }
    }

// Calculate the cost of moving to tiles with x y coordinates in the tile array
// x,y - x and y coordinates of a tile in the tile array
// Returns the cost of traversing the path
    public float CalculateCost(int x, int y)
    {
        if (costRoad != null)
        {
            if (costRoad.GetLength(0) > x && x >= 0)
            {
                if (costRoad.GetLength(1) > y && y >= 0)
                {
                    return costRoad[x, y].CalculateCost();
                }
            }
        }

        return float.NaN;
    }

// Determine the least cost path from the start tile to one of the end tiles
// _beginTile - starting tile
    public void CalculatePath(CostRoad _beginTile)
    {
        if (beginTile != null)
        {
            beginTile.CalculatePath(null, _beginTile);
        }
    }

// Get the path to the end tile
// endTile - end tile
// Returns a list of tiles that make up a path
    public List<CostRoad> GetPath(CostRoad endTile)
    {
        if (endTile != null)
        {
            CostRoad actual;
            List<CostRoad> costRoads = new List<CostRoad>();
            actual = endTile;
            while(actual != null)
            {
                costRoads.Insert(0, actual);
                actual = actual.lastTile;
            }

            return costRoads;
        }

        return null;
    }

    private List<CostRoad> RemovePathFromList(List<CostRoad> data, List<CostRoad> path)
    {
        if (path != null)
        {
            foreach(CostRoad item in path)
            {
                if (item != null)
                {
                    data.Remove(item);
                }
            }    
        }

        return data;
    }

    private List<CostRoad> ListRemoveType(List<CostRoad> data,string type)
    {
        if (data != null)
        {
            List<CostRoad> res = new List<CostRoad>();
            foreach (CostRoad item in data)
            {
                if (item != null)
                {
                    TileCharacter tc = item.GetTileCharacter();
                    if (tc != null && tc.tileType != type)
                    {
                        res.Add(item);
                    }
                }
            }

            return res;
        }

        return data;
    }

    private List<CostRoad> ListToUnique(List<CostRoad> data)
    {
        if (data != null)
        {
            List<CostRoad> res = new List<CostRoad>();
            foreach (CostRoad item in data)
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

        return data;
    }

// List of all tiles adjacent to the path
// path - list of tiles belonging to the path
// Returns a list of all tiles adjacent to the path
    public List<CostRoad> GetNeigboursPath(List<CostRoad> path)
    {
        if (path != null)
        {
            List<CostRoad> res = new List<CostRoad>();
            Vector2 pos;

            foreach(CostRoad item in path)
            {
                if (item != null)
                {
                    pos = FindPositionCostRoad(item);
                    if (!float.IsNaN(pos.x))
                    {
                        if (pos.x > 0 && pos.y > 0)
                            res.Add(costRoad[(int)pos.x - 1, (int)pos.y - 1]);
                        if (pos.x > 0)
                            res.Add(costRoad[(int)pos.x - 1, (int)pos.y]);
                        if (pos.x > 0  && pos.y + 1 < costRoad.GetLength(1))
                            res.Add(costRoad[(int)pos.x - 1, (int)pos.y + 1]);
                        if (pos.y + 1 < costRoad.GetLength(1))
                            res.Add(costRoad[(int)pos.x, (int)pos.y + 1]);
                        if (pos.x + 1 < costRoad.GetLength(0) && pos.y + 1 < costRoad.GetLength(1))
                            res.Add(costRoad[(int)pos.x + 1, (int)pos.y + 1]);
                        if (pos.x + 1 < costRoad.GetLength(0))
                            res.Add(costRoad[(int)pos.x + 1, (int)pos.y]);
                        if (pos.x + 1 < costRoad.GetLength(0) && pos.y > 0)
                            res.Add(costRoad[(int)pos.x + 1, (int)pos.y - 1]);
                        if (pos.y > 0)
                            res.Add(costRoad[(int)pos.x, (int)pos.y - 1]);
                    }
                }
            }
            res = RemovePathFromList(res,path);
            if (path[0] != null)
            {
                TileCharacter tc = path[0].GetTileCharacter();
                if (tc != null)
                {
                    res = ListRemoveType(res,tc.tileType);
                }
            }
            res = ListToUnique(res);

            return res;
        }

        return null;
    }

// Gets the path to the tile at position x,y in the tile array
// x,y - x and y coordinates of the tile in the tile array
// Returns the list of tiles belonging to the path
    public List<CostRoad> GetPath(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < costRoad.GetLength(0) && y < costRoad.GetLength(1))
        {
            if (costRoad[x,y] != null)
            {

                return GetPath(costRoad[x, y]);
            }
        }

        return null;
    }

// Statistics of hits and misses against the enemy on the path
// endTile - the tile where the path ends
// Returns the total number of hits and misses against the enemy on the path
    public HitEnemyInfo GetHits(CostRoad endTile)
    {
        HitEnemyInfo hit = new HitEnemyInfo();
        List<CostRoad> l = GetPath(endTile);
        if (l != null)
        {
            foreach (CostRoad item in l)
            {
                if (item != null)
                {
                    hit.nHit += item.hitEnemyInfo.nHit;
                    hit.nNoHit += item.hitEnemyInfo.nNoHit;
                }
            }
        }

        return hit;
    }

// Risk of crossing the path
// endTile - the tile where the path ends
// Returns the risk of traversing the path
    public float GetRiskPath(CostRoad endTile)
    {
        List<CostRoad> l = GetPath(endTile);
        float risk = 1, tmp, tmp2;
        if (l != null)
        {
            foreach(CostRoad item in l)
            {
                if (item != null)
                {
                    tmp2 = item.hitEnemyInfo.nNoHit + item.hitEnemyInfo.nHit;
                    if (tmp2 == 0)
                    {
                        tmp = 1;
                    }else
                    {
                        tmp = 1 - item.hitEnemyInfo.nHit / (item.hitEnemyInfo.nNoHit + item.hitEnemyInfo.nHit);
                    }
                    risk = risk * tmp;
                }
            }
        }

        return 1 - risk;
    }

// Risk of crossing the path
// x,y - x and y coordinates of the tile in the tile array
// Returns the risk of traversing the path
    public float GetRiskPath(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < costRoad.GetLength(0) && y < costRoad.GetLength(1))
        {
            if (costRoad[x, y] != null)
            {

                return GetRiskPath(costRoad[x, y]);
            }
        }

        return float.NaN;
    }

// Statistics of hits and misses against the enemy on the path
// x,y - x and y coordinates of the tile in the tile array
// Returns the total number of hits and misses against the enemy on the path
    public HitEnemyInfo GetHits(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < costRoad.GetLength(0) && y < costRoad.GetLength(1))
        {
            if (costRoad[x, y] != null)
            {

                return GetHits(costRoad[x, y]);
            }
        }

        return null;
    }


// Determine the path to a random tile
// Returns the list of tiles that make up the path
    public List<CostRoad> GetPathRandomEnd()
    {
        if (endTiles != null && endTiles.Length > 0)
        {
            int no = Random.Range(0, endTiles.Length);

            return GetPath(endTiles[no]);
        }

        return null;
    }

// Adding information about hitting (or not hitting) an enemy in the tile area
// x,y - x and y coordinates of the tile in the tile array
// hit - false is a miss and true is a hit
// add - false is to decrease and true is to increase the number of hits/misses
    public void AddSubHit(int x, int y, bool hit, bool add)
    {
        if (costRoad != null &&costRoad.GetLength(0) > x && x >= 0)
        {
            if (costRoad.GetLength(1) > y && y >= 0)
            {
                if (costRoad[x, y] != null)
                {
                    costRoad[x, y].AddSubHit(hit, add);
                }
            }
        }
    }

// Adding information about hitting (or not hitting) an enemy in the tile area. It is performed for all tiles
// hit - false is a miss and true is a hit
// add - false is to decrease and true is to increase the number of hits/misses
    public void AddSubHit(bool hit, bool add)
    {
        if (costRoad != null)
        {
            int i, j;
            for (i = 0; i< costRoad.GetLength(0); i++)
            {
                for (j = 0; j < costRoad.GetLength(1); j++)
                {
                    costRoad[i,j].AddSubHit(hit, add);
                }
            }
        }
    }
}
