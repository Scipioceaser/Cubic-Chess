﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Pawn : Unit
{
    private Mesh pawnMesh;
    private Material pawnMaterial;
    private Map map;
    
    private void Awake()
    {
        SetModelFromAssets(gameObject, "pawn", "pawn");
        map = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
        currentNode = UnitSpawnPoint.GetNearestNode(transform.position);
        currentNode.SetNodeUnit(this);
    }
    
    // TODO: When movement code is implemented, will need to make sure the code allows for a pawn to cirumnavigate the map and not
    // just move up.
    public override List<Vector3> GetValidMovePositions(Vector3 position)
    {
        List<Vector3> validPositions = new List<Vector3>();
        List<Node> nearbyNodes = map.GetNeighbours(map.NodeFromWorldPoints(position));
        
        foreach (Node node in nearbyNodes)
        {
            if (node.GetType() != typeof(NodeMesh))
            {
                if (position.x == 0 || position.x == (Globals.mapSize + 1))
                {
                    if (node.position == position + Vector3.up)
                    {
                        if (node.nodeUnit == null)
                        {
                            print(node.position);
                            validPositions.Add(node.position);
                        }
                    }
                    else if (node.position == position + new Vector3(0, 1, -1) || node.position == position + new Vector3(0, 1, 1))
                    {
                        if (node.nodeUnit != null)
                        {
                            print(node.position);
                            validPositions.Add(node.position);
                        }
                    }
                }
                else if (position.z == 0 || position.z == (Globals.mapSize + 1))
                {
                    if (node.position == position + Vector3.up)
                    {
                        if (node.nodeUnit == null)
                        {
                            print(node.position);
                            validPositions.Add(node.position);
                        }
                    }
                    else if (node.position == position + new Vector3(-1, 1, 0) || node.position == position + new Vector3(1, 1, 0))
                    {
                        if (node.nodeUnit != null)
                        {
                            print(node.position);
                            validPositions.Add(node.position);
                        }
                    }
                }
                else if (position.y == 0 || position.y == (Globals.mapHeight + 1))
                {

                }
            }
        }

        print(validPositions.Count);

        return validPositions;
    }
}
