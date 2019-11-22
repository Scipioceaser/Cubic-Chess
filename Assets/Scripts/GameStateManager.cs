using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager stateManager { get; private set; }

    private Map map;
    private AIController AI;
    public State currentState = State.WHITE_TURN;
    public State playerState;
    public State enemyState;

    public enum State
    {
        WHITE_TURN,
        BLACK_TURN
    }

    private void Start()
    {
        if (stateManager == null)
        {
            stateManager = this;

            map = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
            AI = GameObject.FindGameObjectWithTag("AI").GetComponent<AIController>();

            if (map.playerTeam == Team.WHITE)
            {
                playerState = State.WHITE_TURN;
                enemyState = State.BLACK_TURN;
            }
            else
            {
                playerState = State.BLACK_TURN;
                enemyState = State.WHITE_TURN;
            }

            DontDestroyOnLoad(stateManager);
        }
        else
        {
            Destroy(this);
        }
    }

    public void SetState(State stateToSet)
    {
        currentState = stateToSet;

        if (currentState == enemyState)
        {
            print("A");
            AI.SendMessage("MoveRandomUnitInRandomDirection");
        }
    }
    
    public bool CheckState(State stateToCheck)
    {
        return currentState == stateToCheck;
    }
}
