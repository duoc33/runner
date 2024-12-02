using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Cysharp.Threading.Tasks;
using QFramework;
using RuntimeComponentAdjustTools;
using UnityEngine;
using UnityEngine.AI;
namespace Runner
{
    public class StraightPathLevelGenerator : RunnerController
    {
        void Start()
        {
            this.GetSystem<UISystem>().Show(0);
            this.RegisterEvent<OnStartGenerateMap>(GenerateMap).UnRegisterWhenGameObjectDestroyed(this);
        }
        private void GenerateMap(OnStartGenerateMap e)
        {
            Build().Forget();
        }
        private async UniTask Build()
        {
            foreach (Transform item in transform)
            {
                Destroy(item.gameObject);
            }
            await UniTask.Yield();
            await Data.levelSO.GenerateMap(transform);
            
            this.SendCommand<OnMapGeneratedCmd>();

            this.GetSystem<UISystem>().Show(1);
        }
    }
    public class OnMapGeneratedCmd : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.SendEvent<OnMapGenerated>();
        }
    }
    public struct OnMapGenerated
    {
        
    }
    
    public class OnStartGenerateMapCmd : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.SendEvent<OnStartGenerateMap>();
        }
    }
    public struct OnStartGenerateMap
    {

    }
}