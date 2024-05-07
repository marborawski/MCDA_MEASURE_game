using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static CostMap;
using static CostMap.CostRoad;
using static EnemyInfo;
using static ReadTilemap;
using static TileCharacter;
using static Towers;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityServer;

[System.Serializable]
// Used to create a list of methods to be run when data is received
public class UnityEventServerReceiveData : UnityEvent<ReceiveData>
{
}

[System.Serializable]
// Used to create a list of methods launched when a command appears in the received data
public class UnityEventCommand : UnityEvent<CommandClass>
{
}

// Component that performs server tasks. It waits for commands sent in the form of xml and executes them
public class UnityServer : MonoBehaviour
{

// Module determining the cost of the distance traveled by the object
    public ManagementCost managementCost;

// Cash manager
    public ManagementCash managementCash;

// Pointer to an object containing information about enemy patterns
    public EnemyInfo enemyInfo;

// Pointer to an object containing information about tower patterns
    public Towers towers;

// Address on which the server is listening
    public string iPAdress = "127.0.0.1";

// Port on which the server is listening
    public int port = 55001;

    [HideInInspector]
// Array storing the towers
    public TowerCharacter[] towerCharacters;
    [HideInInspector]
// Array storing the enemies
    public EnemyCharacter[] enemyCharacters;

    [SerializeField]
// List of methods launched when data is received
    private UnityEventServerReceiveData receiveData;

    [SerializeField]
// List of methods launched when a command is detected in the data
    private UnityEventCommand command;

    private TcpListener listener;

    private ReceiveData actualReceiveData;

// Error codes when receiving and sending data
    public enum ErrorCode
    {
        Unknown,
        None
    }

// Definition of the structure of data sent in the form of XML to the server and the server's response
    [XmlRoot("Data")]
    public class ReceiveData
    {
        [XmlElement("Tilemap")]
        public TilemapRead tilemapRead;
        [XmlElement("Command")]
        public CommandClass command;
    }

    public class CommandClass
    {
        [XmlAttribute("name")]
        public string name = "";

        [XmlElement("StartEnemy")]
        public CommandEnemy startEnemy;

        [XmlElement("SetEnemies")]
        public CommandSetEnemies setEnemies;

        [XmlElement("SetTowers")]
        public CommandSetTowers setTowers;

        [XmlElement("AddTower")]
        public CommandAddTower addTower;
    }

    public class CommandEnemy
    {
        [XmlAttribute("no")]
        public int no;
        [XmlElement("Begin")]
        public TileInfo tileBegin = new TileInfo();
        [XmlElement("End")]
        public TileInfo tileEnd = new TileInfo();
    }

    public class CommandAddTower
    {
        [XmlAttribute("no")]
        public int no;
        [XmlAttribute("x")]
        public float x;
        [XmlAttribute("y")]
        public float y;
    }

    public class CommandSetEnemies
    {
        [XmlElement("Enemy")]
        public List<EnemyInfoSerialize> enemies;
    }

    public class CommandSetTowers
    {
        [XmlElement("Tower")]
        public List<TowerSerialize> towers;
    }

    [XmlRoot("Answer")]
    public class AnswerData
    {
        [XmlAttribute("title")]
        public string title;

        [XmlElement("ChoiceOfPath")]
        public ChoiceOfPath choiceOfPath;

        [XmlElement("LevelPath")]
        public ChoiceOfPath levelPath;
    }

    public class ChoiceOfPath
    {
        [XmlAttribute("beginTileCount")]
        public int beginTileCount;
        [XmlAttribute("endTileCount")]
        public int endTileCount;
        [XmlElement("Path")]
        public List<ChoiceOfPathPath> choiceOfPathPaths = new List<ChoiceOfPathPath>();
        [XmlElement("Enemy")]
        public List<EnemyInfoSerialize> enemies;
        [XmlElement("Tower")]
        public List<TowerSerialize> towers;
        [XmlElement("Waves")]
        public WavesSerialize waves = new WavesSerialize();
        [XmlElement("Towers")]
        public TowersSerialize towersSerialize = new TowersSerialize();
    }

    public class TowerKillEnemiesSerialize
    {
        [XmlAttribute("type")]
        public string type;
        [XmlAttribute("killPassEnemies")]
        public float killPassEnemies = 0;
        [XmlAttribute("lossHealthPassEnemies")]
        public float lossHealthPassEnemies = 0;
    }

    public class KillEnemies {
        [XmlAttribute("type")]
        public string type;
        [XmlElement("Tower")]
        public List<TowerKillEnemiesSerialize> towers = new List<TowerKillEnemiesSerialize>();
    }

    public class ChoiceOfPathPath
    {
        [XmlAttribute("cost")]
        public float cost = float.NaN;
        [XmlAttribute("shotAtTiles")]
        public int shotAtTiles = 0;
        [XmlAttribute("towers")]
        public int towers = 0;
        [XmlAttribute("sumTowerPlace")]
        public int sumTowerPlace = 0;
        [XmlAttribute("hits")]
        public int hits = -1;
        [XmlAttribute("nohits")]
        public int noHits = -1;
        [XmlElement("Begin")]
        public TileInfo tileBegin = new TileInfo();
        [XmlElement("End")]
        public TileInfo tileEnd = new TileInfo();
        [XmlElement("Table")]
        public ChoiceOfPathTablePath table = new ChoiceOfPathTablePath();
        [XmlElement("KillEnemy")]
        public List<KillEnemies> killEnemies = new List<KillEnemies>();
    }

    public class EnemyInfoSerialize
    {
        [XmlAttribute("count")]
        public int count;
        [XmlAttribute("speed")]
        public float speed;
        [XmlAttribute("startHealth")]
        public float startHealth;
        [XmlAttribute("armour")]
        public float armour;
        [XmlAttribute("destroyCoins")]
        public float destroyCoins;
        [XmlAttribute("cost")]
        public float cost;
        [XmlAttribute("coinsToEnd")]
        public float coinsToEnd;
        [XmlAttribute("no")]
        public int no;
        [XmlAttribute("type")]
        public string type;
        [XmlAttribute("tag")]
        public string tag;
        public EnemyInfoSerialize()
        {
        }
        public EnemyInfoSerialize(EnemyParams enemy)
        {
            if (enemy != null)
            {
                count = enemy.count;
                speed = enemy.speed;
                startHealth = enemy.startHealth;
                armour = enemy.armour;
                destroyCoins = enemy.destroyCoins;
                cost = enemy.coins;
                type = enemy.type;
                tag = enemy.tag;
                coinsToEnd = enemy.coinsToEnd;
            }
        }
        public EnemyParams GetEnemyParams()
        {
            EnemyParams enemy = new EnemyParams();
            enemy.count = count;
            enemy.speed = speed;
            enemy.startHealth = startHealth;
            enemy.armour = armour;
            enemy.destroyCoins = destroyCoins;
            enemy.coins = cost;
            enemy.tag = tag;
            enemy.type = type;
            enemy.coinsToEnd = coinsToEnd;

            return enemy;
        }
    }

    public class TowerSerialize
    {
        [XmlAttribute("count")]
        public int count;
        [XmlAttribute("speed")]
        public float speed;
        [XmlAttribute("rateofFire")]
        public float rateOfFire;
        [XmlAttribute("force")]
        public float force;
        [XmlAttribute("bulletStrength")]
        public float bulletStrength;
        [XmlAttribute("cost")]
        public float cost;
        [XmlAttribute("no")]
        public int no;
        [XmlAttribute("type")]
        public string type;
        [XmlAttribute("tag")]
        public string tag;
        public TowerSerialize()
        {
        }
        public TowerSerialize(TowerParams enemy)
        {
            if (enemy != null)
            {
                count = enemy.count;
                speed = enemy.speed;
                rateOfFire = enemy.rateOfFire;
                force = enemy.force;
                bulletStrength = enemy.bulletStrength;
                cost = enemy.cost;
                tag = enemy.tag;
                type = enemy.type;
            }
        }
        public TowerParams GetTowerParams()
        {
            TowerParams enemy = new TowerParams();
            enemy.count = count;
            enemy.speed = speed;
            enemy.rateOfFire = rateOfFire;
            enemy.force = force;
            enemy.bulletStrength = bulletStrength;
            enemy.cost = cost;
            enemy.tag = tag;
            enemy.type = type;

            return enemy;
        }
    }

    public class WavesSerialize
    {
        [XmlAttribute("cash")]
        public float cash = 0;
    }

    public class TowersSerialize
    {
        [XmlAttribute("cash")]
        public float cash = 0;
    }

    public class TileInfoEnemies
    {
        [XmlAttribute("type")]
        public string type = "";
        [XmlAttribute("enemies")]
        public int enemies = 0;
        [XmlAttribute("endMeanHealth")]
        public float endMeanHealth = float.NaN;
    }

    public class TileInfo
    {
        [XmlAttribute("x")]
        public int x;
        [XmlAttribute("y")]
        public int y;
        [XmlAttribute("no")]
        public int no;
        [XmlElement("Enemy")]
        public List<TileInfoEnemies> enemy = new List<TileInfoEnemies>();
    }

    public class ChoiceOfPathTablePath
    {
        [XmlElement("Element")]
        public List<ChoiceOfPathTableElement> elements = new List<ChoiceOfPathTableElement>();
    }

    public class ChoiceOfPathTableElement
    {
        [XmlAttribute("x")]
        public float x;
        [XmlAttribute("y")]
        public float y;
        [XmlAttribute("z")]
        public float z;
    }

// Replacing data in text form sent to the server into data in the form of sets of objects in which the parent object is an instance of the ReceiveData class
// data - data in text form in xml format
// Returns a set of related objects mapping the xml structure
    public ReceiveData GetData(String data)
    {
        ReceiveData receiveData;
        XmlSerializer serializer = new XmlSerializer(typeof(ReceiveData));
        byte[] test = System.Text.Encoding.UTF8.GetBytes(data);
        MemoryStream memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(data));
        receiveData = serializer.Deserialize(memoryStream) as ReceiveData;

        return receiveData;
    }

// Converting the server's response in the form of a set of related objects into text data in XML format
// data - a set of interconnected objects mapping the xml structure
// Returns text data in xml format
    public string AnswerToXMLString(AnswerData data)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(AnswerData));
        using (StringWriter textWriter = new StringWriter())
        {
            xmlSerializer.Serialize(textWriter, data);

            return textWriter.ToString();
        }
    }

    private ChoiceOfPathTablePath AnswerCreatePathElements(TileCharacter begin, TileCharacter end)
    {
        ChoiceOfPathTablePath res = new ChoiceOfPathTablePath();
        if (managementCost != null)
        {
            List<CostMap.CostRoad> costRoads = managementCost.GetPath(begin,end);
            if (costRoads != null)
            {
                int i;
                Vector3 pos;
                ChoiceOfPathTableElement element;
                for (i = 0;i < costRoads.Count; i++)
                {
                    element = new ChoiceOfPathTableElement();
                    pos = costRoads[i].GetPosition();
                    element.x = pos.x;
                    element.y = pos.y;
                    element.z = pos.z;
                    res.elements.Add(element);
                }
            }
        }

        return res;
    }

    private List<EnemyInfoSerialize> GetEnemyInfo()
    {
        List<EnemyInfoSerialize> res = null;

        if (enemyInfo != null)
        {
            EnemyParams[] enemies = enemyInfo.GetEnemyParams();
            if (enemies != null)
            {
                res = new List<EnemyInfoSerialize>();
                EnemyInfoSerialize enemyInfoSerialize;
                int i;
                for(i = 0;i < enemies.Length;i++)
                {
                    enemyInfoSerialize = new EnemyInfoSerialize(enemies[i]);
                    enemyInfoSerialize.no = i;
                    res.Add(enemyInfoSerialize);
                }
            }
        }

        return res;
    }

    private List<TowerSerialize> GetTowerInfo()
    {
        List<TowerSerialize> res = null;

        if (towers != null)
        {
            TowerParams[] t = towers.GetTowerParams();
            if (t != null)
            {
                res = new List<TowerSerialize>();
                TowerSerialize towerSerialize;
                int i;
                for (i = 0; i < t.Length; i++)
                {
                    towerSerialize = new TowerSerialize(t[i]);
                    towerSerialize.no = i;
                    res.Add(towerSerialize);
                }
            }
        }

        return res;
    }

    private List<TowerKillEnemiesSerialize> TowerKillEnemiesInfo(TileCharacter begin, TileCharacter end, string enemy)
    {
        List<TowerKillEnemiesSerialize> tower = new List<TowerKillEnemiesSerialize>();
        TowerKillEnemiesSerialize towerKillEnemy;
        foreach (TowerCharacter item in towerCharacters)
        {
            if (item != null)
            {
                towerKillEnemy = new TowerKillEnemiesSerialize();
                towerKillEnemy.type = item.type;
                towerKillEnemy.killPassEnemies = managementCost.GetKillPassEnemies(begin, end, enemy, item.type);
                towerKillEnemy.lossHealthPassEnemies = managementCost.GetLossPathHealth(begin, end, enemy, item.type);
                tower.Add(towerKillEnemy);
            }
        }

        return tower;
    }

    private List<KillEnemies> KillEnemiesInfo(TileCharacter begin, TileCharacter end)
    {
        List<KillEnemies> killEnemies = new List<KillEnemies>();
        KillEnemies killEnemy;
        foreach (EnemyCharacter item in enemyCharacters)
        {
            if (item != null)
            {
                killEnemy = new KillEnemies();
                killEnemy.type = item.type;
                killEnemy.towers = TowerKillEnemiesInfo(begin, end, item.type);
                killEnemies.Add(killEnemy);
            }
        }

        return killEnemies;
    }

// Downloading information about enemies who left the specified starting point
// begin - starting point
// Returns a list of enemies
    public List<TileInfoEnemies> GetStartEnemies(TileCharacter begin)
    {
        List<TileInfoEnemies> enemies = new List<TileInfoEnemies>();
        TileInfoEnemies enemy;
        foreach (EnemyCharacter item in enemyCharacters)
        {
            if (item != null)
            {
                enemy = new TileInfoEnemies();
                enemy.type = item.type;
                enemy.enemies = begin.GetEnemies(item.type);
                enemies.Add(enemy);
            }
        }

        return enemies;
    }

// Downloading information about enemies who have reached the selected tile
// tile - tile
// Returns a list of enemies
    public List<TileInfoEnemies> GetPassEnemies(TileCharacter tile)
    {
        List<TileInfoEnemies> enemies = new List<TileInfoEnemies>();
        TileInfoEnemies enemy;
        foreach (EnemyCharacter item in enemyCharacters)
        {
            if (item != null)
            {
                enemy = new TileInfoEnemies();
                enemy.type = item.type;
                enemy.enemies = tile.GetPassEnemies(item.type);
                enemy.endMeanHealth = tile.GetEndMeanHealth(item.type);
                enemies.Add(enemy);
            }
        }

        return enemies;
    }

    private WavesSerialize GetWavesInfo()
    {
        WavesSerialize waves = new WavesSerialize();
        if (managementCash != null)
        {
            waves.cash = managementCash.GetEnemyCoins();
        }

        return waves;
    }

    private TowersSerialize GetTowersInfo()
    {
        TowersSerialize towers = new TowersSerialize();
        if (managementCash != null)
        {
            towers.cash = managementCash.GetCoins();
        }

        return towers;
    }

    private AnswerData LevelPathDataAnswer(string title, bool level)
    {
        AnswerData answerData = new AnswerData();
        answerData.title = title;
        if (managementCost != null)
        {
            TileCharacter[] beginTile = managementCost.GetTileBegin();
            TileCharacter[] endTile = managementCost.GetTileEnd();
            if (beginTile != null && endTile != null)
            {
                ChoiceOfPath data = new ChoiceOfPath();
                data = new ChoiceOfPath();
                data.beginTileCount = beginTile.Length;
                data.endTileCount = endTile.Length;
                int i, j;
                Vector2Int posBegin, posEnd;
                ChoiceOfPathPath choice;
                HitEnemyInfo hitEnemyInfo;
                List<TowerCharacter> towers;
                for (i = 0; i < data.beginTileCount; i++)
                {
                    posBegin = managementCost.GetPositionTile(beginTile[i]);
                    for (j = 0; j < data.endTileCount; j++)
                    {
                        posEnd = managementCost.GetPositionTile(endTile[j]);
                        choice = new ChoiceOfPathPath();
                        choice.cost = managementCost.GetCost(beginTile[i], endTile[j]);
                        if (float.IsNaN(choice.cost))
                        {
                            continue;
                        }
                        choice.tileBegin.x = posBegin.x;
                        choice.tileBegin.y = posBegin.y;
                        choice.tileBegin.no = i;
                        choice.tileBegin.enemy = GetStartEnemies(beginTile[i]);
                        choice.tileEnd.x = posEnd.x;
                        choice.tileEnd.y = posEnd.y;
                        choice.tileEnd.no = j;
                        choice.tileEnd.enemy = GetPassEnemies(endTile[i]);                        
                        choice.shotAtTiles = managementCost.GetShotAtTiles(beginTile[i], endTile[j]);
                        towers = managementCost.GetTowers(beginTile[i], endTile[j]);
                        if (towers != null)
                            choice.towers = towers.Count;
                        else
                            choice.towers = 0;
                        choice.sumTowerPlace = managementCost.GetSumNeigboursTileTypePath(beginTile[i], endTile[j],0, new string[] { "Ground" });
                        //                        choice.killEnemies = managementCost.GetKillEnemies(beginTile[i], endTile[j],"Paper","Tower");

                        //                        choice.lossHealth = managementCost.GetLossHealth(beginTile[i], endTile[j]);
                        hitEnemyInfo = managementCost.GetHits(beginTile[i], endTile[j]);
                        choice.hits = hitEnemyInfo.nHit;
                        choice.noHits = hitEnemyInfo.nNoHit;
                        if (level)
                        {
                            choice.table = AnswerCreatePathElements(beginTile[i], endTile[j]);
                        }
                        data.choiceOfPathPaths.Add(choice);
                    }
                }
                data.waves = GetWavesInfo();
                data.towersSerialize = GetTowersInfo();
                if (level)
                {
                    data.enemies = GetEnemyInfo();
                    data.towers = GetTowerInfo();
                    answerData.levelPath = data;
                }else
                {
                    answerData.choiceOfPath = data;
                }
            }
        }

        return answerData;
    }

    private AnswerData LevelDataAnswer()
    {

        return LevelPathDataAnswer("LevelData", true);
    }

    private AnswerData PathDataAnswer()
    {

        return LevelPathDataAnswer("ChoiceOfPathData", false);
    }

    private bool SetEnemies(CommandSetEnemies data)
    {
        if (data != null && data.enemies != null && enemyInfo != null)
        {
            foreach (EnemyInfoSerialize item in data.enemies)
            {
                if (item != null)
                {
                    return enemyInfo.SetEnemyParams(item.GetEnemyParams(),item.no);
                }
            }
        }

        return false;
    }

    private bool SetTowers(CommandSetTowers data)
    {
        if (data != null && data.towers != null && towers != null)
        {
            foreach (TowerSerialize item in data.towers)
            {
                if (item != null)
                {
                    return towers.SetTowerParams(item.GetTowerParams(), item.no);
                }
            }
        }

        return false;
    }

    private ErrorCode AddTower(CommandAddTower data)
    {
        if (towers != null && data != null)
        {
            towers.Generate(data.no,new Vector2Int((int)data.x,(int)data.y));

            return ErrorCode.None;
        }

        return ErrorCode.Unknown;
    }

    private bool StartEnemy(CommandEnemy data)
    {
        if (enemyInfo != null && data != null && managementCost != null)
        {
            TileCharacter[] beginTiles = managementCost.GetTileBegin();
            TileCharacter[] endTiles = managementCost.GetTileEnd();
            if (beginTiles != null && data.tileBegin.no >= 0 && data.tileBegin.no < beginTiles.Length)
            {
                if (endTiles != null && data.tileEnd.no >= 0 && data.tileEnd.no < endTiles.Length)
                {

                    return enemyInfo.Generate(data.no, beginTiles[data.tileBegin.no], endTiles[data.tileEnd.no]);
                }
            }          
        }

        return false;
    }

    private bool Restart(CommandEnemy data)
    {

        listener.Stop();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        return true;
    }

    private AnswerData OkAnswer()
    {
        AnswerData answerData = new AnswerData();
        answerData.title = "Ok";

        return answerData;
    }

    private AnswerData ErrorAnswer()
    {
        AnswerData answerData = new AnswerData();
        answerData.title = "Error";

        return answerData;
    }

    private async void Receive()
    {
        String msg;

        TcpClient client = listener.AcceptTcpClient();
        NetworkStream ns = client.GetStream();
        StreamReader reader = new StreamReader(ns);
        StreamWriter writer = new StreamWriter(ns);
        msg = await reader.ReadLineAsync();
        if (receiveData != null)
        {
            actualReceiveData = GetData(msg);
            AnswerData answerData = OkAnswer();
            if (actualReceiveData.command != null)
            {
                switch(actualReceiveData.command.name)
                {
                    case "LevelData":
                        answerData = LevelDataAnswer();
                        break;
                    case "GetChoiceOfPathData":
                        answerData = PathDataAnswer();
                        break;
                    case "SetEnemies":
                        if(!SetEnemies(actualReceiveData.command.setEnemies))
                        {
                            answerData = ErrorAnswer();
                        }
                        break;
                    case "SetTowers":
                        if (!SetTowers(actualReceiveData.command.setTowers))
                        {
                            answerData = ErrorAnswer();
                        }
                        break;
                    case "AddTower":
                        if (AddTower(actualReceiveData.command.addTower) != ErrorCode.None)
                        {
                            answerData = ErrorAnswer();
                        }
                        break;
                    case "StartEnemy":
                        if (!StartEnemy(actualReceiveData.command.startEnemy))
                        {
                            answerData = ErrorAnswer();
                        }    
                        break;
                    case "Restart":
                        if (!Restart(actualReceiveData.command.startEnemy))
                        {
                        }
                        break;
                }
            }
            string txt = AnswerToXMLString(answerData);
            await writer.WriteAsync(txt.ToCharArray());
            await writer.FlushAsync();
        }
    }

    void Start()
    {
        IPAddress ipadddres;
        ipadddres = IPAddress.Parse(iPAdress);

        listener = new TcpListener(ipadddres, port);
        listener.Start();
        if (receiveData == null)
        {
            receiveData = new UnityEventServerReceiveData();
        }

        towerCharacters = FindObjectsOfType<TowerCharacter>(true);
        enemyCharacters = FindObjectsOfType<EnemyCharacter>(true);
    }

    void Update()
    {
        if (listener != null)
        {
            if (actualReceiveData != null && receiveData != null)
            {
                receiveData.Invoke(actualReceiveData);
                if (actualReceiveData.command != null)
                {
                    command.Invoke(actualReceiveData.command);
                }
                actualReceiveData = null;
            }
            if (actualReceiveData == null)
            {
                if (!listener.Pending())
                {
                }
                else
                {
                    Receive();
                }
            }
        }
    }
}
