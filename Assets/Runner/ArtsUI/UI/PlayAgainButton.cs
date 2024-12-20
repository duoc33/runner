using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ScriptableObjectBase;
using UnityEngine;
using UnityEngine.UI;
namespace Runner
{
    public class PlayAgainButton : RunnerController
    {
        private AllConfig Runner;
        private Button button;
        
        void Start()
        {
            button = GetComponent<Button>();
            Runner = Data.Runner;
            button.onClick.AddListener(async ()=>{
                await Again();
            });
        }

        async UniTask Again()
        {
            Runner.OnDestroy();
            SOBase.Clear();
            SOBase.InitLocal();
            await Runner.Download();
            Runner.StartMixComponents();
        }
        
    }

}
