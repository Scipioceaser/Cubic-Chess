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
    
    // TODO: When movement code is implemented, will need to make sure the code allows for a pawn to cirumnavigate the map and not
    // just move up.
    public override List<Vector3> GetValidMovePositions(Vector3 position)
    {
        List<Vector3> validPositions = new List<Vector3>();
        List<Node> nearbyNodes = map.GetNeighbours(map.NodeFromWorldPoints(position));
        
        foreach (Node node in nearbyNodes)
        {
            if (node.GetType() != typeof(NodeMesh))
            {
                if (position.x == 0 || position.x == (Globals.mapSize + 1))
                {
                    if (node.position == position + Vector3.up && node.nodeUnit == null)
                    {
                        validPositions.Add(node.position);
                    }
                    else if (node.position == position + new Vector3(0, 1, -1) || node.position == position + new Vector3(0, 1, 1))
                    {
                        // Check if nodeUnit is not part of the same team
                        if (node.nodeUnit != null)
                        {
                            validPositions.Add(node.position);
                        }
                    }
                }
                else if (position.z == 0 || position.z == (Globals.mapSize + 1))
                {
                    if (node.position == position + Vector3.up && node.nodeUnit == null)
                    {
                        validPositions.Add(node.position);
                    }
                    else if (node.position == position + new Vector3(-1, 1, 0) || node.position == position + new Vector3(1, 1, 0))
                    {
                        // Check if nodeUnit is not part of the same team
                        if (node.nodeUnit != null)
                        {
                            validPositions.Add(node.position);
                        }
                    }
                }
                else if (position.y == 0)
                {
                    if (node.position == position + transform.up && node.nodeUnit == null)
                    {
                        validPositions.Add(node.position);
                    }
                    else if (node.position == (position + transform.up + transform.right) || node.position == (position + transform.up + -transform.right))
                    {
                        if (node.nodeUnit != null)
                        {
                            validPositions.Add(node.position);
                        }
                    }
                }
                else if (position.y == (Globals.mapHeight + 1))
                {
                    if (node.position == position - transform.up && node.nodeUnit == null)
                    {
                        validPositions.Add(node.position);
                    }
                    else if (node.position == (position - transform.up + transform.right) || node.position == (position - transform.up + -transform.right))
                    {
                        if (node.nodeUnit != null)
                        {
                            validPositions.Add(node.position);
                        }
                    }
                }
            }
        }

        //print(validPositions.Count);
        //Vector3 p = UnitSpawnPoint.GetAdjustedSpawnPosition(0.5f, validPositions[0], UnitSpawnPoint.GetNearestNode(validPositions[0], 1, true).transform.position);
        //print(p);
        //currentNode.SetNodeUnit(null);
        //UnitSpawnPoint.GetNearestNode(validPositions[0]).SetNodeUnit(this);
        //currentNode = UnitSpawnPoint.GetNearestNode(validPositions[0]);
        //StartCoroutine(Move(transform.position, p, 0.5f));
        
        return validPositions;
    }

    public override IEnumerator MoveAlongPath(List<Vector3> positions)
    {
        //TODO: Add loop to move along all points
        Vector3 p = UnitSpawnPoint.GetAdjustedSpawnPosition(0.5f, positions[0], UnitSpawnPoint.GetNearestNode(positions[0], 1, true).transform.position);
        currentNode.SetNodeUnit(null);
        UnitSpawnPoint.GetNearestNode(positions[0]).SetNodeUnit(this);
        currentNode = UnitSpawnPoint.GetNearestNode(positions[0]);
        StartCoroutine(Move(transform.position, p, 0.5f));

        return base.MoveAlongPath(positions);
    }
}
