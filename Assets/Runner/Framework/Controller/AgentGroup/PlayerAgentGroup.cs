using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using QFramework;
using UnityEngine;
namespace Runner
{
    /// <summary>
    /// 玩家代理组，
    /// </summary>
    public class PlayerAgentGroup : AbstractAgentGroupController
    {
        private int ScorePerPlayer = 3;
        public PlayerSO playerSO;
        private List<BehavioursController> selfGroup;

        public List<BehavioursController> SelfGroup => selfGroup;
        
        private PlayerGroupTargetMotion motion;

        private List<Transform> locomotions;
        private int locomotionsCurrentIndex = 0;
        private List<BehavioursController> currentEnemies;
        private Transform currentLocomotion;
        
        
        void Start()
        {
            EndOnce = false;
            onGameOverCmd = new OnGameOverCmd();
            selfGroup = new List<BehavioursController>();
            IsStart = false;
            motion = GetComponent<PlayerGroupTargetMotion>();
            this.RegisterEvent<OnMapGenerated>(InitPlayerAgent).UnRegisterWhenGameObjectDestroyed(this);
            this.RegisterEvent<OnTriggerLevelItemEvent>(PlayerCountAdd).UnRegisterWhenGameObjectDestroyed(this);
        }

        private bool IsStart = false;
        
        private void InitPlayerAgent(OnMapGenerated e)
        {
            int count = playerSO.InitCount;
            count = Mathf.Max(count, 1);
            for (int i = 0; i < count ; i++) 
            {
                // Debug.Log(count);
                // Debug.Log("1");
                SpawnPlayer();
            }
            // Debug.Log(count);
            locomotions = Data.LocomotionGroup;
            locomotionsCurrentIndex = 0;
            Data.PlayerCount.Value = selfGroup.Count;
            IsStart = true;
            IsLocomotionEnd = false;
            
            motion.StartMove();
        }
        
        private void PlayerCountAdd(OnTriggerLevelItemEvent e)
        {
            selfGroup.RemoveAll(p => p==null);
            if(selfGroup.Count <=0)
            {
                this.SendCommand<OnGameOverCmd>();
                return;
            }
            if(e.AddCount==0) return;
            if(e.AddCount > 0)
            {
                for (int i = 0; i < e.AddCount; i++)
                {
                    SpawnPlayer();
                }
            }
            else if(e.AddCount < 0){
                for (int i = 0; i < -e.AddCount; i++)
                {
                    DestroyPlayer();
                }
            }
        }
        
        private bool IsLocomotionEnd = false;
        
        protected override List<BehavioursController> GetAnotherGroup()
        {
            if(motion.IsEnd) return null;
            if(IsLocomotionEnd) return null;
            
            if(locomotions == null || locomotions.Count ==0) return null;
            locomotionsCurrentIndex = Mathf.Clamp(locomotionsCurrentIndex,0,locomotions.Count);
            currentLocomotion = locomotions[locomotionsCurrentIndex];

            // CombatMusic

            if(CheckDistanceTo(currentLocomotion))
            {
                currentEnemies ??= currentLocomotion.GetComponentsInChildren<BehavioursController>()?.ToList();
                if(currentEnemies != null && currentEnemies.Count > 0)
                {
                    Data.MusicSO?.PlayCombatMusic();
                    motion.StopMove();
                    return currentEnemies;
                }
                else{ // 证明已经消灭完了
                    currentEnemies = null;
                    Data.MusicSO?.PlayCombatMusic(false);
                    locomotionsCurrentIndex ++;
                    if(locomotionsCurrentIndex >= locomotions.Count)
                    {
                        IsLocomotionEnd = true;
                    }
                    motion.StartMove();
                }
            }
            return null;
        }

        protected override List<BehavioursController> GetSelfGroup()=> selfGroup;

        protected override bool RunningCondition()
        {
            return IsStart;
        }

        protected override void WhenSelfCountEmpty()
        { 
            if(!EndOnce)
            {
                motion.StopMove();
                onGameOverCmd.isWin = false;
                this.SendCommand(onGameOverCmd);
                EndOnce = true;
            }
            
        }

        private OnGameOverCmd onGameOverCmd;

        private void SpawnPlayer()
        {
            BehavioursController player = playerSO.Spawn(transform).GetComponent<BehavioursController>();
            Data.VFXSO?.PlaySpawnVFX(player.transform , default);
            selfGroup.Add(player);
        }
        private void DestroyPlayer()
        {
            if(selfGroup.Count > 0)
            {
                BehavioursController player = selfGroup[selfGroup.Count - 1];
                Data.VFXSO?.PlayDeathVFX(player.transform.position);
                player.Death();
                selfGroup.RemoveAt(selfGroup.Count - 1);
            }
        }

        protected override void OnAttackProcess(BehavioursController self)
        {
            Data.VFXSO?.PlayPlayerHitVFX(self.transform.position + (self.transform.up * self.ModelSize.y) + (self.transform.forward * self.ModelSize.z));
        }
        private bool EndOnce = false;
        protected override void OnUpdate()
        {
            Data.PlayerCount.Value = selfGroup.Count;
            Data.Score.Value = selfGroup.Count * ScorePerPlayer;
            if((!EndOnce) && motion.IsEnd && Data.LastLevelItem == null)
            {
                EndOnce = true;
                onGameOverCmd.isWin = true;
                this.SendCommand(onGameOverCmd);
            }
        }

        void OnDestroy()
        {
            if(selfGroup==null || selfGroup.Count == 0) return;
            foreach (var player in selfGroup)
            {
                if(player==null) continue;
                Destroy(player.gameObject);
            }
        }
    }
}

