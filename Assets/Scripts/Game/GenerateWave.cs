using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GenerateWave;

// Enemy wave generation module
public class GenerateWave : MonoBehaviour
{

 // Cash manager
    public ManagementCost managementCost;

// Information about the enemy's start
    [System.Serializable]
    public class WaveGarbage
    {
// Pattern number of the enemy's 3D model
        public int no;
// Time from the start of the enemy wave after which the enemy will be created
        public float time;
    }

// Information about the wave of enemies
    [System.Serializable]
    public class WavesGarbage
    {
// Information about the opponents included in the enemy wave
        public WaveGarbage[] wave;
// Time when the enemy wave starts
        public float time;
    }

// A table containing information about enemy waves
    public WavesGarbage[] waves;

// 3D models that are enemy patterns
    public GameObject[] objects;

// Starting tiles from which enemies start
    public TileCharacter[] beginTiles;

    private int actualWave = 0;
    private int actual = 0;
    private bool[] active;

    private float time = 0;
    private float timeWave = 0;

// Generate an opponent
// no        - enemy's pattern number
// beginTile - starting tile
// endTile   - ending tile
    public void Generate(int no, TileCharacter beginTile, TileCharacter endTile)
    {
        int n = objects.Length;
        if (n > 0 && n > no && beginTile != null && endTile != null)
        {
            GameObject go = objects[no];
            if (go != null)
            {
                GameObject newGo = Instantiate(go, beginTile.transform.position,
                    beginTile.transform.rotation);
                if (newGo != null)
                {
                    newGo.SetActive(true);
                }
            }
        }

    }

    void Start()
    {
        if (waves != null && waves.Length > 0)
        {
            active = new bool[waves.Length];
        }
    }

    void Update()
    {
        time += Time.deltaTime;
        timeWave += Time.deltaTime;
        if (waves != null && actual < waves.Length)
        {
            if (!active[actual])
            {
                if (waves[actual].time <= time)
                {
                    active[actual] = true;
                    timeWave = 0;
                    actualWave = 0;
                }
            }
            if (active[actual])
            {
                if (actualWave < waves[actual].wave.Length)
                {
                    if (waves[actual].wave[actualWave].time <= timeWave && managementCost != null)
                    {
                        actualWave++;
                    }
                }
                else
                {
                    actual++;
                }
            }
        }
    }
}
