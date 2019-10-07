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

        // TODO: Units on the top or the bottom can't go down the sides.
        foreach (Node node in nearbyNodes)
        {
            if (node.GetType() != typeof(NodeMesh))
            {
                if (node.position.y == 0 || node.position.y == (Globals.mapHeight + 1))
                {
                    // Check if lateral board movement
                    if (node.position.y == unAdjustedPosition.y)
                    {
                        // Movement along the top
                        if (node.position == position + transform.forward)
                        {
                            validPositions.Add(node.position);
                        }
                        else if (node.position == position - transform.forward)
                        {
                            validPositions.Add(node.position);
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

        //print(validPositions.Count);

        return validPositions;
    }

    public override void MoveAlongPath(Vector3 destination = new Vector3())
    {
        //TODO: Add loop to move along all points
        MovePawn(destination);
    }

    private void MovePawn(Vector3 destination)
    {
        Vector3 p = UnitSpawnPoint.GetAdjustedSpawnPosition(0.5f, destination, 
            UnitSpawnPoint.GetNearestNode(destination, 1, true).transform.position);

        currentNode.SetNodeUnit(null);
        UnitSpawnPoint.GetNearestNode(destination).SetNodeUnit(this);
        currentNode = UnitSpawnPoint.GetNearestNode(destination);
        unAdjustedPosition = destination;
        StartCoroutine(Move(transform.position, p, 0.5f));
    }
}
    /*if (unAdjustedPosition.x != 0 && unAdjustedPosition.z != 0 && unAdjustedPosition.x != (Globals.mapSize + 1) && unAdjustedPosition.z != (Globals.mapSize + 1))
                    {
                        // Lateral top-down movement
                        // The check for direction may be a problem when the units start rotating
                        if (spawnDir == Direction.UP || spawnDir == Direction.DOWN)
                        {
                            if (node.position == position - transform.right * team && node.nodeUnit == null)
                            {
                                validPositions.Add(node.position);
                            }
                            else if (node.position == position + transform.right * team && node.nodeUnit == null)
                            {
                                validPositions.Add(node.position);
                            }
                        }
                        else
                        {
                            if (node.position == position - transform.forward * team && node.nodeUnit == null)
                            {
                                Debug.DrawLine(position, node.position, Color.blue);
                                validPositions.Add(node.position);
                            }
                            else if (node.position == position + transform.forward * team && node.nodeUnit == null)
                            {
                                Debug.DrawLine(position, node.position, Color.red);
                                validPositions.Add(node.position);
                            }
                        }
                    }
                    else if (unAdjustedPosition.x == 1 || unAdjustedPosition.z == 1 || unAdjustedPosition.x == Globals.mapSize || unAdjustedPosition.z == Globals.mapSize || unAdjustedPosition.x == 1 
                        || unAdjustedPosition.z == 1 || unAdjustedPosition.x == Globals.mapSize || unAdjustedPosition.z == Globals.mapSize)
                    {
                        if (unAdjustedPosition.y == 0 || unAdjustedPosition.y == Globals.mapHeight + 1)
                        {
                            Debug.DrawLine(unAdjustedPosition, node.position);
                        }
                    }
                    else
                    {
                        if (position.y == node.position.y)
                        {
                            // Down movement
                            if (node.position.y == 1)
                            {
                                if (node.position == position - Vector3.up)
                                {
                                    validPositions.Add(node.position);
                                }

                                if (node.position == position - transform.forward - Vector3.up)
                                {
                                    validPositions.Add(node.position);
                                }
                            }
                            else if (node.position.y == Globals.mapHeight)
                            {
                                if (node.position == position + Vector3.up)
                                {
                                    validPositions.Add(node.position);
                                }

                                if (node.position == position - transform.forward + Vector3.up)
                                {
                                    validPositions.Add(node.position);
                                }
                            }
                        }
                        else if (position.y < node.position.y)
                        {
                            if (node.position == position + Vector3.up)
                            {
                                validPositions.Add(node.position);
                            }

                            if (node.position == position - transform.forward + Vector3.up)
                            {
                                validPositions.Add(node.position);
                            }
                        }
                        else if (position.y > node.position.y)
                        {
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
     */