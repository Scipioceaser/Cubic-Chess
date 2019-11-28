using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Unit
{
    private float diagonalLine_length = Mathf.Sqrt(2);
    private float sidewaysDiagonalLine_Length = Mathf.Sqrt(1 + (Mathf.Sqrt(2) * Mathf.Sqrt(2)));

    public override void Awake()
    {
        base.Awake();

        unAdjustedPosition = transform.position;
        transform.position = GetAdjustedSpawnPosition(0.5f, transform.localPosition, GetNearestNodeObject(transform.position, 2, true).transform.position);
        currentNode = GetNearestNode(transform.position);
        currentNode.SetNodeUnit(this);
        AlignUnit(currentNode.position);
    }

    public override List<Vector3> GetValidMovePositions(Vector3 position, int team = 1)
    {
        List<Vector3> validPositions = new List<Vector3>();
        List<Node> nearbyNodes = map.GetNeighbours(currentNode, Globals.mapSize + 1);

        foreach (Node node in nearbyNodes)
        {
            if (node.GetType() != typeof(NodeMesh))
            {
                if (node.position.x == 0 && node.position.z == 0 || node.position.x == 0 && node.position.z == Globals.mapSize + 1
                        || node.position.x == Globals.mapSize + 1 && node.position.z == 0 || node.position.x == Globals.mapSize + 1 && node.position.z == Globals.mapSize + 1)
                {
                    continue;
                }

                if (IsNodeAtEmptyEdge(node.position))
                {
                    continue;
                }

                if (node.nodeUnit != null)
                {
                    if (node.nodeUnit.unitTeam == unitTeam)
                    {
                        continue;
                    }
                }

                if (unAdjustedPosition.y == 0 || unAdjustedPosition.y == Globals.mapHeight + 1)
                {
                    if (node.position.y == unAdjustedPosition.y)
                    {
                        if (node.nodeUnit == null)
                        {
                            float d = Vector3.Distance(node.position, position);

                            if (d == 1 || d == diagonalLine_length)
                            {
                                validPositions.Add(node.position);
                            }
                        }
                    }
                    else if (node.position.y == unAdjustedPosition.y - 1 || node.position.y == unAdjustedPosition.y + 1)
                    {
                        float d = Vector3.Distance(node.position, position);
                        if (d == diagonalLine_length || d == sidewaysDiagonalLine_Length)
                        {
                            validPositions.Add(node.position);
                        }
                    }
                }
                else
                {
                    if (node.position.y == Globals.mapHeight + 1 || node.position.y == 0)
                    {
                        float d = Vector3.Distance(node.position, position);
                        if (d == diagonalLine_length || d == sidewaysDiagonalLine_Length)
                        {
                            validPositions.Add(node.position);
                        }
                    }
                    else if (node.position.y == Globals.mapHeight + 1 && node.position != position + Vector3.up || node.position.y == 0 && node.position != position - Vector3.up)
                    {
                        // These nodes aren't right need to keep this so that it doesn't just use the side of the board movement code
                        continue;
                    }
                    else
                    {
                        float d = Vector3.Distance(position, node.position);

                        if (d == 1 || d == diagonalLine_length || d == sidewaysDiagonalLine_Length)
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
        MoveKing(destination);
    }

    private void MoveKing(Vector3 destination)
    {
        Vector3 p = GetAdjustedSpawnPosition(0.5f, destination,
            GetNearestNode(destination, 1, true).transform.position);

        // Handle rotation
        AlignUnit(destination);

        // Set nodes
        currentNode.SetNodeUnit(null);
        GetNearestNode(destination).SetNodeUnit(this);
        currentNode = GetNearestNode(destination);
        unAdjustedPosition = destination;

        // Actually move
        StartCoroutine(Move(transform.position, p, 0.5f));
    }
}
