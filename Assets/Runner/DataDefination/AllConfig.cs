using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers;
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
        private GameObject uiCanvas;
        private UIConfig uIConfig;
        public void SetUIConfig(UIConfig config) => uIConfig = config;
        public override void StartMixComponents()
        {
            RunnerModel model = RunnerApp.Interface.GetModel<RunnerModel>();
            uiCanvas = RunnerApp.Interface.GetSystem<UISystem>().InitUI();
            if(uIConfig!=null)
            {
                uIConfig.Apply(uiCanvas.GetComponent<UIInfo>());
            }
            model.Runner = this;
            model.levelSO = levelSO;
            model.GeneralSO = generalSO;
            levelSO.StartMixComponents();
            playerSO.StartMixComponents();
            generalSO.StartMixComponents();
        }
        public override void OnDestroy()
        {
            Destroy(uiCanvas);
            RunnerApp.Interface.Deinit();
            levelSO.OnDestroy();
            playerSO.OnDestroy();
            generalSO.OnDestroy();
        }
    }

}
