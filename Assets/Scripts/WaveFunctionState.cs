using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveFunctionState : MonoBehaviour
{
    [HideInInspector]
    public int id;
    
    public string stateName;
    public Dictionary<int, WaveFunctionState> allowedNeighborStates = new Dictionary<int, WaveFunctionState>(); 
}
