using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnIndicator_Temp : MonoBehaviour
{
    RawImage image;

    private void Start()
    {
        image = GetComponent<RawImage>();
    }

    private void Update()
    {
        if (GameStateManager.stateManager.CheckState(GameStateManager.State.PLAYER_TURN_THINK))
        {
            image.color = Color.green;
        }
        else if (GameStateManager.stateManager.CheckState(GameStateManager.State.AI_WIN))
        {
            image.color = Color.magenta;
        }
        else if (GameStateManager.stateManager.CheckState(GameStateManager.State.PLAYER_WIN))
        {
            image.color = Color.yellow;
        }
        else
        {
            image.color = Color.red;
        }
    }
}
