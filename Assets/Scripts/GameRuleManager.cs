using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameType
{
    POINTS_TIMED,
    DEATHMATCH_TOTAL,
    CLASSIC,
    SCENARIO
}

public class GameRuleManager : MonoBehaviour
{
    public static GameRuleManager ruleManager { get; private set; }
    public int TimeLimit = 300;
    public int thinkTurn = 30;
    public GameType GameType;
    private Map map;
    private bool timeHasElapsed = false;
    private float playerTimeThink = 0;
    [HideInInspector]
    public bool playerTurnThinkDelay = false;

    private void Awake()
    {
        map = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
    }

    private void Start()
    {
        if (ruleManager == null)
        {
            ruleManager = this;

            if (GameType == GameType.CLASSIC)
            {
                int i = 0;
                foreach (Unit unit in map.units)
                {
                    if (unit.GetType() == typeof(King)) i++;
                }

                if (i != 2)
                {
                    Debug.LogError("There are not two kings present");
                }
            }

            DontDestroyOnLoad(ruleManager);
        }
        else
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        if (GameType == GameType.POINTS_TIMED && GameStateManager.stateManager.CheckState(GameStateManager.State.PLAYER_TURN_THINK))
        {
            playerTimeThink += Time.deltaTime;
            
            if (playerTimeThink >= thinkTurn)
            {
                playerTimeThink = 0;
                playerTurnThinkDelay = true;
                GameStateManager.stateManager.SetState(GameStateManager.State.AI_TURN_THINK, 0.75f);
            }
        }

        switch (GameType)
        {
            default:
                break;
            case GameType.CLASSIC:
                Classic();
                break;
            case GameType.DEATHMATCH_TOTAL:
                DeathmatchTotal();
                break;
            case GameType.POINTS_TIMED:
                PointsTimed();
                break;
            case GameType.SCENARIO:
                //Scenario();
                break;
        }
    }

    private void Classic()
    {
        if (map.enemyDeadUnits.Contains("King"))
        {
            GameStateManager.stateManager.SetState(GameStateManager.State.PLAYER_WIN, 0.01f);
        }
        else if (map.playerDeadUnits.Contains("King"))
        {
            GameStateManager.stateManager.SetState(GameStateManager.State.AI_WIN, 0.01f);
        }
    }

    private void DeathmatchTotal()
    {
        if (map.enemyUnits.Count <= 0)
        {
            GameStateManager.stateManager.SetState(GameStateManager.State.PLAYER_WIN, 0.01f);
        }
        else if (map.playerUnits.Count <= 0)
        {
            GameStateManager.stateManager.SetState(GameStateManager.State.AI_WIN, 0.01f);
        }
    }

    private void PointsTimed()
    {
        if (Time.time > TimeLimit)
        {
            if (!timeHasElapsed)
            {
                timeHasElapsed = true;
                
                if (map.playerPoints > map.aiPoints)
                {
                    GameStateManager.stateManager.SetState(GameStateManager.State.PLAYER_WIN, 0.01f);
                }
                else
                {
                    GameStateManager.stateManager.SetState(GameStateManager.State.AI_WIN, 0.01f);
                }
            }
        }
    }

    private void Scenario(List<string> unitsToKill)
    {
        int i = 0;
        foreach (string item in unitsToKill)
        {
            if (map.enemyDeadUnits.Contains(item))
                i++;
        }

        if (i == unitsToKill.Count)
            GameStateManager.stateManager.SetState(GameStateManager.State.PLAYER_WIN, 0.01f);
    }
}
