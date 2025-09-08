using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InheritanceContainer : ScriptableObject
{
    public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
    public List<DialogueNodeData> dialogueNodeData = new List<DialogueNodeData>(); 
}
