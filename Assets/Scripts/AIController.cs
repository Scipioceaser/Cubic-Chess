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
        mapObject = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
    }

    private void FixedUpdate()
    {
        foreach (Unit unit in mapObject.units)
        {
            if (unit.unitTeam == teamToControl && !units.Contains(unit))
            {
                units.Add(unit);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.M))
        {
            MoveRandomUnitInRandomDirection();
        }
    }

    private void MoveRandomUnitInRandomDirection()
    {
        Unit u = units[Random.Range(0, units.Count)];

        u.SetPositions(u.GetValidMovePositions(u.unAdjustedPosition));

        int destIndex;

        destIndex = Random.Range(0, u.positions.Count);

        foreach (Vector3 vector in u.positions)
        {
            print(mapObject.NodeFromNodeVector(vector).nodeUnit);
            if (mapObject.NodeFromNodeVector(vector).nodeUnit != null)
            {
                if (mapObject.NodeFromNodeVector(vector).nodeUnit.unitTeam != teamToControl)
                {
                    destIndex = u.positions.IndexOf(vector);
                }
            }
        }
        
       Unit enemy = mapObject.NodeFromNodeVector(u.positions[destIndex]).nodeUnit;

        if (enemy != null)
        {
            if (enemy.unitTeam != u.unitTeam)
            {
                enemy.Fight();
            }
        }
        
        u.MoveAlongPath(u.positions[destIndex]);
    }
}
