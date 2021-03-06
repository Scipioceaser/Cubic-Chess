﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Unit
{
    public Vector3 horizontalMoveDirection;
    public GameObject queenPrefab;
    public AudioClip toQueenClip;
    private Vector3 verticalMoveDir = Vector3.up;
    private Node lastNode;
    private int nodesPassed;
    private bool firstMove = true;
    
    public override void Awake()
    {
        base.Awake();

        unAdjustedPosition = transform.position;

        currentNode = GetNearestNode(transform.position);
        currentNode.SetNodeUnit(this);
        AlignUnit(currentNode.position);

        horizontalMoveDirection = UnitDirectionToVectorDirection(spawnDir);
        
        transform.position = GetAdjustedSpawnPosition(0.5f, transform.localPosition, GetNearestNodeObject(transform.localPosition, 2, true).transform.position);
    }

    private void Update()
    {
        //CheckForQueenTransition();
    }

    //TODO: Add in effects to signal unit change
    public void CheckForQueenTransition()
    {
        if (nodesPassed == (Globals.mapHeight * 2 + Globals.mapWidth + Globals.mapLength))
        {
            gameObject.GetComponent<MeshFilter>().mesh = queenPrefab.GetComponent<MeshFilter>().sharedMesh;
            transform.position = unAdjustedPosition;
            Queen q = gameObject.AddComponent<Queen>();
            q.unitTeam = unitTeam;
            q.PlayConfetti();
            q.audioSource.Stop();
            q.audioSource.PlayOneShot(toQueenClip);
            GameStateManager.stateManager.SetState(GameStateManager.State.AI_TURN_THINK, 0.5f);
            Destroy(this);
        }
    }

    public override IEnumerator Move(Vector3 startPos, Vector3 endPos, float timeValue)
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

        CheckForQueenTransition();

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

    public override List<Vector3> GetValidMovePositions(Vector3 position, int team = 1)
    {
        List<Vector3> validPositions = new List<Vector3>();
        List<Node> nearbyNodes = map.GetNeighbours(map.NodeFromWorldPoints(position), 2);
        
        foreach (Node node in nearbyNodes)
        {
            if (node.GetType() != typeof(NodeMesh))
            {
                if (node.nodeUnit != null)
                {
                    if (node.nodeUnit.unitTeam == unitTeam)
                    {
                        continue;
                    }
                }

                if (node.position.x == 0 && node.position.z == 0 || node.position.x == 0 && node.position.z == Globals.mapLength + 1
                       || node.position.x == Globals.mapWidth + 1 && node.position.z == 0 || node.position.x == Globals.mapWidth + 1 && node.position.z == Globals.mapLength + 1)
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

                    if (node.position == position + horizontalMoveDirection && node.nodeUnit == null)
                    {
                        validPositions.Add(node.position);
                    }
                    else if (node.position == position + (horizontalMoveDirection * 2) && firstMove && node.nodeUnit == null)
                    {
                        meshCol.isTrigger = true;
                        if (!EnemyInFrontOfNode(position, node.position))
                        {
                            meshCol.isTrigger = false;
                            validPositions.Add(node.position);
                        }
                    }
                    else if (d == Mathf.Sqrt(2) && node.nodeUnit != null && Mathf.Abs(unAdjustedPosition.y - node.position.y) != 1)
                    {
                        if (Vector3.Distance(unAdjustedPosition + horizontalMoveDirection, node.position) == 1)
                        {
                            if (node.nodeUnit.unitTeam != unitTeam)
                                validPositions.Add(node.position);
                        }
                    }
                    else if (d == Mathf.Sqrt(2) && Mathf.Abs(unAdjustedPosition.y - node.position.y) == 1 && node.nodeUnit == null)
                    {
                        if (node == lastNode)
                            continue;

                        Vector3 p = new Vector3(node.position.x, unAdjustedPosition.y, node.position.z);

                        if (p != position + horizontalMoveDirection)
                            continue;

                        if (node.nodeUnit != null)
                            continue;

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

                    if (node.position == position + verticalMoveDir && node.nodeUnit == null)
                    {
                        validPositions.Add(node.position);
                    }
                    else if (node.position == position + (verticalMoveDir * 2) && firstMove && node.nodeUnit == null)
                    {
                        meshCol.isTrigger = true;
                        if (!EnemyInFrontOfNode(position, node.position))
                        {
                            meshCol.isTrigger = false;
                            validPositions.Add(node.position);
                        }
                    }
                    else if (d == Mathf.Sqrt(2) && node.nodeUnit != null && node.position.y != 0 && node.position.y != Globals.mapHeight + 1)
                    {
                        if (Vector3.Distance(position + verticalMoveDir, node.position) == 1)
                        {
                            if (node.nodeUnit.unitTeam != unitTeam)
                                validPositions.Add(node.position);
                        }
                    }
                    else if (node.position.y == 0 || node.position.y == Globals.mapHeight + 1)
                    {
                        horizontalMoveDirection = SetHorizontalMoveDir(position);

                        if (node.nodeUnit != null)
                            continue;

                        if (d == Mathf.Sqrt(2) && node.nodeUnit == null)
                            validPositions.Add(node.position);
                    }
                }
            }
        }

        return validPositions;
    }
    
    public override void MoveAlongPath(Vector3 destination = new Vector3(), bool changeState = true)
    {
        Vector3 p = GetAdjustedSpawnPosition(0.5f, destination,
            GetNearestNode(destination, 1, true).transform.position);

        lastNode = currentNode;

        // Handle rotation
        AlignUnit(destination);

        // Record the position for undo function
        lastPosition = unAdjustedPosition;

        // Set nodes
        currentNode.SetNodeUnit(null);
        GetNearestNode(destination).SetNodeUnit(this);
        currentNode = GetNearestNode(destination);
        unAdjustedPosition = destination;

        // Add to the total nodes passed
        nodesPassed++;

        firstMove = false;

        // Actually move
        StartCoroutine(Move(transform.position, p, 0.5f));
    }

    private Vector3 SetHorizontalMoveDir(Vector3 position)
    {
        Vector3 dir = new Vector3();

        if (position.x == 0)
        {
            dir = Vector3.right;
        }
        else if (position.x == Globals.mapWidth + 1)
        {
            dir = Vector3.left;
        }
        else if (position.z == 0)
        {
            dir = Vector3.forward;
        }
        else if (position.z == Globals.mapLength + 1)
        {
            dir = Vector3.back;
        }

        return dir;
    }
    
    public override void UndoMove()
    {
        base.UndoMove();

        nodesPassed--;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            DebugArrow.ForGizmo(transform.localPosition, Unit.UnitDirectionToVectorDirection(spawnDir) / 1.5f, Color.blue);
        }
    }
}