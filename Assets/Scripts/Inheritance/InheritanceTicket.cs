using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EParent
{
    PARENT_A, 
    PARENT_B
}

public enum ETrait
{
    HAIR,
    COAT,
    EYES,
    STREAK
}

public struct InheritedTrait
{
    public EParent parent;
    public ETrait trait;
}

public struct InheritanceTicket 
{
    public InheritedTrait hair;
    public InheritedTrait coat;
    public InheritedTrait eyes;
    public InheritedTrait streak;
}
