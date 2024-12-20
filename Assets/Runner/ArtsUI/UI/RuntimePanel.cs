using System.Collections;
using System.Collections.Generic;
using QFramework;
using TMPro;
using UnityEngine;
namespace Runner
{
    public class RuntimePanel : RunnerController
    {
        public TextMeshProUGUI score;
        public TextMeshProUGUI player;

        void Start()
        {
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

