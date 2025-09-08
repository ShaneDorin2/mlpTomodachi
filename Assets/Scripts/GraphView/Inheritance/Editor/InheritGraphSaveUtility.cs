using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class InheritGraphSaveUtility
{
    private InhertanceGraphView _targetGraphView;
    private InheritanceContainer _containerCach;
    
    private List<Edge> Edges => _targetGraphView.edges.ToList();
    private List<InheritanceNode> Nodes => _targetGraphView.nodes.ToList().Cast<InheritanceNode>().ToList(); 

    public static InheritGraphSaveUtility GetInstance(InhertanceGraphView targetGraphView)
    {
        return new InheritGraphSaveUtility
        {
            _targetGraphView = targetGraphView
        };
    }

    public void SaveGraph(string fileName)
    {
        if(!Edges.Any()) return; //If there are no edges, return nothing. 

        //Create scriptable object that will contain the data
        var dialogueContainer = ScriptableObject.CreateInstance<InheritanceContainer>();
        var connectedPorts = Edges.Where(x => x.input.node != null).ToArray(); //only get edges that are bonded to another node. 
        
        for (int i = 0; i < connectedPorts.Length; i++)
        {
            var outputNode = connectedPorts[i].output.node as InheritanceNode;
            var inputNode = connectedPorts[i].input.node as InheritanceNode;

            dialogueContainer.NodeLinks.Add(new InheritNodeLinkData  //turn each edge into a NodeLinkData
            {
                BaseNodeGuid = outputNode.GUID,
                PortName = connectedPorts[i].output.portName,
                TargetNodeGuid = inputNode.GUID
            });
        }
        
        foreach (var dialogueNode in Nodes.Where(node => !node.EntryPoint))
        {
            dialogueContainer.dialogueNodeData.Add(new InheritanceNodeData //turn each node into DiologueNodeData
            {
                Guid = dialogueNode.GUID,
                DilalogueText = dialogueNode.DialogueText,
                Position = dialogueNode.GetPosition().position
            });
        }

        if (!AssetDatabase.IsValidFolder("Assets/Resources")) //check if resources folder exists. 
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }
        //save scriptable object into asset files. 
        AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();
    }

    public void LoadGraph(string fileName)
    {
        _containerCach = Resources.Load<InheritanceContainer>(fileName);
        if (_containerCach == null)
        {
            EditorUtility.DisplayDialog("File not found", "target dialogue graph file does not exit!", "OK");
            return;
        }
        ClearGraph();
        GenerateNodes();
        ConnectNodes();
    }

    private void ConnectNodes()
    {

    }

    private void GenerateNodes()
    {
        foreach (var nodeData in _containerCach.dialogueNodeData)
        {
            var tempNode = _targetGraphView.CreateDialogueNode(nodeData.DilalogueText);
            tempNode.GUID = nodeData.Guid;
            _targetGraphView.AddElement(tempNode);

            var nodePorts = _containerCach.NodeLinks.Where(x => x.BaseNodeGuid == nodeData.Guid).ToList(); 
            nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.PortName));
        }
    }

    private void ClearGraph()
    {
        //sets entry point GUID. discards old entry point GUID 
        Nodes.Find(x => x.EntryPoint).GUID = _containerCach.NodeLinks[0].BaseNodeGuid;

        foreach (var node in Nodes)
        {
            if (node.EntryPoint) return;
            Edges.Where(x => x.input.node == node).ToList()
                .ForEach(Edge => _targetGraphView.RemoveElement(Edge));

            _targetGraphView.RemoveElement(node);
        }
    }
}


