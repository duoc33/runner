using System;
using Managers;
using ScriptableObjectBase;

namespace Runner
{
    public class RunnerData : ExecutorData
    {
        public AllConfig allConfig;
        public UIConfig uiConfig;

        public async override void Apply()
        {
            base.Apply();
        
            SOBase.InitServer();

            await allConfig.Download();
            
            if (uiConfig != null)
            {
                await uiConfig.Download();
                allConfig.SetUIConfig(uiConfig);
            }

            allConfig.StartMixComponents();
        }

        public override Type GetMonoType() => typeof(RunnerGame);
    }

    public class RunnerGame: ExecutorBehaviour<RunnerData> { }
}