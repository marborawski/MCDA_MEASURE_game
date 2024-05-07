using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 3D terrain models
public class TerrainModels : MonoBehaviour
{

// Model package name
    public string nameModels;

// Basic model
    public GameObject normal;

// The path is straight
    public GameObject straight;

// A bend in the road
    public GameObject curve;

// A crossroads with three roads branching off
    public GameObject intersection3Road;

// A crossroads with four roads branching off
    public GameObject intersection4Road;

// The beginning or end of the path
    public GameObject beginEnd;

// Start of the track
    public GameObject begin;

// End of track
    public GameObject end;

// Straight path the beginning of the path
    public GameObject straightBegin;

// Straight path end of path
    public GameObject straightEnd;
}
