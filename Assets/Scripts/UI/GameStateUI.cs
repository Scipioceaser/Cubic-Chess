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
            timer.text = System.TimeSpan.FromSeconds((int)Time.time).ToString();
            turnTime.text = System.TimeSpan.FromSeconds((int)Time.time).ToString();
            playerPoints.SetText(map.playerPoints.ToString());
            AiPoints.SetText(map.aiPoints.ToString());
        }
        
        if (image != null)
        {
            if (GameStateManager.stateManager.CheckState(GameStateManager.State.PLAYER_TURN_THINK))
            {
                image.color = Color.white;
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
                image.color = Color.black;
            }
        }
    }
}
