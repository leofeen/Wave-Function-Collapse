using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveFunctionState : MonoBehaviour
{
    [HideInInspector]
    public int id;
    
    public string stateName;
    public List<WaveFunctionState> allowedNeighborStates = new List<WaveFunctionState>(); 
}
