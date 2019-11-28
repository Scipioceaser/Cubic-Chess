using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum Team
{
    BLACK,
    WHITE
}

public enum Direction
{
    UP,
    DOWN,
    FORWARD,
    BACK,
    LEFT,
    RIGHT
}

//TODO: Add in teams
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Unit : MonoBehaviour
{
    //[HideInInspector]
    public Node currentNode;
    [HideInInspector]
    public int moveIndex = 0;
    [HideInInspector]
    public List<Vector3> positions;
    //[HideInInspector]
    public Direction spawnDir;
    //Refer to the node vector position of the unit, not the global (real-world) position
    [HideInInspector]
    public Vector3 unAdjustedPosition;
    [HideInInspector]
    public Map map;
    [HideInInspector]
    public Vector3 moveDirection;

    public Team unitTeam = Team.BLACK;

    private MeshRenderer meshrender;
    private MeshFilter meshfilter;
    private Color outlineColorDefault = Color.black;
    
    public virtual void Awake()
    {
        meshrender = GetComponent<MeshRenderer>();
        meshfilter = GetComponent<MeshFilter>();

        map = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
    }

    #region NODE FUNCTIONS
    public Vector3 GetAdjustedSpawnPosition(float value, Vector3 pos, Vector3 nodePosition)
    {
        return value * Vector3.Normalize(nodePosition - pos) + pos;
    }

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

    public GameObject GetNearestNodeObject(Vector3 position, float distance = 1, bool meshOnly = false)
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
    #endregion

    private void Start()
    {
        if (unitTeam == map.playerTeam)
        {
            map.playerUnits.Add(this);
        }
        else
        {
            map.enemyUnits.Add(this);
        }

        if (unitTeam == Team.WHITE)
        {
            meshrender.sharedMaterial.SetColor("_Color", Color.white);
        }
        else
        {
            meshrender.sharedMaterial.SetColor("_Color", Color.grey);
        }
    }
    
    public void Fight()
    {
        map.units.Remove(this);
        // Replace tag with team name
        if (unitTeam == map.playerTeam)
        {
            map.playerUnits.Remove(this);
            map.playerDeadUnits.Add(transform.name + ":" + transform.tag);
        }
        else
        {
            map.enemyUnits.Remove(this);
            map.enemyDeadUnits.Add(transform.name + ":" + transform.tag);
        }
        
        this.enabled = false;
        meshrender.enabled = false;
        currentNode.nodeUnit = null;
    }

    public void SetOutlineWidthAndColor(float width = 1.01f)
    {
        meshrender.sharedMaterial.SetFloat("_OutlineWidth", width);
    }

    public void SetOutlineWidthAndColor(Color color, float width = 1.01f)
    {
        meshrender.sharedMaterial.SetFloat("_OutlineWidth", width);
        meshrender.sharedMaterial.SetColor("_OutlineColor", color);
    }
    
    public bool IsLineStraight(float A1, float A2, float B1, float B2)
    {
        return Mathf.Abs(A2 - A1) == Mathf.Abs(B2 - B1);
    }
    
    public float LineDelta(float A, float B)
    {
        return Mathf.Abs(B - A);
    }

    public bool IsNodeAtEmptyEdge(Vector3 position)
    {
        return position.y == Globals.mapHeight + 1 && position.x == 0 
            || position.y == Globals.mapHeight + 1 && position.x == Globals.mapSize + 1
            || position.y == Globals.mapHeight + 1 && position.z == 0
            || position.y == Globals.mapHeight + 1 && position.z == Globals.mapSize + 1
            || position.y == 0 && position.x == 0
            || position.y == 0 && position.x == Globals.mapSize + 1
            || position.y == 0 && position.z == 0
            || position.y == 0 && position.z == Globals.mapSize + 1;
    }

    //TODO: Add particle effect when node mesh lands at destination. Some shake would be good too.
    public IEnumerator Move(Vector3 startPos, Vector3 endPos, float timeValue)
    {
        float r = 1.0f / timeValue;
        float t = 0.0f;

        while (t < 1.0f)
        {
            t += Time.deltaTime * r;
            gameObject.transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0.0f, 1.0f, t));
            
            yield return null;
        }

        if (GameStateManager.stateManager.CheckState(GameStateManager.State.PLAYER_TURN_MOVE))
        {
            GameStateManager.stateManager.SetState(GameStateManager.State.AI_TURN_THINK, 0.75f);
        }
        else if (GameStateManager.stateManager.CheckState(GameStateManager.State.AI_TURN_MOVE))
        {
            GameStateManager.stateManager.SetState(GameStateManager.State.PLAYER_TURN_THINK, 0.01f);
        }
        
        //moving = false;
    }
    
    //TODO: Add check so that units who can move long distances need to be at edge to go to the sides
    public virtual List<Vector3> GetValidMovePositions(Vector3 position, int team = 1)
    {
        // Don't copy this bit
        return null;
    }

    public void SetPositions(List<Vector3> movePositions)
    {
        positions = movePositions;
    }

    public static Vector3 UnitDirectionToVectorDirection(Direction dir, bool multiplyGlobalScale = false)
    {
        Vector3 d = Vector3.zero;

        switch (dir)
        {
            case Direction.FORWARD:
                d = Vector3.forward;
                break;
            case Direction.BACK:
                d = Vector3.back;
                break;
            case Direction.LEFT:
                d = Vector3.left;
                break;
            case Direction.RIGHT:
                d = Vector3.right;
                break;
            case Direction.UP:
                d = Vector3.up;
                break;
            case Direction.DOWN:
                d = Vector3.down;
                break;
            default:
                break;
        }

        if (multiplyGlobalScale)
            d *= Globals.scale;

        return d;
    }

    public void AlignUnit(Vector3 destination)
    {
        if (!this || !gameObject)
            return;

        Vector3 d = (destination - GetNearestNode(destination, 1, true).position);
        //transform.LookAt(transform.localPosition + d);
        transform.rotation = Quaternion.FromToRotation(Vector3.up, d);
    }

    //TODO: Add loop to move along all points
    public virtual void MoveAlongPath(Vector3 destination = new Vector3()) { }
}
