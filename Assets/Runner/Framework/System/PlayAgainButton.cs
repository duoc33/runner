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
        // Start is called before the first frame update
        void Start()
        {
            button = GetComponent<Button>();
            Runner = Data.Runner;
            button.onClick.AddListener(()=>{
                Again().Forget();
            });
        }
        async UniTask Again()
        {
            Runner.OnDestroy();
            SOBase.Clear();
            SOBase.InitServer();
            await Runner.Download();
            Runner.StartMixComponents();
        }
        
    }

}