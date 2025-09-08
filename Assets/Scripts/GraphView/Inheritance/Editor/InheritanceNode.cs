using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

/* Manages the nodes that appear in the graphView
 * Used by DialogueGraphView class
 */

public class InheritanceNode : Node
{
    //'Global Unique ID' is a AUTOMATICALLY generated id string that Unity provodes to all files and assets. 
    public string GUID;

    //The dialogue text contained within the node.
    public string DialogueText;

    //idk
    public bool EntryPoint = false;
}
