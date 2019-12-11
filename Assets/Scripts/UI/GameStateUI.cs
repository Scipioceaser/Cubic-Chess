using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameStateUI : MonoBehaviour
{
    public RawImage image;
    public TextMeshProUGUI playerPoints;
    public TextMeshProUGUI AiPoints;
    public TextMeshProUGUI timer;
    public TextMeshProUGUI turnTime;
    private Map map;

    private void Awake()
    {
        map = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
    }

    private void Update()
    {
        if (playerPoints != null && AiPoints != null && GameRuleManager.ruleManager.GameType == GameType.POINTS_TIMED)
        {
            //TODO: Remove hour sections
            timer.text = System.TimeSpan.FromSeconds((int)Time.timeSinceLevelLoad).ToString();
            turnTime.text = System.TimeSpan.FromSeconds((int)GameRuleManager.ruleManager.playerTimeThink).ToString();
            playerPoints.SetText(map.playerPoints.ToString());
            AiPoints.SetText(map.aiPoints.ToString());
        }

        if (PauseMenu.paused)
        {
            image.enabled = false;
            timer.enabled = false;
            AiPoints.enabled = false;
            playerPoints.enabled = false;
            turnTime.enabled = false;
        }
        else
        {
            image.enabled = true;
            timer.enabled = true;
            AiPoints.enabled = true;
            playerPoints.enabled = true;
            turnTime.enabled = true;
        }
            
        if (image != null)
        {
            if (GameStateManager.stateManager.CheckState(GameStateManager.State.PLAYER_TURN_THINK))
            {
                image.color = Color.white;
            }
            else if (GameStateManager.stateManager.CheckState(GameStateManager.State.AI_WIN) ||
                GameStateManager.stateManager.CheckState(GameStateManager.State.PLAYER_WIN) ||
                PauseMenu.paused)
            {
                image.enabled = false;
            }
            else
            {
                image.color = Color.black;
            }
        }
    }
}
