using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Unit
{
    public Vector3 horizontalMoveDirection;

    public override void Awake()
    {
        base.Awake();
        
        SetModelFromAssets(gameObject, "pawn", "pawn", "Outline");
        currentNode = UnitSpawnPoint.GetNearestNode(transform.position);
        currentNode.SetNodeUnit(this);
        AlignUnit(currentNode.position);
    }

    public override List<Vector3> GetValidMovePositions(Vector3 position, int team = 1)
    {
        List<Vector3> validPositions = new List<Vector3>();
        List<Node> nearbyNodes = map.GetNeighbours(map.NodeFromWorldPoints(position), 2);

        foreach (Node node in nearbyNodes)
        {
            if (node.GetType() != typeof(NodeMesh))
            {
                if (node.position.y == 0 || node.position.y == Globals.mapHeight + 1)
                {
                    if (node.position.y == unAdjustedPosition.y)
                    {
                        // Horizontal movement
                        if (node.position == position + horizontalMoveDirection || node.position == position - horizontalMoveDirection)
                        {
                            if (node.position.x == 0 || node.position.x == Globals.mapSize + 1 
                                || node.position.z == 0 || node.position.z == Globals.mapSize + 1)
                            {
                                if (unAdjustedPosition.y == 0)
                                {
                                    validPositions.Add(node.position + Vector3.up);
                                }
                                else if (unAdjustedPosition.y == Globals.mapHeight + 1)
                                {
                                    validPositions.Add(node.position - Vector3.up);
                                }
                            }

                            if (node.position == position + horizontalMoveDirection || node.position == position - horizontalMoveDirection)
                            {
                                validPositions.Add(node.position);
                            }
                        }
                    }
                    else if (node.position == position + Vector3.up + horizontalMoveDirection
                            || node.position == position + Vector3.up - horizontalMoveDirection)
                    {
                        // Going up along edge
                        validPositions.Add(node.position);
                    }
                    else if (node.position == position - Vector3.up + horizontalMoveDirection
                            || node.position == position - Vector3.up - horizontalMoveDirection)
                    {
                        // Going down along edge
                        validPositions.Add(node.position);
                    }
                }
                else
                {
                    if (node.position == position - Vector3.up * team && node.nodeUnit == null)
                    {
                        validPositions.Add(node.position);
                    }
                    else if (node.position == position + Vector3.up * team && node.nodeUnit == null)
                    {
                        validPositions.Add(node.position);
                    }
                }
            }
        }

        return validPositions;
    }

    // TODO: Make pawn rotate around edge
    public List<Vector3> GetValidMovePositionsOld(Vector3 position, int team = 1)
    {
        List<Vector3> validPositions = new List<Vector3>();
        List<Node> nearbyNodes = map.GetNeighbours(map.NodeFromWorldPoints(position), 1);
        
        if (unAdjustedPosition.y == 0 || unAdjustedPosition.y == Globals.mapHeight + 1)
        {
            moveDirection = Vector3.forward;
        }
        else
        {
            moveDirection = Vector3.up;
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
                                if (node.position == position + moveDirection)
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
                                else if (node.position == position - moveDirection)
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
                                if (node.position == position + moveDirection)
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
                                else if (node.position == position - moveDirection)
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
                        
                        if (position.x == 0 && position.z != 0 && position.z != Globals.mapSize + 1
                            || position.x == Globals.mapSize + 1 && position.z == 0 && position.z == Globals.mapSize + 1)
                        {
                            if (node.position == position - Vector3.right + Vector3.up
                            || node.position == position + Vector3.right + Vector3.up)
                            {
                                Debug.DrawLine(position, node.position, Color.green);
                                validPositions.Add(node.position);
                            }
                        }
                        else
                        {
                            if (node.position == position - Vector3.forward + Vector3.up
                            || node.position == position + Vector3.forward + Vector3.up)
                            {
                                Debug.DrawLine(position, node.position, Color.red);
                                validPositions.Add(node.position);
                            }
                        }
                    }
                    else if (node.position.y == unAdjustedPosition.y - 1)
                    {
                        // Going down
                        if (node.position == position - Vector3.up)
                        {
                            //validPositions.Add(node.position);
                        }

                        if (position.x == 0 && position.z != 0 && position.z != Globals.mapSize + 1
                            || position.x == Globals.mapSize + 1 && position.z == 0 && position.z == Globals.mapSize + 1)
                        {
                            if (node.position == position - Vector3.right + Vector3.up
                            || node.position == position + Vector3.right + Vector3.up)
                            {
                                Debug.DrawLine(position, node.position, Color.green);
                                validPositions.Add(node.position);
                            }
                        }
                        else
                        {
                            if (node.position == position - Vector3.forward + Vector3.up
                            || node.position == position + Vector3.forward + Vector3.up)
                            {
                                Debug.DrawLine(position, node.position, Color.red);
                                validPositions.Add(node.position);
                            }
                        }
                    }
                }
                else
                {
                    // Code for side movement
                    if (node.position == position - Vector3.up * team && node.nodeUnit == null)
                    {
                        validPositions.Add(node.position);
                    }
                    else if (node.position == position + Vector3.up * team && node.nodeUnit == null)
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