using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum EParent
{
    PARENT_A, 
    PARENT_B
}

enum ETrait
{
    HAIR,
    COAT,
    EYES,
    STREAK
}

public struct InheritedTrait
{
    EParent parent;
    ETrait trait;
}

public struct InheritanceTicket 
{
    InheritedTrait hair;
    InheritedTrait coat;
    InheritedTrait eyes;
    InheritedTrait streak;
}
