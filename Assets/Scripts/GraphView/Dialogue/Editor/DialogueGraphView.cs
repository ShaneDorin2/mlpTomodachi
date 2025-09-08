using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

/* Manages the funtionality of the GraphView. 
 * Used by DialogueGraph class
 * Uses DialogueNode class
 */

public class DialogueGraphView : GraphView 
{
    private readonly Vector2 defaultNodeSize = new Vector2(150, 200);

    public DialogueGraphView() //Constructor
    {
        //alows you to zoom in and out. 
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        
        //Adds pre-made tools to our graphView.
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());



        // AddElement is from GraphView base class
        AddElement(GenerateEntryPointNode()); //Creat starter node
    }

    
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    //to determin what ports can attach to what.
    {
        var compatiblePorts = new List<Port>();

        ports.ForEach(port =>
        {
            if(startPort != port && startPort.node != port.node) 
            {
                compatiblePorts.Add(port);
            }
        });
        return compatiblePorts;
    }
    

    private Port GeneratePort(DialogueNode node,                             //the node that will have this port. 
                              Direction portDirection,                       //input port or output port ?
                              Port.Capacity capacity = Port.Capacity.Single) //how many edges can connect to this one port?
    {
        // Returns a Port object.
        return node.InstantiatePort(Orientation.Horizontal, 
                                     portDirection, 
                                     capacity, 
                                     typeof(float)); //the type of data transfere though port (we don't use this here_
    }


    private DialogueNode GenerateEntryPointNode()
    {
        var node = new DialogueNode //create new DialogueNode object
        {
            // from Node constuctor
            title = "Start",

            //from DialogueNode contructor
            GUID = Guid.NewGuid().ToString(), //Guid => Unity class that auto-generates a unique ID. 
            DialogueText = "I am the first-est node! yay!!",
            EntryPoint = true
        };

        var generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "Next"; //change name of Port object
        node.outputContainer.Add(generatedPort); //Adds Port to Node

        //do these after adding to containers to prevent deformaties. 
        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(x: 100, y: 200, width: 100, height: 150));
        return node; 
    }  
    

    public void CreateNode(string nodeName)
    {
        // NOTE: Just because a node is CREATED does not mean it will appear!
        // It must be ADDED to the GraphView. 
        AddElement(CreateDialogueNode(nodeName));
    }


    public DialogueNode CreateDialogueNode(string nodeName) //called when 'create node' button is pressed
    {
        var dialogueNode = new DialogueNode //create node obj
        {
            // constructor
            title = nodeName,
            DialogueText = nodeName,
            GUID = Guid.NewGuid().ToString(),
        };

        // create and add input port
        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);

        //create button that adds new output port.
        var button = new Button(()=>{ AddChoicePort(dialogueNode);}); // bru. y this en in );}); ???
        button.text = "New Choice";
        dialogueNode.titleContainer.Add(button);

        // always do these after adding to containers to prevent deformaties.
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();

        //pos and size of node
        dialogueNode.SetPosition(new Rect(Vector2.zero, defaultNodeSize));

        return dialogueNode;
    }


    public void AddChoicePort(DialogueNode dialogueNode, string overridenPortName = "") //creates new output port upon button press. 
    {
        var generatedPort = GeneratePort(dialogueNode, Direction.Output);

        //Hide the port label text. So only textfield is visible
        var oldlabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldlabel);

        //gets the number of existing ports and uses it to name port
        var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;

        var choicePortName = string.IsNullOrEmpty(overridenPortName)
            ? $"Choice {outputPortCount+1}" 
            : overridenPortName;

        //Create re-nameable ports
        var textField = new TextField
        {
            name = string.Empty,
            value = choicePortName
        };
        textField.RegisterValueChangedCallback(evt  => generatedPort.portName = evt.newValue);

        //add text field to port label
        generatedPort.contentContainer.Add(new Label (" "));
        generatedPort.contentContainer.Add(textField);

        var deleteButton = new Button(() => RemovePort(dialogueNode, generatedPort))
        {
            text = "X"
        };
        generatedPort.contentContainer.Add(deleteButton);

        generatedPort.portName = choicePortName;

        //always do this
        dialogueNode.outputContainer.Add(generatedPort);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();
        ;
    }

    /*public void AddChoicePort(DialogueNode nodeCache, string overriddenPortName = "")
    {
        var generatedPort = GeneratePort(nodeCache, Direction.Output);
        var portLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(portLabel);

        var outputPortCount = nodeCache.outputContainer.Query("connector").ToList().Count();
        var outputPortName = string.IsNullOrEmpty(overriddenPortName)
            ? $"Option {outputPortCount + 1}"
            : overriddenPortName;


        var textField = new TextField()
        {
            name = string.Empty,
            value = outputPortName
        };
        textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
        generatedPort.contentContainer.Add(new Label("  "));
        generatedPort.contentContainer.Add(textField);
        var deleteButton = new Button(() => RemovePort(nodeCache, generatedPort))
        {
            text = "X"
        };
        generatedPort.contentContainer.Add(deleteButton);
        generatedPort.portName = outputPortName;
        nodeCache.outputContainer.Add(generatedPort);
        nodeCache.RefreshPorts();
        nodeCache.RefreshExpandedState();
    }*/

    /*private void RemovePort(DialogueNode dialogueNode, Port generatedPort)
    {
        // find port with matching port name and matching node
        var targetEdge = edges.ToList().Where(x =>
            x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);
        
        if (!targetEdge.Any()) return;
        var edge = targetEdge.First();

        //must remove and disconnect edge or the line will remain even after removing the port
        edge.input.Disconnect(edge);
        RemoveElement(targetEdge.First());

        dialogueNode.outputContainer.Remove(generatedPort);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();
    }*/

    private void RemovePort(Node node, Port socket)
    {
        var targetEdge = edges.ToList()
            .Where(x => x.output.portName == socket.portName && x.output.node == socket.node);
        if (targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());
        }

        node.outputContainer.Remove(socket);
        node.RefreshPorts();
        node.RefreshExpandedState();
    }
}
