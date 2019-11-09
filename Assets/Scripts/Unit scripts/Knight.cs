﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Unit
{
    float upDownLineLength = Mathf.Sqrt(10);
    float sidewaysLineLength = Mathf.Sqrt(5);

    public override void Awake()
    {
        base.Awake();
        SetModelFromAssets(gameObject, "Knight", "Knight", "Outline");
        currentNode = UnitSpawnPoint.GetNearestNode(transform.position);
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
                    continue;

                if (node.position.x == unAdjustedPosition.x || node.position.z == unAdjustedPosition.z || node.position.y == unAdjustedPosition.y)
                {
                    if (Vector3.Distance(node.position, position) == sidewaysLineLength)
                    {
                        validPositions.Add(node.position);
                    }
                }
                else
                {
                    if (Vector3.Distance(node.position, position) == upDownLineLength)
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
        MoveKnight(destination);
    }

    private void MoveKnight(Vector3 destination)
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