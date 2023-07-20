using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class WaveFunctionCollapse : MonoBehaviour
{
    public int spaceSize = 10;
    public int numberOfSides = 4;
    public List<WaveFunctionState> avaibleStates = new List<WaveFunctionState>();

    void ClearNeighbourStates()
    {
        foreach (WaveFunctionState state in avaibleStates)
        {
            for (int i = 0; i < numberOfSides; i++)
            {
                state.allowedNeighborStates[i] = new List<WaveFunctionState>();
            }
        }
    }

    void DeduplicateNeighbourStates()
    {
        foreach (WaveFunctionState state in avaibleStates)
        {
            for (int i = 0; i < numberOfSides; i++)
            {
                state.allowedNeighborStates[i] = state.allowedNeighborStates[i].Distinct().ToList();
            }
        }
    }

    public void ApplyPreset(WavePreset preset)
    {  
        ClearNeighbourStates();

        for (int i = 0; i < preset.nodeDatas.Count; i++)
        {
            WaveFunctionState currentState = avaibleStates[i];

            List<NodeLinkData> connections = preset.nodeLinkDatas.Where(x => x.baseNodeGUID == currentState.GUID).ToList();
            for (int j = 0; j < connections.Count; j++)
            {
                string targetGuid = connections[j].targetNodeGUID;
                WaveFunctionState targetState = avaibleStates.First(x => x.GUID == targetGuid);

                int currentSide = connections[j].baseNodePortSide;
                int targetSide = connections[j].targetNodePortSide;

                currentState.allowedNeighborStates[currentSide].Add(targetState);
                targetState.allowedNeighborStates[targetSide].Add(currentState);
            }
        }

        DeduplicateNeighbourStates();
    }
}
