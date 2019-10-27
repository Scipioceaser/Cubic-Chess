using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Unit
{
    // Magic number for diagonal lines
    private float singleDiagonalLine_Length = Mathf.Sqrt(1 + (Mathf.Sqrt(2) * Mathf.Sqrt(2)));

    public override void Awake()
    {
        base.Awake();
        SetModelFromAssets(gameObject, "bishop", "bishop", "Outline");
        currentNode = UnitSpawnPoint.GetNearestNode(transform.position);
        currentNode.SetNodeUnit(this);
        AlignUnit(currentNode.position);
    }
    
    public override List<Vector3> GetValidMovePositions(Vector3 position, int team = 1)
    {
        List<Vector3> validPositions = new List<Vector3>();
        List<Node> nearbyNodes = map.GetNeighbours(currentNode, 4);

        foreach (Node node in nearbyNodes)
        {
            if (node.GetType() != typeof(NodeMesh))
            {
                if (node.position.x == 0 && node.position.z == 0 || node.position.x == 0 && node.position.z == Globals.mapSize + 1
                        || node.position.x == Globals.mapSize + 1 && node.position.z == 0 || node.position.x == Globals.mapSize + 1 && node.position.z == Globals.mapSize + 1)
                {
                    continue;
                }

                // Check unit height
                if (unAdjustedPosition.y == Globals.mapHeight + 1 || unAdjustedPosition.y == 0)
                {
                    if (node.position.y == unAdjustedPosition.y)
                    {
                        if (node.position.x == 0 || node.position.x == Globals.mapSize + 1 || node.position.z == 0 || node.position.z == Globals.mapSize + 1)
                        {
                            continue;
                        }

                        if (node.nodeUnit == null)
                        {
                            if (IsLineStraight(position.x, node.position.x, position.z, node.position.z) && LineDelta(position.x, node.position.x) > 0)
                            {
                                validPositions.Add(node.position);
                            }
                        }
                    }
                    else if (node.position.y == unAdjustedPosition.y - 1 || node.position.y == unAdjustedPosition.y + 1)
                    {
                        if (Vector3.Distance(node.position, position) == singleDiagonalLine_Length)
                        {
                            validPositions.Add(node.position);
                        }
                    }
                }
                else
                {
                    if (node.position.y == Globals.mapHeight + 1 && node.position == position + Vector3.up || node.position.y == 0 && node.position == position - Vector3.up)
                    {
                        foreach (Vector3 vector in GetDiagonalNodeNeigbours(node.position))
                        {
                            validPositions.Add(vector);
                        }
                    }
                    else if (node.position.y == Globals.mapHeight + 1 && node.position != position + Vector3.up || node.position.y == 0 && node.position != position - Vector3.up)
                    {
                        // These nodes aren't right need to keep this so that it doesn't just use the side of the board movement code
                        continue;
                    }
                    else if (node.position.y == unAdjustedPosition.y + 1 && Vector3.Distance(node.position, position) == singleDiagonalLine_Length
                        || node.position.y == unAdjustedPosition.y - 1 && Vector3.Distance(node.position, position) == singleDiagonalLine_Length)
                    {
                        validPositions.Add(node.position);
                    }
                    else
                    {
                        if (position.x == node.position.x)
                        {
                            if (IsLineStraight(position.z, node.position.z, position.y, node.position.y) && LineDelta(position.z, node.position.z) > 0)
                            {
                                validPositions.Add(node.position);
                            }
                        }
                        else if (position.z == node.position.z)
                        {
                            if (IsLineStraight(position.x, node.position.x, position.y, node.position.y) && LineDelta(position.x, node.position.x) > 0)
                            {
                                validPositions.Add(node.position);
                            }
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

    public List<Vector3> GetDiagonalNodeNeigbours(Vector3 nodePosition)
    {
        Node node = map.NodeFromNodeVector(nodePosition);
        List<Node> neighbours = map.GetNeighbours(node);
        List<Vector3> positions = new List<Vector3>();

        foreach (Node nearbyNode in neighbours)
        {
            if (nearbyNode.GetType() != typeof(NodeMesh) 
                && IsLineStraight(nodePosition.x, nearbyNode.position.x, nodePosition.z, nearbyNode.position.z) 
                && LineDelta(nodePosition.x, nearbyNode.position.x) > 0
                && Vector3.Distance(nearbyNode.position, UnitSpawnPoint.GetNearestNode(nearbyNode.position, 1, true).position) == 1f
                && nearbyNode.position.y != unAdjustedPosition.y)
            {
                positions.Add(nearbyNode.position);
            }
        }

        return positions;
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
