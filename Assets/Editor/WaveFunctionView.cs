using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class WaveFunctionView : GraphView
{
    public WaveFunctionCollapse target;
    public WaveFunctionView()
    {
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        GridBackground background = new GridBackground();
        Insert(0, background);
        background.StretchToParentSize();

        styleSheets.Add(Resources.Load<StyleSheet>("WaveFunctionRedactor"));
    }

    public WaveFunctionStateNode GenerateNode(WaveFunctionState state, int index)
    {
        WaveFunctionStateNode node = new WaveFunctionStateNode{
            title = state.stateName,
            GUID = Guid.NewGuid().ToString(),
            referenceState = state,
        };

        node.capabilities &= ~Capabilities.Deletable;
        node.capabilities &= ~Capabilities.Copiable;

        node.SetPosition(new Rect(200*index, 100, 500, 200));

        return node;
    }

    Port GeneratePort(WaveFunctionStateNode node, Direction portDirection)
    {
        Port.Capacity capacity = Port.Capacity.Multi;
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(bool));
    }

    public void GenerateNodePorts(WaveFunctionStateNode node, int numberOfSides)
    {
        for (int i = 0; i < numberOfSides; i++)
        {
            Port outPort = GeneratePort(node, Direction.Output);
            outPort.portName = "Out, Side - " + i.ToString();
            outPort.name = "Out, Side - " + i.ToString();
            node.outputContainer.Add(outPort);

            Port inPort = GeneratePort(node, Direction.Input);
            inPort.portName = "In, Side - " + i.ToString();
            inPort.name = "In, Side - " + i.ToString();
            node.inputContainer.Add(inPort);
        }

        node.RefreshExpandedState();
        node.RefreshPorts();
    }

    public void GenerateUnconnectButton(WaveFunctionStateNode node)
    {
        Button unconnectButton = new Button(() => {
            List<Edge> toDelete = new List<Edge>();

            foreach (VisualElement e in node.outputContainer.Children())
            {
                if (e is Port port)
                {
                    foreach (Edge edge in port.connections)
                    {
                        if (edge.input.node == edge.output.node)
                        {
                            toDelete.Add(edge);
                        }
                    }
                }
            }

            foreach (Edge edge in toDelete)
            {
                edge.input.Disconnect(edge);
                edge.output.Disconnect(edge);
            }

            DeleteElements(toDelete);
        });

        unconnectButton.text = "Unconnect itself";

        node.titleContainer.Add(unconnectButton);
}

    public void GenerateStateNodes()
    {
        int totalCountOfStates = target.avaibleStates.Count;

        for (int i = 0; i < totalCountOfStates; i++)
        {
            WaveFunctionState state = target.avaibleStates[i];
            WaveFunctionStateNode node = GenerateNode(state, i);

            GenerateUnconnectButton(node);

            GenerateNodePorts(node, target.numberOfSides);

            this.AddElement(node);
        }
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        List<Port> compatiblePorts = new List<Port>();

        ports.ForEach((port) => {
            if (port != startPort) compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }
}
