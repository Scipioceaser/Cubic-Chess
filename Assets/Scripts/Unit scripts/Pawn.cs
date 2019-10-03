using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Pawn : Unit
{
    private Mesh pawnMesh;
    private Material pawnMaterial;
    private Node currentNodeLocation;
    private bool initialized = false;

    private void Awake()
    {
        SetModelFromAssets(gameObject, "pawn", "pawn");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Globals.meshNodesCreated == (Globals.mapSize * Globals.mapSize * Globals.mapHeight) && !initialized)
        {
            initialized = true;
            currentNodeLocation = UnitSpawnPoint.GetNearestNode(transform.localPosition);
            print(currentNodeLocation.position);
        }
    }
}
