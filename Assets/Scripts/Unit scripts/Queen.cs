using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Unit
{
    private float diagonalLine_length = Mathf.Sqrt(2);
    private float sidewaysDiagonalLine_Length = Mathf.Sqrt(1 + (Mathf.Sqrt(2) * Mathf.Sqrt(2)));

    private ParticleSystem confetti;

    public override void Awake()
    {
        base.Awake();

        confetti = GetComponent<ParticleSystem>();

        unAdjustedPosition = transform.position;
        transform.position = GetAdjustedSpawnPosition(0.5f, transform.localPosition, GetNearestNodeObject(transform.position, 2, true).transform.position);
        currentNode = GetNearestNode(unAdjustedPosition);
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

                if (node.nodeUnit != null)
                {
                    if (node.nodeUnit.unitTeam == this.unitTeam)
                    {
                        continue;
                    }
                }

                if (IsNodeAtEmptyEdge(node.position))
                    continue;

                if (unAdjustedPosition.y == 0 || unAdjustedPosition.y == Globals.mapHeight + 1)
                {
                    if (node.position.y == unAdjustedPosition.y)
                    {
                        if (IsNodeAtEmptyEdge(node.position))
                            continue;

                        if (IsLineStraight(position.x, node.position.x, position.z, node.position.z) && LineDelta(position.x, node.position.x) > 0)
                        {
                            if (node.nodeUnit == null && !EnemyInFrontOfNode(position, node.position))
                            {
                                validPositions.Add(node.position);
                            }
                            else if (node.nodeUnit != null && node.nodeUnit.unitTeam != unitTeam)
                            {
                                meshCol.isTrigger = true;
                                if (!node.nodeUnit.EnemyInFrontOfNode(node.nodeUnit.unAdjustedPosition, unAdjustedPosition))
                                {
                                    validPositions.Add(node.position);
                                    meshCol.isTrigger = false;
                                }
                            }
                        }

                        if (position.x != node.position.x && position.z == node.position.z || position.x == node.position.x && position.z != node.position.z)
                        {
                            if (node.nodeUnit == null && !EnemyInFrontOfNode(position, node.position))
                            {
                                validPositions.Add(node.position);
                            }
                            else if (node.nodeUnit != null && node.nodeUnit.unitTeam != unitTeam)
                            {
                                meshCol.isTrigger = true;
                                if (!node.nodeUnit.EnemyInFrontOfNode(node.nodeUnit.unAdjustedPosition, unAdjustedPosition))
                                {
                                    validPositions.Add(node.position);
                                    meshCol.isTrigger = false;
                                }
                            }
                        }
                    }
                    else if (node.position.y == unAdjustedPosition.y - 1 || node.position.y == unAdjustedPosition.y + 1)
                    {
                        if (IsNodeAtEmptyEdge(node.position))
                            continue;

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
                        if (IsNodeAtEmptyEdge(node.position))
                            continue;

                        float d = Vector3.Distance(node.position, position);
                        if (d == diagonalLine_length || d == sidewaysDiagonalLine_Length)
                        {
                            validPositions.Add(node.position);
                        }
                    }
                    else
                    {
                        if (IsNodeAtEmptyEdge(node.position))
                            continue;

                        if (position.x == Globals.mapSize + 1 && position.x == node.position.x
                            || position.x == 0 && position.x == node.position.x)
                        {
                            if (position.z != node.position.z && position.y == node.position.y || position.z == node.position.z && position.y != node.position.y)
                            {
                                if (node.nodeUnit == null && !EnemyInFrontOfNode(position, node.position))
                                {
                                    validPositions.Add(node.position);
                                }
                                else if (node.nodeUnit != null && node.nodeUnit.unitTeam != unitTeam)
                                {
                                    meshCol.isTrigger = true;
                                    if (!node.nodeUnit.EnemyInFrontOfNode(node.nodeUnit.unAdjustedPosition, unAdjustedPosition))
                                    {
                                        validPositions.Add(node.position);
                                        meshCol.isTrigger = false;
                                    }
                                }
                            }

                            if (IsLineStraight(position.z, node.position.z, position.y, node.position.y) && LineDelta(position.z, node.position.z) > 0)
                            {
                                if (node.nodeUnit == null && !EnemyInFrontOfNode(position, node.position))
                                {
                                    validPositions.Add(node.position);
                                }
                                else if (node.nodeUnit != null && node.nodeUnit.unitTeam != unitTeam)
                                {
                                    meshCol.isTrigger = true;
                                    if (!node.nodeUnit.EnemyInFrontOfNode(node.nodeUnit.unAdjustedPosition, unAdjustedPosition))
                                    {
                                        validPositions.Add(node.position);
                                        meshCol.isTrigger = false;
                                    }
                                }
                            }
                        }
                        else if (position.z == Globals.mapSize + 1 && position.z == node.position.z
                            || position.z == 0 && position.z == node.position.z)
                        {
                            if (position.x != node.position.x && position.y == node.position.y || position.x == node.position.x && position.y != node.position.y)
                            {
                                if (node.nodeUnit == null && !EnemyInFrontOfNode(position, node.position))
                                {
                                    validPositions.Add(node.position);
                                }
                                else if (node.nodeUnit != null && node.nodeUnit.unitTeam != unitTeam)
                                {
                                    meshCol.isTrigger = true;
                                    if (!node.nodeUnit.EnemyInFrontOfNode(node.nodeUnit.unAdjustedPosition, unAdjustedPosition))
                                    {
                                        validPositions.Add(node.position);
                                        meshCol.isTrigger = false;
                                    }
                                }
                            }

                            if (IsLineStraight(position.x, node.position.x, position.y, node.position.y) && LineDelta(position.x, node.position.x) > 0)
                            {
                                if (node.nodeUnit == null && !EnemyInFrontOfNode(position, node.position))
                                {
                                    validPositions.Add(node.position);
                                }
                                else if (node.nodeUnit != null && node.nodeUnit.unitTeam != unitTeam)
                                {
                                    meshCol.isTrigger = true;
                                    if (!node.nodeUnit.EnemyInFrontOfNode(node.nodeUnit.unAdjustedPosition, unAdjustedPosition))
                                    {
                                        validPositions.Add(node.position);
                                        meshCol.isTrigger = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            float d = Vector3.Distance(node.position, position);
                            if (d == diagonalLine_length || d == sidewaysDiagonalLine_Length)
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

    public void PlayConfetti()
    {
        confetti.Play();
    }

    public override void MoveAlongPath(Vector3 destination = new Vector3(), bool changeState = true)
    {
        Vector3 p = GetAdjustedSpawnPosition(0.5f, destination,
                GetNearestNode(destination, 1, true).transform.position);

        // Handle rotation
        AlignUnit(destination);

        // Record the position for undo function
        lastPosition = unAdjustedPosition;

        // Set nodes
        currentNode.SetNodeUnit(null);
        GetNearestNode(destination).SetNodeUnit(this);
        currentNode = GetNearestNode(destination);
        unAdjustedPosition = destination;

        // Actually move
        StartCoroutine(Move(transform.position, p, 0.5f));
    }
}
