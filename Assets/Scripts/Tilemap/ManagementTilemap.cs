using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static CostMap;
using static ReadTilemap;
using static UnityServer;

// Tilemap management
public class ManagementTilemap : MonoBehaviour
{

// The name of the file from which tilemap can be read
    public string filename;

    private TilemapData[,] tilemapTable;

    private TerrainModels[] terrainModels;

    private CreateTilemap createTilemap;

    private TileCharacter[,] tilemap;

// An object specifying the cost of the distance traveled by the object
    public ManagementCost managementCost;

// Methods called after creating the tilemap
    public UnityEvent createTilemapEvent;

// Transfer tilemap data from the server and create a tilemap
// data - data sent to the server and tilemap to be created
    public void ServerReceiveData(ReceiveData data)
    {
        if (data != null && data.tilemapRead != null)
        {
            DeleteTilemap();
            CreateTilemap(data.tilemapRead);
        }
    }

    private void DeleteTilemap()
    {
        if (tilemap != null)
        {
            int i, j;
            for (i = 0; i < tilemap.GetLength(0);i ++)
            {
                for (j = 0; j < tilemap.GetLength(1); j++)
                {
                    Destroy(tilemap[i,j].gameObject);
                }

            }
        }
    }

    private void CreateTilemap(TilemapRead data)
    {
        tilemapTable = data.GetData();
        terrainModels = GetComponentsInChildren<TerrainModels>();
        createTilemap = GetComponent<CreateTilemap>();
        if (createTilemap != null)
        {
            tilemap = createTilemap.Create(tilemapTable, terrainModels);
            if (managementCost != null)
            {
                managementCost.ChangeTilemap(tilemap);
            }
            createTilemapEvent.Invoke();
        }
    }

// Getting sizes in number of tilemap tiles
    public Vector2Int GetSize()
    {
        Vector2Int res = new Vector2Int();

        if (tilemap != null)
        {
            res.x = tilemap.GetLength(0);
            res.y = tilemap.GetLength(1);
        }

        return res;
    }

    void Start()
    {
        if (managementCost == null)
        {
            managementCost = GetComponent<ManagementCost>();
        }
        if (filename != "")
        {
#if UNITY_EDITOR
            tilemapTable = ReadTilemap.LoadFromFile(filename);
#else
            tilemapTable = ReadTilemap.LoadFromResource(filename);
#endif
            terrainModels = GetComponentsInChildren<TerrainModels>();
            createTilemap = GetComponent<CreateTilemap>();
            if (createTilemap != null)
            {
                tilemap = createTilemap.Create(tilemapTable, terrainModels);
                if (managementCost != null)
                {
                    managementCost.ChangeTilemap(tilemap);
                }
            }
        }
    }
}
