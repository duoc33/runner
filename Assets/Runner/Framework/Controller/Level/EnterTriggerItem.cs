using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
namespace Runner
{
    public class EnterTriggerItem : RunnerController
    {
        public int TriggerIndex;
        public int PlayerCountDelta;
        public Vector3 CenterOffset;
        public bool IsNeedDestroy;
        public float DestroyTime;
        public bool isTriggered;
        public Vector3 modelSize;
        TriggerEnterLevelItemCmd triggerCmd;
        static string PlayerTag = "Player"; 

        private List<EnterTriggerItem> triggers ;
        void Start()
        {
            isTriggered = false;
            triggerCmd = new TriggerEnterLevelItemCmd();
            triggers = Data.levelSO.Rule.Triggers[TriggerIndex];
        }
        
        void OnTriggerEnter(Collider other)
        {
            if(isTriggered) return;

            if(!other.CompareTag(PlayerTag)) return;
            foreach (var item in triggers)
            {
                item.isTriggered = true;
                if(item.IsNeedDestroy)
                {
                    if(item != null)
                    {
                        Destroy(item.gameObject,item.DestroyTime);
                    }
                    
                }
            }
            triggers.Clear();
            isTriggered = true;
            triggerCmd.triggerTrans = this.transform;
            triggerCmd.CenterOffset = CenterOffset;
            triggerCmd.AddCount = PlayerCountDelta;
            triggerCmd.triggerPos = transform.position;
            triggerCmd.modelSize = modelSize;
            this.SendCommand(triggerCmd);
            if(IsNeedDestroy)
            {
                Destroy(gameObject, DestroyTime);
            }
            // Destroy(gameObject);
        }

    }
    public class TriggerEnterLevelItemCmd : AbstractCommand
    {
        public Transform triggerTrans;
        public Vector3 triggerPos;
        public Vector3 CenterOffset;
        public int AddCount;
        public Vector3 modelSize;
        private RunnerModel runnerModel;
        private OnTriggerLevelItemEvent onTriggerLevelItem;
        protected override void OnExecute()
        {
            onTriggerLevelItem = new OnTriggerLevelItemEvent();
            onTriggerLevelItem.levelItem = triggerTrans;
            onTriggerLevelItem.CenterOffset = CenterOffset;
            onTriggerLevelItem.AddCount = AddCount;
            onTriggerLevelItem.triggerPos = triggerPos;
            if(runnerModel==null)
            {
                runnerModel = this.GetModel<RunnerModel>();
            }
            runnerModel.VFXSO?.PlayEnterTriggerVFX(triggerPos + CenterOffset);
            runnerModel.MusicSO?.PlayEnterTriggerMusic();
            this.SendEvent(onTriggerLevelItem);
        }
    }
    
    public struct OnTriggerLevelItemEvent 
    {
        public int AddCount;
        public Vector3 triggerPos;
        public Transform levelItem;
        public Vector3 CenterOffset;
    }
    
}


