using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager stateManager { get; private set; }

    private Map map;
    private AIController AI;
    public State currentState = State.PLAYER_TURN_THINK;

    public enum State
    {
        PLAYER_TURN_THINK,
        AI_TURN_THINK,
        PLAYER_TURN_MOVE,
        AI_TURN_MOVE
    }

    private void Start()
    {
        if (stateManager == null)
        {
            stateManager = this;

            map = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
            AI = GameObject.FindGameObjectWithTag("AI").GetComponent<AIController>();
            
            DontDestroyOnLoad(stateManager);
        }
        else
        {
            Destroy(this);
        }
    }

    // Need to add some sort of delay to state change, at least for now. 
    //When AI needs more time to think, may be less important to have a delay.
    public async void SetState(State stateToSet, float delay = 1f)
    {
        int time = Mathf.RoundToInt((delay * 1000));
        await Task.Delay(time);
        currentState = stateToSet;

        if (currentState == State.AI_TURN_THINK)
        {
            SetState(State.PLAYER_TURN_THINK, 0.001f);
            //AI.SendMessage("MoveRandomUnitInRandomDirection");
        }
    }
    
    public bool CheckState(State stateToCheck)
    {
        return currentState == stateToCheck;
    }
}
