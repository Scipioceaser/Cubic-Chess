using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Pawn : Unit
{
    private Mesh pawnMesh;
    private Material pawnMaterial;
    private Map map;
    private Vector3 pawnDirection;

    private void Awake()
    {
        SetModelFromAssets(gameObject, "pawn", "pawn");
        map = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
        currentNode = UnitSpawnPoint.GetNearestNode(transform.position);
        currentNode.SetNodeUnit(this);
    }

    // TODO: Make pawn rotate around edge
    public override List<Vector3> GetValidMovePositions(Vector3 position, int team = 1)
    {
        List<Vector3> validPositions = new List<Vector3>();
        List<Node> nearbyNodes = map.GetNeighbours(map.NodeFromWorldPoints(position), 1);
        
        if (unAdjustedPosition.y == 0 || unAdjustedPosition.y == Globals.mapHeight + 1)
        {
            pawnDirection = Vector3.forward;
        }
        else
        {
            pawnDirection = Vector3.up;
        }
        
        // Make sure the object move properly and i haven't gone insane.
        foreach (Node node in nearbyNodes)
        {
            if (node.GetType() != typeof(NodeMesh) && node.position != currentNode.position)
            {
                if (node.position.y == 0 || node.position.y == (Globals.mapHeight + 1))
                {
                    // Check if lateral board movement
                    if (node.position.y == unAdjustedPosition.y)
                    {
                        if (node.position.y == 0)
                        {
                            // Movement along the top and bottom
                            if (node.nodeUnit == null)
                            {
                                if (node.position == position + pawnDirection)
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
                                else if (node.position == position - pawnDirection)
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
                                if (node.position == position + pawnDirection)
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
                                else if (node.position == position - pawnDirection)
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
                            validPositions.Add(node.position);
                        }

                        if (node.position == position - transform.forward + Vector3.up)
                        {
                            validPositions.Add(node.position);
                        }
                    }
                    else if (node.position.y == unAdjustedPosition.y - 1)
                    {
                        // Going down
                        if (node.position == position - Vector3.up)
                        {
                            validPositions.Add(node.position);
                        }

                        if (node.position == position - transform.forward - Vector3.up)
                        {
                            validPositions.Add(node.position);
                        }
                    }
                }
                else
                {
                    // Code for side movement
                    if (node.position == position - pawnDirection * team && node.nodeUnit == null)
                    {
                        validPositions.Add(node.position);
                    }
                    else if (node.position == position + pawnDirection * team && node.nodeUnit == null)
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
        MovePawn(destination);
    }

    //TODO: Rotate around edge mesh node;
    private void MovePawn(Vector3 destination)
    {
        Vector3 p = UnitSpawnPoint.GetAdjustedSpawnPosition(0.5f, destination, 
            UnitSpawnPoint.GetNearestNode(destination, 1, true).transform.position);

        // Handle rotation
        AlignUnit(gameObject, destination);

        // Set nodes
        currentNode.SetNodeUnit(null);
        UnitSpawnPoint.GetNearestNode(destination).SetNodeUnit(this);
        currentNode = UnitSpawnPoint.GetNearestNode(destination);
        unAdjustedPosition = destination;

        // Actually move
        StartCoroutine(Move(transform.position, p, 0.5f));
    }
}