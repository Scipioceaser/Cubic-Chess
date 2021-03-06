﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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
[RequireComponent(typeof(AudioSource))]
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
    [HideInInspector]
    public Vector3 lastPosition;
    [HideInInspector]
    public Quaternion lastRotation;
    
    public Team unitTeam = Team.BLACK;

    private MeshRenderer meshrender;
    private MeshFilter meshfilter;
    [HideInInspector]
    public AudioSource audioSource;
    private Color outlineColorDefault = Color.black;

    public int pointValue = 100;

    [Header("Sound")]
    public AudioClip dragClip;
    public AudioClip breakClip;

    [HideInInspector]
    public MeshCollider meshCol;
    
    public virtual void Awake()
    {
        meshrender = GetComponent<MeshRenderer>();
        meshfilter = GetComponent<MeshFilter>();
        meshCol = GetComponent<MeshCollider>();
        audioSource = GetComponent<AudioSource>();

        map = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
        map.units.Add(this);
        lastPosition = transform.position;
        lastRotation = transform.rotation;
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
            meshrender.material.SetColor("_Color", new Color(0.8f, 0.8f, 0.8f, 1.0f));
        }
        else
        {
            meshrender.material.SetColor("_Color", new Color(0.2f, 0.2f, 0.2f, 1.0f));
        }
    }
    
    public void Fight(bool permanent = true)
    {
        map.units.Remove(this);
        
        if (unitTeam == map.playerTeam)
        {
            map.playerUnits.Remove(this);
            map.aiPoints += pointValue;
            map.playerDeadUnits.Add(transform.name);
        }
        else
        {
            map.enemyUnits.Remove(this);
            map.playerPoints += pointValue;
            map.enemyDeadUnits.Add(transform.name);
        }
        
        this.enabled = false;
        meshrender.enabled = false;
        meshCol.enabled = false;

        if (!GameStateManager.stateManager.CheckState(GameStateManager.State.PLAYER_WIN))
        {
            audioSource.clip = breakClip;
            audioSource.PlayDelayed(0.35f);
        }

        if (permanent)
            currentNode.nodeUnit = null;
    }

    public void UndoFight()
    {
        map.units.Add(this);
        // Replace tag with team name
        if (unitTeam == map.playerTeam)
        {
            map.playerUnits.Add(this);
            map.aiPoints -= pointValue;
            map.playerDeadUnits.Remove(transform.name);
        }
        else
        {
            map.enemyUnits.Add(this);
            map.playerPoints -= pointValue;
            map.enemyDeadUnits.Remove(transform.name);
        }

        this.enabled = true;
        meshrender.enabled = true;
        meshCol.enabled = true;
        currentNode.nodeUnit = this;
        GetNearestNode(unAdjustedPosition, 1).nodeUnit = this;
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

    public bool EnemyInFrontOfNode(Vector3 fromPosition, Vector3 nodePosition)
    {
        Vector3 dir = nodePosition - fromPosition;
        return Physics.Raycast(fromPosition, dir, dir.magnitude, ~0, QueryTriggerInteraction.Ignore);
    }

    public bool IsNodeAtEmptyEdge(Vector3 position)
    {
        return position.y == Globals.mapHeight + 1 && position.x == 0 
            || position.y == Globals.mapHeight + 1 && position.x == Globals.mapWidth + 1
            || position.y == Globals.mapHeight + 1 && position.z == 0
            || position.y == Globals.mapHeight + 1 && position.z == Globals.mapLength + 1
            || position.y == 0 && position.x == 0
            || position.y == 0 && position.x == Globals.mapWidth + 1
            || position.y == 0 && position.z == 0
            || position.y == 0 && position.z == Globals.mapLength + 1;
    }

    public virtual void UndoMove()
    {

        if (currentNode.nodeUnit == this)
        {
            currentNode.SetNodeUnit(null);
        }

        GetNearestNode(lastPosition).SetNodeUnit(this);
        currentNode = GetNearestNode(lastPosition);
        unAdjustedPosition = lastPosition;

        Vector3 p = GetAdjustedSpawnPosition(0.5f, unAdjustedPosition,
            GetNearestNode(unAdjustedPosition, 1, true).transform.position);

        transform.rotation = lastRotation;

        StopAllCoroutines();
        transform.position = p;
    }
    
    public virtual IEnumerator Move(Vector3 startPos, Vector3 endPos, float timeValue)
    {
        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(dragClip);

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
            if (GameRuleManager.ruleManager.playerTurnThinkDelay)
            {
                GameStateManager.stateManager.SetState(GameStateManager.State.PLAYER_TURN_THINK, 0.85f);
                GameRuleManager.ruleManager.playerTurnThinkDelay = false;
            }
            else
            {
                GameStateManager.stateManager.SetState(GameStateManager.State.PLAYER_TURN_THINK, 0.01f);
            }
        }
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
    public virtual void MoveAlongPath(Vector3 destination = new Vector3(), bool changeState = true) { }
}
