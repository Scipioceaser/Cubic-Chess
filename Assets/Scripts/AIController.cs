﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIController : MonoBehaviour
{
    public Team teamToControl;
    public List<Unit> units;
    private Map mapObject;
    
    private void Start()
    {
        DontDestroyOnLoad(this);
        mapObject = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();

        UpdateUnits();
    }

    private void Update()
    {
        if (units.Count == 0 || units != mapObject.enemyUnits)
        {
            UpdateUnits();
        }
    }

    private void UpdateUnits()
    {
        units.Clear();
        foreach (Unit unit in mapObject.enemyUnits)
        {
            if (!units.Contains(unit))
            {
                units.Add(unit);
            }
        }
    }

    //So this works, alright, but it could be faster by using alpha-beta pruning and caching, but it does seem more clever than just random moves, so that's nice.
    private Vector3 MinimaxRoot(int position, int depth, bool maximizingPlayer)
    {
        float bestMove = Mathf.Infinity;
        Vector3 bestMoveFound = Vector3.zero;
        foreach (Unit unit in units)
        {
            foreach (Vector3 vector3 in unit.GetValidMovePositions(unit.unAdjustedPosition))
            {
                int bestValue = Minimax(vector3, position, depth, 0, 0, !maximizingPlayer);

                if (bestValue < bestMove)
                {
                    bestMove = bestValue;
                    bestMoveFound = vector3;
                }
            }
        }

        if (bestMove == GetTotalSceneValue() || bestMoveFound == Vector3.zero)
        {
            bestMoveFound = GetBestRandomMove();
        }
        
        return bestMoveFound;
    }

    private Vector3 GetBestRandomMove(int i = 0)
    {
        List<Pawn> pawns = new List<Pawn>();

        foreach (Unit unit in units)
        {
            if (unit.GetType() == typeof(Pawn))
            {
                pawns.Add((Pawn)unit);
            }
        }

        if (pawns.Count > 0 && i == 0)
        {
            Pawn p = pawns[Random.Range(0, pawns.Count)];
            List<Vector3> moves = p.GetValidMovePositions(p.unAdjustedPosition);

            if (moves.Count != 0)
            {
                return moves[Random.Range(0, moves.Count)];
            }
            else
            {
                return GetBestRandomMove(1);
            }
        }
        else
        {
            Unit u = units[Random.Range(0, units.Count)];
            List<Vector3> moves = u.GetValidMovePositions(u.unAdjustedPosition);

            if (moves.Count != 0)
            {
                return moves[Random.Range(0, moves.Count)];
            }
            else
            {
                return GetBestRandomMove(1);
            }
        }
    }

    private int Minimax(Vector3 move, int position, int depth, int alpha, int beta, bool maximizingPlayer = false)
    {
        if (depth == 0)
        {
            return GetPossibleNewSceneValue(position, move, maximizingPlayer ? mapObject.playerTeam : teamToControl);
        }
        
        if (maximizingPlayer)
        {
            int value = -100000;
            
            int i = GetPossibleNewSceneValue(position, move, teamToControl);

            foreach (Vector3 vector in AllPossbleMoves(teamToControl))
            {
                value = Mathf.Max(value, Minimax(vector, i, depth - 1, alpha, beta, false));
                alpha = Mathf.Max(alpha, value);
                if (beta <= alpha)
                    break;
            }
            
            return value;
        }
        else
        {
            int value = 100000;

            int i = GetPossibleNewSceneValue(position, move, teamToControl);

            foreach (Vector3 vector in AllPossbleMoves(teamToControl))
            {
                value = Mathf.Min(value, Minimax(vector, i, depth - 1, alpha, beta, false));
                beta = Mathf.Min(beta, value);
                if (beta <= alpha)
                    break;
            }

            return value;
        }
    }
    
    private int GetPossibleNewSceneValue(int sceneValue, Vector3 position, Team controlTeam)
    {
        if (mapObject.NodeFromNodeVector(position).nodeUnit != null 
            && mapObject.NodeFromNodeVector(position).nodeUnit.unitTeam != controlTeam)
        {
            return sceneValue - GetUnitValue(mapObject.NodeFromNodeVector(position).nodeUnit);
        }
        else
        {
            return sceneValue;
        }
    }

    private List<Vector3> AllPossbleMoves(Team team)
    {
        List<Vector3> vectors = new List<Vector3>();

        if (team == mapObject.playerTeam)
        {
            foreach (Unit unit in mapObject.playerUnits)
            {
                foreach (Vector3 vector in unit.GetValidMovePositions(unit.unAdjustedPosition))
                {
                    vectors.Add(vector);
                }
            }
        }
        else
        {
            foreach (Unit unit in units)
            {
                foreach (Vector3 vector in unit.GetValidMovePositions(unit.unAdjustedPosition))
                {
                    vectors.Add(vector);
                }
            }
        }

        return vectors;
    }

    private int GetTotalSceneValue()
    {
        int total = 0;

        foreach (Unit unit in mapObject.units)
        {
            total += GetUnitValue(unit);
        }

        return total;
    }

    private int GetUnitValue(Unit unit)
    {
        if (unit == null)
            return 0;

        int result = 0;

        if (unit.GetType() == typeof(King))
        {
            if (GameRuleManager.ruleManager.GameType == GameType.CLASSIC)
            {
                result = 400;
            }
            else
            {
                result = 4;
            }
        }
        else if (unit.GetType() == typeof(Pawn))
        {
            result = 1;
        }
        else if (unit.GetType() == typeof(Bishop))
        {
            result = 3;
        }
        else if (unit.GetType() == typeof(Rook))
        {
            result = 5;
        }
        else if (unit.GetType() == typeof(Knight))
        {
            result = 3;
        }
        else if (unit.GetType() == typeof(Queen))
        {
            result = 9;
        }

        //Debug.LogWarning("Unit type not recognized");
        if (unit.unitTeam == teamToControl)
        {
            return -result;
        }
        else
        {
            return result;
        }
    }

    private void MoveAndFight(Unit unit, Vector3 destination, bool changeState = true, bool undo = false)
    {
        if (units.Count == 0)
            return;

        if (GameStateManager.stateManager.CheckState(GameStateManager.State.AI_TURN_THINK))
        {
            unit.SetPositions(unit.GetValidMovePositions(unit.unAdjustedPosition));

            if (unit.positions.Count != 0 && unit.positions.Contains(destination))
            {
                Unit enemyUnit = mapObject.NodeFromNodeVector(destination).nodeUnit;

                if (enemyUnit != null)
                {
                    if (enemyUnit.unitTeam != unit.unitTeam)
                    {
                        enemyUnit.Fight();
                    }
                }

                GameStateManager.stateManager.SetState(GameStateManager.State.AI_TURN_MOVE, 0.0001f);

                unit.MoveAlongPath(destination);
            }
        }
    }

    private void MoveRandomUnitInRandomDirection()
    {
        if (units.Count == 0)
            return;

       if (GameStateManager.stateManager.CheckState(GameStateManager.State.AI_TURN_THINK))
       {
            UpdateUnits();

            // May need to be improved
            Vector3 bMove = MinimaxRoot(GetTotalSceneValue(), 4, true);
            List<Unit> possibleUnits = new List<Unit>();
            
            foreach (Unit unit in units)
            {
                foreach (Vector3 vector in unit.GetValidMovePositions(unit.unAdjustedPosition))
                {
                    if (vector == bMove)
                    {
                        possibleUnits.Add(unit);
                    }
                }
            }
            
            Unit u = possibleUnits[Random.Range(0, possibleUnits.Count)];

            //Debug.DrawRay(bMove, Vector3.up, Color.red, 10f);
            //Debug.DrawLine(u.unAdjustedPosition, bMove, Color.red, 10f);
            MoveAndFight(u, bMove);

            UpdateUnits();
        }
    }
}
