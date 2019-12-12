using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

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
    public List<Unit> unitsToKill = new List<Unit>();
    private Map map;
    private bool timeHasElapsed = false;
    [HideInInspector]
    public float playerTimeThink = 0;
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
                Thread.Sleep(2000);
                int c = 0;
                int i = 0;
                int j = 0;
                foreach (Unit unit in map.playerUnits)
                {
                    if (unit.GetType() == typeof(King)) i++;
                }

                foreach (Unit unit in map.enemyUnits)
                {
                    if (unit.GetType() == typeof(King)) j++;
                }

                if (i != 1 && j != 1)
                {
                    c++;
                }
                else if (i != 1 && j != 1 && c == 1000)
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
                playerTimeThink = 0f;
                playerTurnThinkDelay = true;
                GameStateManager.stateManager.SetState(GameStateManager.State.AI_TURN_THINK, 0.75f);
            }
        }

        if (GameStateManager.stateManager.CheckState(GameStateManager.State.PLAYER_TURN_MOVE))
        {
            playerTimeThink = 0f;
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
                Scenario(unitsToKill);
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

    private void Scenario(List<Unit> unitsToKill)
    {
        if (map.playerUnits.Count <= 0)
            GameStateManager.stateManager.SetState(GameStateManager.State.AI_WIN, 0.01f);

        if (unitsToKill.Count == 0)
            Debug.LogError("No target units specified!");

        int i = 0;
        foreach (Unit item in unitsToKill)
        {
            if (map.enemyDeadUnits.Contains(item.name))
                i++;
        }

        if (i == unitsToKill.Count)
            GameStateManager.stateManager.SetState(GameStateManager.State.PLAYER_WIN, 0.01f);
    }
}
