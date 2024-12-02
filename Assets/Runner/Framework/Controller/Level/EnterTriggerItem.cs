using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
namespace Runner
{
    public struct TriggerItemInfo
    {
        public int PlayerCountDelta;
        public Vector3 CenterOffset;
    }
    public class EnterTriggerItem : RunnerController
    {
        public bool IsNeedDestroy;
        public float DestroyTime;
        private bool isTriggered;
        public Vector3 modelSize;
        public TriggerItemInfo itemInfo;
        TriggerEnterLevelItemCmd triggerCmd;
        static string PlayerTag = "Player"; 
        void Start()
        {
            isTriggered = false;
            triggerCmd = new TriggerEnterLevelItemCmd();
        }
        void OnTriggerEnter(Collider other)
        {
            if(!other.CompareTag(PlayerTag) || isTriggered) return;
            isTriggered = true;
            triggerCmd.triggerTrans = this.transform;
            triggerCmd.CenterOffset = itemInfo.CenterOffset;
            triggerCmd.AddCount = itemInfo.PlayerCountDelta;
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


