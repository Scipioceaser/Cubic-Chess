using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{
    PAWN=0,
    ROOK,
    KNIGHT,
    QUEEN,
    KING,
    BISHOP
}

public class UnitSpawnPoint : MonoBehaviour
{
    public UnitType unit;

    private Vector3 spawnMeshPosition = Vector3.negativeInfinity;
    
    private void SpawnUnit(Unit unit)
    {
        
    }

    private List<GameObject> GetNearbyObjects(Vector3 position, float distance = 1, bool meshOnly = false)
    {
        Collider[] colliders = Physics.OverlapSphere(position, distance);
        List<GameObject> nodePoints = new List<GameObject>();

        foreach (Collider collider in colliders)
        {
            // This is quite precarious code.
            if (collider.transform.name.Contains("Node") || collider.transform.name.Contains("NodeMesh"))
            {
                if (meshOnly)
                {
                    if (collider.transform.name.Contains("NodeMesh"))
                        nodePoints.Add(collider.gameObject);
                }
                else
                {
                    nodePoints.Add(collider.gameObject);
                }
            }
        }

        return nodePoints;
    }

    // Only use this in the scene because we can't add the node script for I don't know why.
    private GameObject GetNearestNodeObject(Vector3 position, float distance = 1, bool meshOnly = false)
    {
        Collider[] colliders = Physics.OverlapSphere(position, distance);
        List<GameObject> nodePoints = new List<GameObject>();

        foreach (Collider collider in colliders)
        {
            // This is quite precarious code.
            if (collider.transform.name.Contains("Node") || collider.transform.name.Contains("NodeMesh"))
            {
                if (meshOnly)
                {
                    if (collider.transform.name.Contains("NodeMesh"))
                        nodePoints.Add(collider.gameObject);
                }
                else
                {
                    nodePoints.Add(collider.gameObject);
                }
            }
        }

        if (nodePoints.Count <= 0)
        {
            Debug.LogWarning("No nodes found");
            return null;
        }
        
        GameObject g = nodePoints[0];

        float d = Vector3.Distance(position, nodePoints[0].transform.position);
        foreach (GameObject gameObject in nodePoints)
        {
            if (Vector3.Distance(position, gameObject.transform.position) < d)
            {
                d = Vector3.Distance(position, gameObject.transform.position);
                g = gameObject;
            }
        }

        return g;
    }

    // This could be cleaner But it works, I guess?
    private Node GetNearestNode(Vector3 position, float distance = 1, bool meshOnly = false)
    {
        Node node = null;

        Collider[] colliders = Physics.OverlapSphere(position, distance);
        List<GameObject> nodePoints = new List<GameObject>();

        foreach (Collider collider in colliders)
        {
            // This is quite precarious code.
            if (collider.transform.name.Contains("Node") || collider.transform.name.Contains("NodeMesh"))
            {
                if (meshOnly)
                {
                    if(collider.transform.name.Contains("NodeMesh"))
                        nodePoints.Add(collider.gameObject);
                }
                else
                {
                    nodePoints.Add(collider.gameObject);
                }
            }
        }

        if (nodePoints.Count <= 0)
            Debug.LogWarning("No nodes found");

        GameObject g = nodePoints[0];
        
        float d = Vector3.Distance(position, nodePoints[0].transform.position);
        foreach (GameObject gameObject in nodePoints)
        {
            if (Vector3.Distance(position, gameObject.transform.position) < d)
            {
                d = Vector3.Distance(position, gameObject.transform.position);
                g = gameObject;
            }
        }

        if (g != null)
        {
            if (meshOnly)
            {
                node = g.GetComponent<NodeMesh>();
            }
            else
            {
                node = g.GetComponent<Node>();
            }
        }
        else
        {
            Debug.LogWarning("Could not find gameObject.");
        }
            
        if (node == null)
            Debug.LogWarning("Could not find nearby node: " + transform.name);

        return node;
    }

    private void OnDrawGizmos()
    {
        if (spawnMeshPosition != Vector3.negativeInfinity)
        {
            GameObject g = GetNearestNodeObject(transform.position, 1, true);

            if (g != null)
                Gizmos.DrawLine(transform.position, g.transform.position);
        }
    }
}
