using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Unit
{
    public override void Awake()
    {
        base.Awake();
        SetModelFromAssets(gameObject, "bishop", "bishop");
        currentNode = UnitSpawnPoint.GetNearestNode(transform.position);
        currentNode.SetNodeUnit(this);
        AlignUnit(currentNode.position);
    }

    //TODO: Look into problems with dead corners
    public override List<Vector3> GetValidMovePositions(Vector3 position, int team = 1)
    {
        List<Vector3> validPositions = new List<Vector3>();
        List<Node> nearbyNodes = map.GetNeighbours(currentNode, 3);

        foreach (Node node in nearbyNodes)
        {
            if (node.GetType() != typeof(NodeMesh))
            {
                if (node.position.x == 0 && node.position.z == 0 || node.position.x == 0 && node.position.z == Globals.mapSize + 1
                        || node.position.x == Globals.mapSize + 1 && node.position.z == 0 || node.position.x == Globals.mapSize + 1 && node.position.z == Globals.mapSize + 1)
                {
                    continue;
                }

                float d = Vector3.Distance(node.position, position);

                if (node.position.y == 0 || node.position.y == Globals.mapHeight + 1)
                {
                    if (node.position.y == unAdjustedPosition.y)
                    {
                        if (node.position.x == 0 || node.position.x == Globals.mapSize + 1 || node.position.z == 0 || node.position.z == Globals.mapSize + 1)
                        {
                            continue;
                        }

                        if (node.position.y == 0)
                        {
                            // Movement along the bottom
                            if (node.nodeUnit == null)
                            {
                                if (node.position == position + (Vector3.forward + Vector3.right) * Mathf.Floor(d)
                                    || node.position == position + (Vector3.forward - Vector3.right) * Mathf.Floor(d))
                                {
                                    
                                    validPositions.Add(node.position);
                                    
                                }
                                else if (node.position == position - (Vector3.forward + Vector3.right) * Mathf.Floor(d)
                                   || node.position == position - (Vector3.forward - Vector3.right) * Mathf.Floor(d))
                                {
                                    validPositions.Add(node.position);
                                }
                            }
                        }
                        else if (node.position.y == Globals.mapHeight + 1)
                        {
                            // Movement along the top and bottom
                            if (node.nodeUnit == null)
                            {
                                if (node.position == position + (Vector3.forward + Vector3.right) * Mathf.Floor(d)
                                    || node.position == position + (Vector3.forward - Vector3.right) * Mathf.Floor(d))
                                {
                                    validPositions.Add(node.position);
                                }
                                else if (node.position == position - (Vector3.forward + Vector3.right) * Mathf.Floor(d)
                                   || node.position == position - (Vector3.forward - Vector3.right) * Mathf.Floor(d))
                                {
                                    validPositions.Add(node.position);
                                }
                            }
                        }
                    }
                    else if (node.position == position - Vector3.up + Vector3.forward || node.position == position - Vector3.up - Vector3.forward)
                    {
                        validPositions.Add(node.position - Vector3.right);
                    }
                    else if (node.position == position - Vector3.up + Vector3.right || node.position == position - Vector3.up - Vector3.right)
                    {
                        validPositions.Add(node.position - Vector3.forward);
                    }
                    else if (node.position == position + Vector3.up + Vector3.forward || node.position == position + Vector3.up - Vector3.forward)
                    {
                        validPositions.Add(node.position - Vector3.right);
                    }
                    else if (node.position == position + Vector3.up + Vector3.right || node.position == position + Vector3.up - Vector3.right)
                    {
                        validPositions.Add(node.position - Vector3.forward);
                    }
                }
                else
                {
                    if (unAdjustedPosition.y == 0 || unAdjustedPosition.y == Globals.mapHeight + 1)
                    {
                        
                        if (node.position == position - Vector3.up + Vector3.forward || node.position == position - Vector3.up - Vector3.forward)
                        {
                            if ((node.position + Vector3.right).x == 0 && (node.position + Vector3.right).z == 0 || (node.position + Vector3.right).x == 0 && (node.position + Vector3.right).z == Globals.mapSize + 1
                            || (node.position + Vector3.right).x == Globals.mapSize + 1 && (node.position + Vector3.right).z == 0 || (node.position + Vector3.right).x == Globals.mapSize + 1 && (node.position + Vector3.right).z == Globals.mapSize + 1)
                            {
                                continue;
                            }
                            else
                            {
                                validPositions.Add(node.position + Vector3.right);
                            }

                            if ((node.position - Vector3.right).x == 0 && (node.position - Vector3.right).z == 0 || (node.position - Vector3.right).x == 0 && (node.position - Vector3.right).z == Globals.mapSize + 1
                            || (node.position - Vector3.right).x == Globals.mapSize + 1 && (node.position - Vector3.right).z == 0 || (node.position - Vector3.right).x == Globals.mapSize + 1 && (node.position - Vector3.right).z == Globals.mapSize + 1)
                            {
                                continue;
                            }
                            else
                            {
                                validPositions.Add(node.position - Vector3.right);
                            }
                        }
                        else if (node.position == position - Vector3.up + Vector3.right || node.position == position - Vector3.up - Vector3.right)
                        {
                            if ((node.position + Vector3.forward).x == 0 && (node.position + Vector3.forward).z == 0 || (node.position + Vector3.forward).x == 0 && (node.position + Vector3.forward).z == Globals.mapSize + 1
                            || (node.position + Vector3.forward).x == Globals.mapSize + 1 && (node.position + Vector3.forward).z == 0 || (node.position + Vector3.forward).x == Globals.mapSize + 1 && (node.position + Vector3.forward).z == Globals.mapSize + 1)
                            {
                                continue;
                            }
                            else
                            {
                                validPositions.Add(node.position + Vector3.forward);
                            }

                            if ((node.position - Vector3.forward).x == 0 && (node.position - Vector3.forward).z == 0 || (node.position - Vector3.forward).x == 0 && (node.position - Vector3.forward).z == Globals.mapSize + 1
                            || (node.position - Vector3.forward).x == Globals.mapSize + 1 && (node.position - Vector3.forward).z == 0 || (node.position - Vector3.forward).x == Globals.mapSize + 1 && (node.position - Vector3.forward).z == Globals.mapSize + 1)
                            {
                                continue;
                            }
                            else
                            {
                                validPositions.Add(node.position - Vector3.forward);
                            }
                        }
                        else if (node.position == position + Vector3.up + Vector3.forward || node.position == position + Vector3.up - Vector3.forward)
                        {
                            if ((node.position + Vector3.right).x == 0 && (node.position + Vector3.right).z == 0 || (node.position + Vector3.right).x == 0 && (node.position + Vector3.right).z == Globals.mapSize + 1
                            || (node.position + Vector3.right).x == Globals.mapSize + 1 && (node.position + Vector3.right).z == 0 || (node.position + Vector3.right).x == Globals.mapSize + 1 && (node.position + Vector3.right).z == Globals.mapSize + 1)
                            {
                                continue;
                            }
                            else
                            {
                                validPositions.Add(node.position + Vector3.right);
                            }

                            if ((node.position - Vector3.right).x == 0 && (node.position - Vector3.right).z == 0 || (node.position - Vector3.right).x == 0 && (node.position - Vector3.right).z == Globals.mapSize + 1
                            || (node.position - Vector3.right).x == Globals.mapSize + 1 && (node.position - Vector3.right).z == 0 || (node.position - Vector3.right).x == Globals.mapSize + 1 && (node.position - Vector3.right).z == Globals.mapSize + 1)
                            {
                                continue;
                            }
                            else
                            {
                                validPositions.Add(node.position - Vector3.right);
                            }
                        }
                        else if (node.position == position + Vector3.up + Vector3.right || node.position == position + Vector3.up - Vector3.right)
                        {
                            if ((node.position + Vector3.forward).x == 0 && (node.position + Vector3.forward).z == 0 || (node.position + Vector3.forward).x == 0 && (node.position + Vector3.forward).z == Globals.mapSize + 1
                            || (node.position + Vector3.forward).x == Globals.mapSize + 1 && (node.position + Vector3.forward).z == 0 || (node.position + Vector3.forward).x == Globals.mapSize + 1 && (node.position + Vector3.forward).z == Globals.mapSize + 1)
                            {
                                continue;
                            }
                            else
                            {
                                validPositions.Add(node.position + Vector3.forward);
                            }

                            if ((node.position - Vector3.forward).x == 0 && (node.position - Vector3.forward).z == 0 || (node.position - Vector3.forward).x == 0 && (node.position - Vector3.forward).z == Globals.mapSize + 1
                            || (node.position - Vector3.forward).x == Globals.mapSize + 1 && (node.position - Vector3.forward).z == 0 || (node.position - Vector3.forward).x == Globals.mapSize + 1 && (node.position - Vector3.forward).z == Globals.mapSize + 1)
                            {
                                continue;
                            }
                            else
                            {
                                validPositions.Add(node.position - Vector3.forward);
                            }
                        }
                    }
                    else
                    {
                        if (node.position.x == 0 && node.position.z == 0 || node.position.x == 0 && node.position.z == Globals.mapSize + 1
                        || node.position.x == Globals.mapSize + 1 && node.position.z == 0 || node.position.x == Globals.mapSize + 1 && node.position.z == Globals.mapSize + 1)
                        {
                            continue;
                        }

                        if (node.position == position + (Vector3.up + Vector3.forward) * Mathf.Floor(d) || node.position == position - (Vector3.up + Vector3.forward) * Mathf.Floor(d)
                        || node.position == position + (Vector3.up - Vector3.forward) * Mathf.Floor(d) || node.position == position - (Vector3.up - Vector3.forward) * Mathf.Floor(d))
                        {
                            validPositions.Add(node.position);
                        }
                        else if (node.position == position + (Vector3.up + Vector3.right) * Mathf.Floor(d) || node.position == position - (Vector3.up + Vector3.right) * Mathf.Floor(d)
                           || node.position == position + (Vector3.up - Vector3.right) * Mathf.Floor(d) || node.position == position - (Vector3.up - Vector3.right) * Mathf.Floor(d))
                        {
                            validPositions.Add(node.position);
                        }
                    }
                }
            }
        }
    
        return validPositions;
    }

    public override void MoveAlongPath(Vector3 destination = new Vector3())
    {
        MoveBishop(destination);
    }

    private void MoveBishop(Vector3 destination)
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
