using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base node class, empty
// Probably shouldn't use this for spawning, but to hold all the functions for all the nodes
// Addendum: Ehh, this could be the class for unit movement too.

[RequireComponent(typeof(BoxCollider))]
public class Node : MonoBehaviour
{
    public Vector3 position;
    public int scale = 1;
    public BoxCollider nodeCollider;
    public Unit nodeUnit;
    
    public virtual void Init(int scale, Vector3 position)
    {
        this.scale = scale;
        this.position = position;
        this.nodeCollider = GetComponent<BoxCollider>();
        this.nodeUnit = null;
    }

    public void SetNodeUnit(Unit unit)
    {
        nodeUnit = unit;
    }
}
