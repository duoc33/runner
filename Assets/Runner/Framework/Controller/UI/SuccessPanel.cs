using System.Collections;
using System.Collections.Generic;
using QFramework;
using TMPro;
using UnityEngine;
namespace Runner
{
    public class SuccessPanel : RunnerController
    {
        void Start()
        {
            this.RegisterEvent<OnGameOverEvent>(OnSuccess);
        }
        void OnSuccess(OnGameOverEvent e)
        {
            if(e.IsWin)
            {
                TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
                
                text.text = $"Success !!!\nPlayer Remain Count : {Data.PlayerCount}.\nScore : {Data.Score}";
            }
            
        }
    }
}

