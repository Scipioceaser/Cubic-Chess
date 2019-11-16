﻿using System.Collections;
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

        u.MoveAlongPath(u.positions[Random.Range(0, u.positions.Count)]);
    }
}
