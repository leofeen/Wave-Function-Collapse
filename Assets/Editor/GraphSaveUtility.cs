using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System.Linq;
using UnityEngine.UIElements;

public class GraphSaveUtility
{
    WaveFunctionView targetView;
    List<Edge> edges => targetView.edges.ToList();
    List<WaveFunctionStateNode> nodes => targetView.nodes.ToList().Cast<WaveFunctionStateNode>().ToList();

    WavePreset cachePreset;

    public static GraphSaveUtility GetInstance(WaveFunctionView targetView)
    {
        return new GraphSaveUtility{
            targetView = targetView,
        };
    }

    public void SaveGraph(string presetName)
    {
        if (!edges.Any()) return;

        WavePreset preset = ScriptableObject.CreateInstance<WavePreset>();

        Edge[] connectedEdges = edges.Where(x => x.input.node != null).ToArray();
        for (int i = 0; i < connectedEdges.Length; i++)
        {
            WaveFunctionStateNode inputNode = connectedEdges[i].input.node as WaveFunctionStateNode;
            WaveFunctionStateNode outputNode = connectedEdges[i].output.node as WaveFunctionStateNode;

            preset.nodeLinkDatas.Add(new NodeLinkData{
                baseNodeGUID = outputNode.GUID,
                baseNodePortName = connectedEdges[i].output.portName,
                targetNodeGUID = inputNode.GUID,
                targetNodePortName = connectedEdges[i].input.portName,
            });
        }

        foreach (WaveFunctionStateNode node in nodes)
        {
            preset.nodeDatas.Add(new WaveFunctionStateNodeData{
                GUID = node.GUID,
                position = node.GetPosition().position,
                referenceState = node.referenceState,
            });
        }

        if (!AssetDatabase.IsValidFolder("Assets/Resources")) AssetDatabase.CreateFolder("Assets", "Resources");

        AssetDatabase.CreateAsset(preset, $"Assets/Resources/{presetName}.asset");
        AssetDatabase.SaveAssets();
    }

    public void LoadGraph(string presetName)
    {
        cachePreset = Resources.Load<WavePreset>(presetName);

        if (cachePreset is null)
        {
            EditorUtility.DisplayDialog("FIle does not exist", "Please, enter valid preset name", "OK");
            return;
        }

        ClearGraph();
        CreateNodes();
        ConnectNodes();
    }

    void ClearGraph()
    {
        foreach (WaveFunctionStateNode node in nodes)
        {
            edges.Where(x => x.input.node == node).ToList().ForEach(edge => targetView.RemoveElement(edge));
            targetView.RemoveElement(node);
        }
    }

    void CreateNodes()
    {
        foreach (WaveFunctionStateNodeData nodeData in cachePreset.nodeDatas)
        {
            WaveFunctionStateNode tempNode = targetView.GenerateNode(nodeData.referenceState, 0);
            targetView.GenerateNodePorts(tempNode, targetView.target.numberOfSides);
            targetView.GenerateUnconnectButton(tempNode);

            tempNode.GUID = nodeData.GUID;
            tempNode.SetPosition(new Rect(nodeData.position, new Vector2(500, 200)));

            targetView.AddElement(tempNode);
        }
    }

    void ConnectNodes()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            List<NodeLinkData> connections = cachePreset.nodeLinkDatas.Where(x => x.baseNodeGUID == nodes[i].GUID).ToList();
            for (int j = 0; j < connections.Count; j++)
            {
                string targetGuid = connections[j].targetNodeGUID;
                WaveFunctionStateNode targetNode = nodes.First(x => x.GUID == targetGuid);

                LinkNodes(nodes[i].outputContainer[j].Q<Port>(), targetNode.inputContainer.Q<Port>(connections[j].targetNodePortName));
            }
        }
    }

    void LinkNodes(Port output, Port input)
    {
        Edge edge = new Edge{
            output = output,
            input = input,
        };

        edge.input.Connect(edge);
        edge.output.Connect(edge);

        targetView.Add(edge);
    }
}
