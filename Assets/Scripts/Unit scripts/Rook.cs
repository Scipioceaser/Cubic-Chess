﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Unit
{
    public override void Awake()
    {
        base.Awake();
        SetModelFromAssets(gameObject, "rook", "rook");
        currentNode = UnitSpawnPoint.GetNearestNode(transform.position);
        currentNode.SetNodeUnit(this);
        AlignUnit(currentNode.position);
    }

    //TODO: Add sideways transitionary movement
    //TODO: Limit movement when enemies in the path of the rook
    public override List<Vector3> GetValidMovePositions(Vector3 position, int team = 1)
    {
        List<Vector3> validPositions = new List<Vector3>();
        List<Node> nearbyNodes = map.GetNeighbours(currentNode, 3);
        
        foreach (Node node in nearbyNodes)
        {
            if (node.GetType() != typeof(NodeMesh) && node != currentNode)
            {
                if (node.position.y == 0 || node.position.y == Globals.mapHeight + 1)
                {
                    if (node.position.y == unAdjustedPosition.y)
                    {
                        if (node.position.y == 0)
                        {
                            // Movement along the top and bottom
                            if (node.nodeUnit == null)
                            {
                                float d = Vector3.Distance(node.position, position);

                                if (node.position == position + Vector3.forward * d)
                                {
                                    if (node.position.x == 0 || node.position.x == Globals.mapSize + 1 ||
                                    node.position.z == 0 || node.position.z == Globals.mapSize + 1)
                                    {
                                        validPositions.Add(node.position + Vector3.up);
                                    }
                                    else
                                    {
                                        validPositions.Add(node.position);
                                    }
                                }
                                else if (node.position == position - Vector3.forward * d)
                                {
                                    if (node.position.x == 0 || node.position.x == Globals.mapSize + 1 ||
                                    node.position.z == 0 || node.position.z == Globals.mapSize + 1)
                                    {
                                        validPositions.Add(node.position + Vector3.up);
                                    }
                                    else
                                    {
                                        validPositions.Add(node.position);
                                    }
                                }
                                if (node.position == position + Vector3.right * d)
                                {
                                    if (node.position.x == 0 || node.position.x == Globals.mapSize + 1 ||
                                    node.position.z == 0 || node.position.z == Globals.mapSize + 1)
                                    {
                                        validPositions.Add(node.position + Vector3.up);
                                    }
                                    else
                                    {
                                        validPositions.Add(node.position);
                                    }
                                }
                                else if (node.position == position - Vector3.right * d)
                                {
                                    if (node.position.x == 0 || node.position.x == Globals.mapSize + 1 ||
                                    node.position.z == 0 || node.position.z == Globals.mapSize + 1)
                                    {
                                        validPositions.Add(node.position + Vector3.up);
                                    }
                                    else
                                    {
                                        validPositions.Add(node.position);
                                    }
                                }
                            }
                        }
                        else if (node.position.y == Globals.mapHeight + 1)
                        {
                            // Movement along the top and bottom
                            if (node.nodeUnit == null)
                            {
                                float d = Vector3.Distance(node.position, position);

                                if (node.position == position + Vector3.forward * d)
                                {
                                    if (node.position.x == 0 || node.position.x == Globals.mapSize + 1 ||
                                    node.position.z == 0 || node.position.z == Globals.mapSize + 1)
                                    {
                                        validPositions.Add(node.position - Vector3.up);
                                    }
                                    else
                                    {
                                        validPositions.Add(node.position);
                                    }
                                }
                                else if (node.position == position - Vector3.forward * d)
                                {
                                    if (node.position.x == 0 || node.position.x == Globals.mapSize + 1 ||
                                    node.position.z == 0 || node.position.z == Globals.mapSize + 1)
                                    {
                                        validPositions.Add(node.position - Vector3.up);
                                    }
                                    else
                                    {
                                        validPositions.Add(node.position);
                                    }
                                }
                                if (node.position == position + Vector3.right * d)
                                {
                                    if (node.position.x == 0 || node.position.x == Globals.mapSize + 1 ||
                                    node.position.z == 0 || node.position.z == Globals.mapSize + 1)
                                    {
                                        validPositions.Add(node.position - Vector3.up);
                                    }
                                    else
                                    {
                                        validPositions.Add(node.position);
                                    }
                                }
                                else if (node.position == position - Vector3.right * d)
                                {
                                    if (node.position.x == 0 || node.position.x == Globals.mapSize + 1 ||
                                    node.position.z == 0 || node.position.z == Globals.mapSize + 1)
                                    {
                                        validPositions.Add(node.position - Vector3.up);
                                    }
                                    else
                                    {
                                        validPositions.Add(node.position);
                                    }
                                }
                            }
                        }
                    }
                    else if (node.position.y == unAdjustedPosition.y + 1)
                    {
                        // Going up
                        if (node.position == position + Vector3.up)
                        {
                            //validPositions.Add(node.position);
                        }
                        
                        if (node.position == position - Vector3.forward + Vector3.up 
                            || node.position == position + Vector3.forward + Vector3.up)
                        {
                            validPositions.Add(node.position);
                        }
                        else if (node.position == position - Vector3.right + Vector3.up
                            || node.position == position + Vector3.right + Vector3.up)
                        {
                            validPositions.Add(node.position);
                        }
                    }
                    else if (node.position.y == unAdjustedPosition.y - 1)
                    {
                        // Going down
                        if (node.position == position - Vector3.up)
                        {
                            //validPositions.Add(node.position);
                        }

                        if (node.position == position - Vector3.forward - Vector3.up
                            || node.position == position + Vector3.forward - Vector3.up)
                        {
                            validPositions.Add(node.position);
                        }
                        else if (node.position == position - Vector3.right - Vector3.up
                            || node.position == position + Vector3.right - Vector3.up)
                        {
                            validPositions.Add(node.position);
                        }
                    }
                }
                else
                {
                    float d = Vector3.Distance(node.position, position);
                    
                    if (node.position == position + Vector3.up * d)
                    {
                        validPositions.Add(node.position);
                    }
                    else if (node.position == position - Vector3.up * d)
                    {
                        validPositions.Add(node.position);
                    }
                    else if (node.position == position + Vector3.forward * d || node.position == position + Vector3.right * d)
                    {
                        validPositions.Add(node.position);
                    }
                    else if (node.position == position - Vector3.forward * d || node.position == position - Vector3.right * d)
                    {
                        validPositions.Add(node.position);
                    }
                }
            }
        }

        return validPositions;
    }

    public override void MoveAlongPath(Vector3 destination = new Vector3())
    {
        MoveRook(destination);
    }

    private void MoveRook(Vector3 destination)
    {
        Vector3 p = UnitSpawnPoint.GetAdjustedSpawnPosition(0.5f, destination,
            UnitSpawnPoint.GetNearestNode(destination, 1, true).transform.position);

        // Handle rotation
        AlignUnit(destination);

        // Set nodes
        currentNode.SetNodeUnit(null);
        UnitSpawnPoint.GetNearestNode(destination).SetNodeUnit(this);
        currentNode = UnitSpawnPoint.GetNearestNode(destination);
        unAdjustedPosition = destination;

        // Actually move
        StartCoroutine(Move(transform.position, p, 0.5f));
    }
}
