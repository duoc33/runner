using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ScriptableObjectBase;
using UnityEngine;
namespace Runner
{
    public class AllConfig : SOBase
    {
        public StraightPathLevelSO levelSO;
        public PlayerSO playerSO;
        public GeneralSO generalSO;
        public override async UniTask Download()
        {
            await levelSO.Download();
            await playerSO.Download();
            await generalSO.Download();
            await UniTask.Yield();
        }
        public override void StartMixComponents()
        {
            RunnerModel model = RunnerApp.Interface.GetModel<RunnerModel>();
            model.levelSO = levelSO;
            model.GeneralSO = generalSO;
            levelSO.StartMixComponents();
            playerSO.StartMixComponents();
            generalSO.StartMixComponents();
        }
        public override void OnDestroy()
        {
            RunnerApp.Interface.Deinit();
            levelSO.OnDestroy();
            playerSO.OnDestroy();
            generalSO.OnDestroy();
        }
    }

}
