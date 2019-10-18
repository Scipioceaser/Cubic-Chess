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

    public override List<Vector3> GetValidMovePositions(Vector3 position, int team = 1)
    {
        List<Vector3> validPositions = new List<Vector3>();
        List<Node> nearbyNodes = map.GetNeighbours(currentNode, 3);

        foreach (Node node in nearbyNodes)
        {
            if (node.GetType() != typeof(NodeMesh))
            {
                float d = Vector3.Distance(node.position, position);

                if (node.position.y == 0 || node.position.y == Globals.mapHeight + 1)
                {

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
