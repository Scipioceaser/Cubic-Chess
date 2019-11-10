using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum UnitType
{
    PAWN=0,
    ROOK,
    KNIGHT,
    QUEEN,
    KING,
    BISHOP
}

public enum Direction
{
    FORWARD,
    BACK,
    LEFT,
    RIGHT,
    UP,
    DOWN
}

public class UnitSpawnPoint : MonoBehaviour
{
    public UnitType unit;
    public Direction alignDirection;
    
    private Vector3 spawnDirection;

    private Vector3 spawnMeshPosition = Vector3.negativeInfinity;
    
    private void FixedUpdate()
    {
        if (Globals.meshNodesCreated == (Globals.mapSize * Globals.mapSize * Globals.mapHeight))
        {
            SpawnUnit(unit);
        }
    }

    private void SpawnUnit(UnitType type)
    {
        //transform.LookAt(transform.localPosition + GetSpawnDirection(alignDirection));

        if (type == UnitType.PAWN)
        {
            //TODO: Add mesh drop-in shader effect
            
            Pawn p = gameObject.AddComponent<Pawn>();
            p.spawnDir = alignDirection;
            p.unAdjustedPosition = transform.position;

            if (p.unAdjustedPosition.y == 0 || p.unAdjustedPosition.y == Globals.mapHeight + 1)
            {
                p.horizontalMoveDirection = Vector3.forward;
            }
            else
            {
                if (alignDirection == Direction.RIGHT || alignDirection == Direction.LEFT)
                {
                    p.horizontalMoveDirection = Vector3.right;
                }
                else
                {
                    p.horizontalMoveDirection = Vector3.forward;
                }
            }
            
            transform.position = GetAdjustedSpawnPosition(0.5f, transform.localPosition, GetNearestNodeObject(transform.localPosition, 2, true).transform.position);
        }
        else if (type == UnitType.ROOK)
        {
            Rook r = gameObject.AddComponent<Rook>();
            r.spawnDir = alignDirection;
            r.unAdjustedPosition = transform.position;

            transform.position = GetAdjustedSpawnPosition(0.5f, transform.localPosition, GetNearestNodeObject(transform.position, 2, true).transform.position);
        }
        else if (type == UnitType.BISHOP)
        {
            Bishop b = gameObject.AddComponent<Bishop>();
            b.spawnDir = alignDirection;
            b.unAdjustedPosition = transform.position;

            transform.position = GetAdjustedSpawnPosition(0.5f, transform.localPosition, GetNearestNodeObject(transform.position, 2, true).transform.position);
        }
        else if (type == UnitType.KING)
        {
            King k = gameObject.AddComponent<King>();
            k.spawnDir = alignDirection;
            k.unAdjustedPosition = transform.position;

            transform.position = GetAdjustedSpawnPosition(0.5f, transform.localPosition, GetNearestNodeObject(transform.position, 2, true).transform.position);
        }
        else if (type == UnitType.QUEEN)
        {
            Queen q = gameObject.AddComponent<Queen>();
            q.spawnDir = alignDirection;
            q.unAdjustedPosition = transform.position;

            transform.position = GetAdjustedSpawnPosition(0.5f, transform.localPosition, GetNearestNodeObject(transform.position, 2, true).transform.position);
        }
        else if (type == UnitType.KNIGHT)
        {
            Knight k = gameObject.AddComponent<Knight>();
            k.spawnDir = alignDirection;
            k.unAdjustedPosition = transform.position;

            transform.position = GetAdjustedSpawnPosition(0.5f, transform.localPosition, GetNearestNodeObject(transform.position, 2, true).transform.position);
        }

        gameObject.GetComponent<Unit>().unitTeam = Team.BLACK;
        gameObject.name = type.ToString().ToLower();
        DestroyImmediate(this);
    }

    public static Vector3 GetAdjustedSpawnPosition(float value, Vector3 pos, Vector3 nodePosition)
    {
        return value * Vector3.Normalize(nodePosition - pos) + pos;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;
        
        DebugArrow.ForGizmo(transform.localPosition, Unit.UnitDirectionToVectorDirection(alignDirection) / 1.5f, Color.green);
    }

    #region Nearby node scripts

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
    public static Node GetNearestNode(Vector3 position, float distance = 1, bool meshOnly = false)
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
            Debug.LogWarning("Could not find nearby node");

        return node;
    }

    public class DebugArrow
    {

        public static void ForGizmo(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Gizmos.DrawRay(pos, direction);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
        }

        public static void ForGizmo(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Gizmos.color = color;
            Gizmos.DrawRay(pos, direction);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
        }
    }

    #endregion
}
