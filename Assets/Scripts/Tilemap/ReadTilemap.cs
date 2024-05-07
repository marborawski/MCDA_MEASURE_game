using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Xml.Serialization;
using UnityEngine;

// Reading tilemap from file
public class ReadTilemap
{

// Data about tilemap
    public class TilemapData
    {

// Tile name
        public string value;

// Tile type
        public string type;

// Tile orientation
        public string orientation;

// Road type
        public string typeRoad;
    }

// Definition of the data structure of the file containing information about tilemap
    [XmlRoot("Tilemap")]
    public class TilemapRead
    {
        [XmlElement("Table")]
        public TilemapTableRead tilemapTableRead = new TilemapTableRead();

        public TilemapData[,] GetData()
        {
            return tilemapTableRead.GetData();
        }
    }

    public class TilemapTableRead
    {
        [XmlElement("Row")]
        public List<TilemapRowRead> tilemapRowRead = new List<TilemapRowRead>();

        public TilemapData[,] GetData()
        {
            TilemapData[][] data = new TilemapData[tilemapRowRead.Count][];
            TilemapData[,] result = null;
            int i,j,max = -1;
            for (i = 0; i < data.Length; i++)
            {
                data[i] = tilemapRowRead[i].GetData();
                if (max < data[i].Length)
                {
                    max = data[i].Length;
                }
            }
            if (max >= 0)
            {
                result = new TilemapData[data.Length, max];
                for (i = 0; i < data.Length; i++)
                {
                    for (j = 0; j < data[i].Length; j++)
                    {
                        result[i, j] = data[i][j];
                    }
                }
            }

            return result;
        }
    }

    public class TilemapRowRead
    {
        [XmlElement("Cell")]
        public List<TilemapCellRead> tilemapCellRead = new List<TilemapCellRead>();

        public TilemapData[] GetData()
        {
            TilemapData[] result = new TilemapData[tilemapCellRead.Count];
            int i;
            for (i = 0;i < result.Length;i++)
            {
                result[i] = tilemapCellRead[i].GetData();
            }

            return result;
        }
    }

    public class TilemapCellRead
    {
        [XmlElement("Data")]
        public TilemapCellDataRead tilemapCellDataRead = new TilemapCellDataRead();

        public TilemapData GetData()
        {
            return tilemapCellDataRead.GetData();
        }
    }

    public class TilemapCellDataRead
    {
        [XmlAttribute("type")]
        public string type = "Normal";

        [XmlAttribute("orientation")]
        public string orientation = "None";

        [XmlAttribute("typeRoad")]
        public string typeRoad = "None";

        [XmlText]
        public string value = "None";

        public TilemapData GetData()
        {
            TilemapData result = new TilemapData();
            result.value = value;
            result.type = type;
            result.orientation = orientation;
            result.typeRoad = typeRoad;
            return result;
        }
    }

// Compare the contents of two string variables
// str1, str2 - compared variables
// Breaks true if the contents of the variables are identical or the contents of the variable with shorter text are identical to the contents of the initial data of the second variable
    public static bool ComapreEndString(string str1, string str2)
    {
        int i, j;
        if (str1.Length < str2.Length)
            return false;
        for (i = str1.Length - 1, j = str2.Length - 1; i >= 0 && j >= 0; i--, j--)
        {
            if (str1[i] != str2[j])
                return false;
        }

        return true;
    }

// Reading tilemap from file
// filename - file name
// Returns an array with information about the tilemap
    public static TilemapData[,] LoadFromFile(string filename)
    {
        string filePath = PrepareFilePath(filename);
        if (File.Exists(filePath))
        {
            TilemapRead tilemapRead;
            XmlSerializer serializer = new XmlSerializer(typeof(TilemapRead));
            using (Stream stream = new FileStream(filePath, FileMode.Open))
            {
                try
                {
                    tilemapRead = serializer.Deserialize(stream) as TilemapRead;
                }catch(Exception)
                {
                    return null;
                }
            }
            return tilemapRead.GetData();
        }

        return null;
    }

// Reading tilemap from resources
// filename - file name
// Returns an array with information about the tilemap
    public static TilemapData[,] LoadFromResource(string filename)
    {
        TilemapRead tilemapRead;
        TextAsset textInResources = (TextAsset)Resources.Load(filename, typeof(TextAsset));
        XmlSerializer serializer = new XmlSerializer(typeof(TilemapRead));
        MemoryStream memoryStream = new MemoryStream(textInResources.bytes);
        tilemapRead = serializer.Deserialize(memoryStream) as TilemapRead;

        return tilemapRead.GetData();
    }

    private static string PrepareFilePath(string filename)
    {
        string fileName2 = filename;
        if (!ComapreEndString(fileName2, ".xml"))
            fileName2 = fileName2 + ".xml";
        string filePath = Path.Combine(Application.dataPath, Path.Combine("Resources", fileName2));

        return filePath;
    }
}
