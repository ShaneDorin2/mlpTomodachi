using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InheritanceContainer : ScriptableObject
{
    public List<InheritNodeLinkData> NodeLinks = new List<InheritNodeLinkData>();
    public List<InheritanceNodeData> dialogueNodeData = new List<InheritanceNodeData>(); 
}
