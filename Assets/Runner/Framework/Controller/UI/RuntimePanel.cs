using System.Collections;
using System.Collections.Generic;
using QFramework;
using TMPro;
using UnityEngine;
namespace Runner
{
    public class RuntimePanel : RunnerController
    {
        private TextMeshProUGUI score;
        private TextMeshProUGUI player;

        void Start()
        {
            score = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            player = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            Data.Score.RegisterWithInitValue(ScoreChange).UnRegisterWhenGameObjectDestroyed(this);
            Data.PlayerCount.RegisterWithInitValue(PlayerChange).UnRegisterWhenGameObjectDestroyed(this);
        }
        private void ScoreChange(int value)
        {
            score.text ="Score : " + value.ToString();
        }
        private void PlayerChange(int value)
        {
            player.text ="Player Count : " + value.ToString();
        }

    }
}

