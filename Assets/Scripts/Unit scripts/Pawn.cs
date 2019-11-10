using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Unit
{
    public Vector3 horizontalMoveDirection;
    private Vector3 verticalMoveDir = Vector3.down;

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
                Debug.DrawRay(position, horizontalMoveDirection);
                Debug.DrawRay(position, verticalMoveDir);

                if (node.position.x == 0 && node.position.z == 0 || node.position.x == 0 && node.position.z == Globals.mapSize + 1
                       || node.position.x == Globals.mapSize + 1 && node.position.z == 0 || node.position.x == Globals.mapSize + 1 && node.position.z == Globals.mapSize + 1)
                {
                    continue;
                }

                if (IsNodeAtEmptyEdge(node.position))
                    continue;

                if (unAdjustedPosition.y == 0 || unAdjustedPosition.y == Globals.mapHeight + 1)
                {
                    float d = Vector3.Distance(node.position, unAdjustedPosition);
                    if (node.nodeUnit != null)
                    {
                        if (node.nodeUnit.unitTeam == this.unitTeam)
                             continue;
                    }
                    
                    if (node.position == position + horizontalMoveDirection)
                    {
                        validPositions.Add(node.position);
                    }
                    else if (d == Mathf.Sqrt(2) && node.nodeUnit != null)
                    {
                        if (node.nodeUnit.unitTeam == unitTeam)
                            validPositions.Add(node.position);
                    }
                    else if (d == Mathf.Sqrt(2) && Mathf.Abs(unAdjustedPosition.y - node.position.y) == 1 && node.nodeUnit == null)
                    {

                        if (unAdjustedPosition.y == 0)
                        {
                            verticalMoveDir = Vector3.up;
                        }
                        else if (unAdjustedPosition.y == Globals.mapHeight + 1)
                        {
                            verticalMoveDir = Vector3.down;
                        }
                        
                        validPositions.Add(node.position);
                    }
                }
                else
                {
                    float d = Vector3.Distance(node.position, unAdjustedPosition);
                    if (node.nodeUnit != null)
                    {
                        if (node.nodeUnit.unitTeam == this.unitTeam)
                            continue;
                    }

                    if (node.position == position + verticalMoveDir)
                    {
                        validPositions.Add(node.position);
                    }
                    else if (d == Mathf.Sqrt(2) && node.nodeUnit != null)
                    {
                        if (node.nodeUnit.unitTeam == unitTeam)
                            validPositions.Add(node.position);
                    }
                    else if (node.position.y == 0 || node.position.y == Globals.mapHeight + 1)
                    {
                        horizontalMoveDirection = -horizontalMoveDirection;

                        if (d == Mathf.Sqrt(2) && node.nodeUnit == null)
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