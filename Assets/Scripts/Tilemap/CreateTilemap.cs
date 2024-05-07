using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static ReadTilemap;
using static TileCharacter;

// Create tilemap
public class CreateTilemap : MonoBehaviour
{
// Types of tiles on which an object can move
    public enum TypeRoad
    {
        Terrain,
        Intersection4,
        Intersection3,
        Curve,
        Straight,
        BeginEnd,
        Lonely,
        None
    }

// Tile orientation
    public enum Orientation
    {
        North,
        South,
        East,
        West,
        None
    }

// Tilemap coordinates
    public Vector3 positionTilemap;

// Tilemap sizes in number of tiles in x-axis and y-axis
    public Vector2 tileSize;

// Parent object for tilemap
    public GameObject parent;

// Road type
    public string roadName = "Water";

// Create a tilemap
// tilemaps - information about tiles
// maps     - array of objects containing models
// Starts the tilemap
    public TileCharacter[,] Create(TilemapData[,] tilemaps, TerrainModels[] models)
    {
        if (tilemaps != null && models != null)
        {
            int i, j;
            TerrainModels model;
            TileCharacter[,] res = new TileCharacter[tilemaps.GetLength(0), tilemaps.GetLength(1)];
            for (i = 0; i < tilemaps.GetLength(0); i++)
            {
                for (j = 0; j < tilemaps.GetLength(1); j++)
                {
                    if (tilemaps[i, j] != null)
                    {
                        model = FindModels(models, tilemaps[i, j].value);
                        res[i,j] = CloneModel(tilemaps, model, new Vector2Int(i, j));
                    }
                }
            }
            return res;
        }

        return null;
    }

    private TerrainModels FindModels(TerrainModels[] models, string name)
    {
        if (models != null)
        {
            foreach (TerrainModels model in models)
            {
                if (model != null && model.nameModels == name)
                {
                    return model;
                }
            }
        }

        return null;
    }

    private TileCharacter CloneModel(TilemapData[,] tilemaps, TerrainModels model, Vector2Int pos)
    {
        if (model != null && tilemaps != null)
        {
            TilemapData data = tilemaps[pos.x, pos.y];
            if (data != null)
            {
                TileCharacter tileCharacter = null;

                TypeRoad typeRoad = StringToTypeRoad(data.typeRoad);
                if (typeRoad == TypeRoad.None)
                {
                    typeRoad = GetTypeRoad(tilemaps, pos);
                }
                Orientation orientation = StringToOrientation(data.orientation, typeRoad);
                if (orientation == Orientation.None)
                {
                    orientation = GetOrientation(tilemaps, pos, typeRoad);
                }
                TileCharacter.Type type = StringToType(data.type);
                TileCharacter.CostRoad costRoad;

                switch (typeRoad)
                {
                    case TypeRoad.Terrain:
                    case TypeRoad.Lonely:
                        tileCharacter = CloneTile(model.normal, model.normal, pos, orientation);
                        AddInformationToTileCharacter(tileCharacter, model.nameModels, type,
                            new TileCharacter.CostRoad(-1, -1, -1, -1));
                        break;
                    case TypeRoad.Intersection4:
                        if (data.value == roadName)
                            costRoad = new TileCharacter.CostRoad(1, 1, 1, 1);
                        else
                            costRoad = new TileCharacter.CostRoad(-1, -1, -1, -1);
                        tileCharacter = CloneTile(model.intersection4Road, model.normal, pos, orientation);
                        AddInformationToTileCharacter(tileCharacter, model.nameModels, type, costRoad);
                        break;
                    case TypeRoad.Intersection3:
                        if (data.value == roadName)
                            costRoad = new TileCharacter.CostRoad(1, -1, 1, 1);
                        else
                            costRoad = new TileCharacter.CostRoad(-1, -1, -1, -1);
                        tileCharacter = CloneTile(model.intersection3Road, model.normal, pos, orientation);
                        AddInformationToTileCharacter(tileCharacter, model.nameModels, type, costRoad);
                        break;
                    case TypeRoad.Straight:
                        switch (type)
                        {
                            case Type.Begin:
                                tileCharacter = CloneTile(model.straightBegin, model.normal, pos, orientation);
                                break;
                            case Type.End:
                                tileCharacter = CloneTile(model.straightEnd, model.normal, pos, orientation);
                                break;
                            default:
                                tileCharacter = CloneTile(model.straight, model.normal, pos, orientation);
                                break;
                        }
                        if (data.value == roadName)
                            costRoad = new TileCharacter.CostRoad(-1, 1, -1, 1);
                        else
                            costRoad = new TileCharacter.CostRoad(-1, -1, -1, -1);
                        AddInformationToTileCharacter(tileCharacter, model.nameModels, type, costRoad);
                        break;
                    case TypeRoad.Curve:
                        if (data.value == roadName)
                            costRoad = new TileCharacter.CostRoad(1, -1, -1, 1);
                        else
                            costRoad = new TileCharacter.CostRoad(-1, -1, -1, -1);
                        tileCharacter = CloneTile(model.curve, model.normal, pos, orientation);
                        AddInformationToTileCharacter(tileCharacter, model.nameModels, type, costRoad);
                        break;
                    case TypeRoad.BeginEnd:
                        switch (type)
                        {
                            case Type.Begin:
                                tileCharacter = CloneTile(model.begin, model.normal, pos, orientation);
                                break;
                            case Type.End:
                                tileCharacter = CloneTile(model.end, model.normal, pos, orientation);
                                break;
                            default:
                                tileCharacter = CloneTile(model.beginEnd, model.normal, pos, orientation);
                                break;
                        }
                        if (data.value == roadName)
                            costRoad = new TileCharacter.CostRoad(-1, -1, -1, 1);
                        else
                            costRoad = new TileCharacter.CostRoad(-1, -1, -1, -1);
                        AddInformationToTileCharacter(tileCharacter, model.nameModels, type, costRoad);
                        break;
                }
                return tileCharacter;
            }
        }

        return null;
    }

    private void AddInformationToTileCharacter(TileCharacter tileCharacter, string tileType,
        TileCharacter.Type type, TileCharacter.CostRoad costRoad)
    {
        if (tileCharacter != null)
        {
            tileCharacter.tileType = tileType;
            tileCharacter.tileFunction = type;
            if (tileCharacter.costRoad != null)
            {
                tileCharacter.costRoad[0] = costRoad;
                tileCharacter.ActualizeRotation();
            }
        }
    }

    private TileCharacter CloneTile(GameObject go, GameObject substitution, Vector2Int pos, Orientation orientation)
    {
        GameObject g = go;
        if (g == null)
        {
            g = substitution;
        }
        if (g != null)
        {
            GameObject ob = Instantiate(g);
            if (ob != null)
            {
                TileCharacter tc = ob.GetComponent<TileCharacter>();
                if (parent != null)
                {
                    ob.transform.parent = parent.transform;
                }
                if (tc != null)
                {
                    tc.costRoad = new TileCharacter.CostRoad[1];
                    Quaternion rotation;
                    ob.transform.position = positionTilemap + new Vector3(pos.x * tileSize.x, 0, pos.y * tileSize.y);
                    ob.SetActive(true);
                    switch (orientation)
                    {
                        case Orientation.East:
                            rotation = Quaternion.Euler(0, -90, 0) * ob.transform.rotation;
                            ob.transform.rotation = rotation;
                            break;
                        case Orientation.South:
                            rotation = Quaternion.Euler(0, -180, 0) * ob.transform.rotation;
                            ob.transform.rotation = rotation;
                            break;
                        case Orientation.West:
                            rotation = Quaternion.Euler(0, -270, 0) * ob.transform.rotation;
                            ob.transform.rotation = rotation;
                            break;
                    }

                    return tc;
                }
            }

        }

        return null;
    }

    private Orientation GetOrientation(TilemapData[,] tilemaps, Vector2Int pos, TypeRoad typeRoad)
    {
        if (tilemaps != null)
        {
            switch (typeRoad)
            {
                case TypeRoad.Terrain:
                case TypeRoad.Intersection4:
                case TypeRoad.Lonely:
                    return Orientation.North;
                case TypeRoad.Intersection3:
                    return GetIntersection3Orientaiton(tilemaps, pos);
                case TypeRoad.Curve:
                    return GetCurveOrientaiton(tilemaps, pos);
                case TypeRoad.Straight:
                    return GetStraightOrientaiton(tilemaps, pos);
                case TypeRoad.BeginEnd:
                    return GetBeginEndOrientaiton(tilemaps, pos);
                default:
                    return Orientation.None;
            }
        }

        return Orientation.None;
    }

    private Orientation GetIntersection3Orientaiton(TilemapData[,] tilemaps, Vector2Int pos)
    {
        if (tilemaps != null)
        {
            string name = tilemaps[pos.x, pos.y].value;
            if (pos.x > 0)
            {
                if (tilemaps[pos.x - 1, pos.y].value != name)
                {

                    return Orientation.West;
                }
            }
            if (pos.y > 0)
            {
                if (tilemaps[pos.x, pos.y - 1].value != name)
                {

                    return Orientation.North;
                }
            }
            if (pos.x + 1 < tilemaps.GetLength(0))
            {
                if (tilemaps[pos.x + 1, pos.y].value != name)
                {

                    return Orientation.East;
                }
            }
            if (pos.y + 1 < tilemaps.GetLength(1))
            {
                if (tilemaps[pos.x, pos.y + 1].value != name)
                {

                    return Orientation.South;
                }
            }
        }

        return Orientation.None;
    }

    private Orientation GetCurveOrientaiton(TilemapData[,] tilemaps, Vector2Int pos)
    {
        if (tilemaps != null)
        {
            string name = tilemaps[pos.x, pos.y].value;
            if (pos.x + 1 < tilemaps.GetLength(0) && pos.y > 0)
            {
                if (tilemaps[pos.x, pos.y - 1].value == name && tilemaps[pos.x + 1, pos.y].value == name)
                {

                    return Orientation.South;
                }
            }
            if (pos.x + 1 < tilemaps.GetLength(0) && pos.y + 1 < tilemaps.GetLength(1))
            {
                if (tilemaps[pos.x + 1, pos.y].value == name && tilemaps[pos.x, pos.y + 1].value == name)
                {

                    return Orientation.West;
                }
            }
            if (pos.x > 0 && pos.y + 1 < tilemaps.GetLength(1))
            {
                if (tilemaps[pos.x, pos.y + 1].value == name && tilemaps[pos.x - 1, pos.y].value == name)
                {

                    return Orientation.North;
                }
            }
            if (pos.x > 0 && pos.y > 0)
            {
                if (tilemaps[pos.x - 1, pos.y].value == name && tilemaps[pos.x, pos.y - 1].value == name)
                {

                    return Orientation.East;
                }
            }
        }

        return Orientation.None;
    }

    private Orientation GetStraightOrientaiton(TilemapData[,] tilemaps, Vector2Int pos)
    {
        if (tilemaps != null)
        {
            string name = tilemaps[pos.x, pos.y].value;
            if (pos.x > 0 && pos.x + 1 < tilemaps.GetLength(0))
            {
                if (tilemaps[pos.x - 1, pos.y].value == name && tilemaps[pos.x + 1, pos.y].value == name)
                {

                    return Orientation.North;
                }
            }
            if (pos.y > 0 && pos.y + 1 < tilemaps.GetLength(1))
            {
                if (tilemaps[pos.x, pos.y - 1].value == name && tilemaps[pos.x, pos.y + 1].value == name)
                {

                    return Orientation.West;
                }
            }
            if (pos.x > 0 && pos.x + 1 < tilemaps.GetLength(0))
            {
                if (tilemaps[pos.x - 1, pos.y].value != name && tilemaps[pos.x + 1, pos.y].value != name)
                {

                    return Orientation.West;
                }
            }
            if (pos.y > 0 && pos.y + 1 < tilemaps.GetLength(1))
            {
                if (tilemaps[pos.x, pos.y - 1].value != name && tilemaps[pos.x, pos.y + 1].value != name)
                {

                    return Orientation.North;
                }
            }
        }

        return Orientation.None;
    }

    private Orientation GetBeginEndOrientaiton(TilemapData[,] tilemaps, Vector2Int pos)
    {
        if (tilemaps != null)
        {
            string name = tilemaps[pos.x, pos.y].value;
            if (pos.x > 0)
            {
                if (tilemaps[pos.x - 1, pos.y].value == name)
                {

                    return Orientation.North;
                }
            }
            if (pos.y > 0)
            {
                if (tilemaps[pos.x, pos.y - 1].value == name)
                {

                    return Orientation.East;
                }
            }
            if (pos.x + 1 < tilemaps.GetLength(0))
            {
                if (tilemaps[pos.x + 1, pos.y].value == name)
                {

                    return Orientation.South;
                }
            }
            if (pos.y + 1 < tilemaps.GetLength(1))
            {
                if (tilemaps[pos.x, pos.y + 1].value == name)
                {

                    return Orientation.West;
                }
            }
        }

        return Orientation.None;
    }

    private int CountNeighbors(TilemapData[,] tilemaps, Vector2Int pos)
    {
        int n = 0;
        if (tilemaps != null)
        {
            string name = tilemaps[pos.x, pos.y].value;
            if (pos.x <= 0 && pos.y <= 0)
            {
                n++;
            } else
            {
                if (pos.x <= 0 || pos.y <= 0)
                {
                    n++;
                } else
                {
                    if (tilemaps[pos.x - 1, pos.y - 1].value == name)
                    {
                        n++;
                    }
                }
            }
            if (pos.y <= 0)
            {
                n++;
            } else
            {
                if (tilemaps[pos.x, pos.y - 1].value == name)
                {
                    n++;
                }
            }
            if (pos.x + 1 >= tilemaps.GetLength(0) && pos.y <= 0)
            {
                n++;
            }
            else
            {
                if (pos.x + 1 >= tilemaps.GetLength(0) || pos.y <= 0)
                {
                    n++;
                }
                else
                {
                    if (tilemaps[pos.x + 1, pos.y - 1].value == name)
                    {
                        n++;
                    }
                }
            }
            if (pos.x + 1 >= tilemaps.GetLength(0))
            {
                n++;
            }
            else
            {
                if (tilemaps[pos.x + 1, pos.y].value == name)
                {
                    n++;
                }
            }
            if (pos.x + 1 >= tilemaps.GetLength(0) && pos.y + 1 >= tilemaps.GetLength(1))
            {
                n++;
            }
            else
            {
                if (pos.x + 1 >= tilemaps.GetLength(0) || pos.y + 1 >= tilemaps.GetLength(1))
                {
                    n++;
                }
                else
                {
                    if (tilemaps[pos.x + 1, pos.y + 1].value == name)
                    {
                        n++;
                    }
                }
            }
            if (pos.y + 1 >= tilemaps.GetLength(1))
            {
                n++;
            }
            else
            {
                if (tilemaps[pos.x, pos.y + 1].value == name)
                {
                    n++;
                }
            }
            if (pos.x <= 0 && pos.y + 1 >= tilemaps.GetLength(1))
            {
                n++;
            }
            else
            {
                if (pos.x <= 0 || pos.y + 1 >= tilemaps.GetLength(1))
                {
                    n++;
                }
                else
                {
                    if (tilemaps[pos.x - 1, pos.y + 1].value == name)
                    {
                        n++;
                    }
                }
            }
            if (pos.x <= 0)
            {
                n++;
            }
            else
            {
                if (tilemaps[pos.x - 1, pos.y].value == name)
                {
                    n++;
                }
            }
        }

        return n;
    }

    private int CountNearNeighbors(TilemapData[,] tilemaps, Vector2Int pos)
    {
        int n = 0;
        if (tilemaps != null)
        {
            string name = tilemaps[pos.x, pos.y].value;
            if (pos.y <= 0)
            {
                n++;
            }
            else
            {
                if (tilemaps[pos.x, pos.y - 1].value == name)
                {
                    n++;
                }
            }
            if (pos.x + 1 >= tilemaps.GetLength(0))
            {
                n++;
            }
            else
            {
                if (tilemaps[pos.x + 1, pos.y].value == name)
                {
                    n++;
                }
            }
            if (pos.y + 1 >= tilemaps.GetLength(1))
            {
                n++;
            }
            else
            {
                if (tilemaps[pos.x, pos.y + 1].value == name)
                {
                    n++;
                }
            }
            if (pos.x <= 0)
            {
                n++;
            }
            else
            {
                if (tilemaps[pos.x - 1, pos.y].value == name)
                {
                    n++;
                }
            }
        }

        return n;
    }

    private bool StraightNeighbors(TilemapData[,] tilemaps, Vector2Int pos)
    {
        if (tilemaps != null)
        {
            bool YN = false;
            string name = tilemaps[pos.x, pos.y].value;
            if (pos.y <= 0)
            {
                YN = true;
            }
            else
            {
                if (tilemaps[pos.x, pos.y - 1].value == name)
                {
                    YN = true;
                }
            }
            if (YN)
            {
                if (pos.y + 1 >= tilemaps.GetLength(1))
                {

                    return true;
                }
                else
                {
                    if (tilemaps[pos.x, pos.y + 1].value == name)
                    {

                        return true;
                    }
                }

                return false;
            }
            if (pos.x + 1 >= tilemaps.GetLength(0))
            {
                YN = true;
            }
            else
            {
                if (tilemaps[pos.x + 1, pos.y].value == name)
                {
                    YN = true;
                }
            }
            if (!YN)
            {

                return false;
            }
            if (pos.x <= 0)
            {

                return true;
            }
            else
            {
                if (tilemaps[pos.x - 1, pos.y].value == name)
                {

                    return true;
                }
            }
        }

        return false;
    }


    private TypeRoad StringToTypeRoad(string name)
    {
        switch (name)
        {
            case "Terrain":

                return TypeRoad.Terrain;
            default:

                return TypeRoad.None;
        }
    }

    private TypeRoad GetTypeRoad(TilemapData[,] tilemaps, Vector2Int pos)
    {
        if (tilemaps != null)
        {
            int n;
            n = CountNearNeighbors(tilemaps, pos);
            switch (n)
            {
                case 4:

                    return TypeRoad.Intersection4;
                case 3:

                    return TypeRoad.Intersection3;
                case 2:
                    if (StraightNeighbors(tilemaps, pos))
                    {

                        return TypeRoad.Straight;
                    }

                    return TypeRoad.Curve;
                case 1:

                    return TypeRoad.BeginEnd;
                case 0:

                    return TypeRoad.Lonely;
            }
        }

        return TypeRoad.None;
    }

    private bool FindLetter(string text, char letter)
    {
        foreach (char l in text)
        {
            if (l == letter)
            {

                return true;
            }
        }

        return false;
    }

    private char FourthLetter(string orientation)
    {
        string letters = "NSEW";
        foreach (char l in orientation)
        {
            if (!FindLetter(letters, l))
            {

                return l;
            }
        }

        return '?';
    }

    private Orientation StringToOrientation(string orientation, TypeRoad typeRoad)
    {
        switch (typeRoad)
        {
            case TypeRoad.Terrain:
            case TypeRoad.Intersection4:
            case TypeRoad.Lonely:
                return Orientation.North;
            case TypeRoad.Intersection3:
                char letter = FourthLetter(orientation);
                switch (letter)
                {
                    case 'E':
                        return Orientation.North;
                    case 'N':
                        return Orientation.West;
                    case 'W':
                        return Orientation.South;
                    case 'S':
                        return Orientation.East;
                    default:
                        return Orientation.None;
                }
            case TypeRoad.Curve:
                switch (orientation)
                {
                    case "NE":
                    case "EN":
                        return Orientation.North;
                    case "ES":
                    case "SE":
                        return Orientation.East;
                    case "SW":
                    case "WS":
                        return Orientation.South;
                    case "WN":
                    case "NW":
                        return Orientation.West;
                    default:
                        return Orientation.None;
                }
            case TypeRoad.Straight:
                switch (orientation)
                {
                    case "NS":
                    case "SN":
                        return Orientation.North;
                    case "WE":
                    case "EW":
                        return Orientation.South;
                    default:
                        return Orientation.None;
                }
            case TypeRoad.BeginEnd:
                switch (orientation)
                {
                    case "N":
                        return Orientation.North;
                    case "W":
                        return Orientation.West;
                    case "S":
                        return Orientation.South;
                    case "E":
                        return Orientation.East;
                    default:
                        return Orientation.None;
                }
            default:
                return Orientation.None;
        }

    }

    private Type StringToType(string type)
    {
        switch (type)
        {
            case "Begin":
                return Type.Begin;
            case "End":
                return Type.End;
            default:
                return Type.Normal;
        }

    }
}
