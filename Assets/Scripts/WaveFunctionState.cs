using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveFunctionState : MonoBehaviour
{
    [HideInInspector]
    public string GUID;
    
    public string stateName;
    public Dictionary<int, List<WaveFunctionState>> allowedNeighborStates = new Dictionary<int, List<WaveFunctionState>>(); 
}
