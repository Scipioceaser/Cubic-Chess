using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public Team teamToControl;
    public List<Unit> units;
    private Map mapObject;
    
    private void Start()
    {
        DontDestroyOnLoad(this);
        mapObject = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
    }

    private void FixedUpdate()
    {
        foreach (Unit unit in mapObject.enemyUnits)
        {
            if (!units.Contains(unit))
            {
                units.Add(unit);
            }
        }
    }
    
    private void MoveRandomUnitInRandomDirection()
    {
        if (units.Count == 0)
            return;

        if (GameStateManager.stateManager.CheckState(GameStateManager.State.AI_TURN_THINK))
        {
            Unit u = units[Random.Range(0, units.Count)];
            
            u.SetPositions(u.GetValidMovePositions(u.unAdjustedPosition));

            if (u.positions.Count != 0)
            {
                int destIndex;

                destIndex = Random.Range(0, u.positions.Count);

                foreach (Vector3 vector in u.positions)
                {
                    if (mapObject.NodeFromNodeVector(vector).nodeUnit != null)
                    {
                        if (mapObject.NodeFromNodeVector(vector).nodeUnit.unitTeam != teamToControl)
                        {
                            destIndex = u.positions.IndexOf(vector);
                        }
                    }
                }
                Node enemyNode = mapObject.NodeFromNodeVector(u.positions[destIndex]);

                if (enemyNode.nodeUnit != null)
                {
                    if (enemyNode.nodeUnit.unitTeam != u.unitTeam)
                    {
                        enemyNode.nodeUnit.Fight();
                    }
                }

                GameStateManager.stateManager.SetState(GameStateManager.State.AI_TURN_MOVE, 0.0001f);
                u.MoveAlongPath(u.positions[destIndex]);
            }
            else
            {
                MoveRandomUnitInRandomDirection();
            }
        }
    }
}
