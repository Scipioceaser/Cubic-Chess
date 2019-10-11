using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Unit
{
    public override void Awake()
    {
        base.Awake();
        SetModelFromAssets(gameObject, "pawn", "pawn");
        currentNode = UnitSpawnPoint.GetNearestNode(transform.position);
        currentNode.SetNodeUnit(this);
    }

    //TODO: Limit movement when enemies in the path of the rook
    public override List<Vector3> GetValidMovePositions(Vector3 position, int team = 1)
    {
        List<Vector3> validPositions = new List<Vector3>();
        List<Node> nearbyNodes = map.GetNeighbours(currentNode, 2);

        foreach (Node node in nearbyNodes)
        {
            if (node.GetType() != typeof(NodeMesh))
            {
                float d = Vector3.Distance(node.position, position);

                if(node.position == position + Vector3.up * d)
                {
                    validPositions.Add(node.position);   
                }
                else if (node.position == position - Vector3.up * d)
                {
                    validPositions.Add(node.position);
                }
                if (node.position == position + transform.right * d)
                {
                    validPositions.Add(node.position);
                }
                else if (node.position == position - transform.right * d)
                {
                    validPositions.Add(node.position);
                }
            }
        }

        return validPositions;
    }
}
