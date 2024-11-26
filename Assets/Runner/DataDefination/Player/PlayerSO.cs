using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using ScriptableObjectBase;
using UnityEngine;
namespace Runner
{
    public class PlayerSO : SOBase
    {
        public string PlayerUrl;

        public int InitCount = 1;
        public float MaxSpeed = 6;
        public float WalkSpeed = 3f;
        public float Acceleration = 8;

        public int DamageToOther = 1;
        public int CanAffordDamage = 1;

        public HumanAnimationSO PlayerAnimationSO;


        public float MoveSpeed = 5f; // Z轴移动速度
        public float HorizontalMoveSpeed = 5f; // X轴移动速度
        public float CameraVerticalLength = 5f; // 相机垂直距离
        public float CameraDistance = 13f; // 相机距离距离跟谁物体绝对距离



        [JsonIgnore]
        [NonSerialized]
        public ModelSO modelSO;

        public override async UniTask Download()
        {
            await DownloadAndPostProcessGameObject(PlayerUrl, PostProcess , DownloadAnimation);
        }
        private async UniTask<GameObject> DownloadAnimation(GameObject target)
        {
            if(PlayerAnimationSO!=null)
            {
                await PlayerAnimationSO.Download();
                PlayerAnimationSO.Apply(target);
            }
            return target;
        }
        private GameObject PostProcess(GameObject target)
        {
            modelSO ??=CreateInstance<ModelSO>();
            modelSO.colliderType = ModelSO.ColliderType.Box;
            modelSO.pivotType = ModelSO.PivotType.Bottom;
            modelSO.isTrigger = false;
            modelSO.isStatic = false;
            
            GameObject newGameObject = modelSO.PostProcess(target);
            
            

            return newGameObject;
        }

        public override void StartMixComponents()
        {
            PlayerGroupTarget = new GameObject("PlayerGroup");
            PlayerAgentGroup playerAgentGroup = PlayerGroupTarget.AddComponent<PlayerAgentGroup>();
            playerAgentGroup.playerSO = this;
            playerAgentGroup.GetArchitecture().GetModel<RunnerModel>().PlayerGroupTarget = playerAgentGroup.transform;
            PlayerGroupTargetMotion motion = PlayerGroupTarget.AddComponent<PlayerGroupTargetMotion>(); 
            motion.moveSpeed = MoveSpeed;
            motion.horizontalMoveSpeed = HorizontalMoveSpeed;
            motion.CameraVerticalLength = CameraVerticalLength;
            motion.CameraDistance = CameraDistance;
        }

        private GameObject PlayerGroupTarget;
        
        public GameObject Spawn(Transform parent)
        {
            GameObject player = Instantiate(modelSO.GetPrefab(),parent.position,parent.rotation);
            player.tag = "Player";
            //时间节点考虑一下
            NavMeshAgentComponent Agent = CreateInstance<NavMeshAgentComponent>(); // AgentController
            Agent.Acceleration = Acceleration;
            Agent.MaxSpeed = MaxSpeed;
            Agent.WalkSpeed = WalkSpeed;
            Agent.Decorate(player, modelSO.GetModelSize());

            BehavioursController behavioursController = player.AddComponent<BehavioursController>();
            behavioursController.CanAffordDamageCount = CanAffordDamage;
            behavioursController.Damge = DamageToOther; 
            behavioursController.ModelSize = modelSO.GetModelSize();
            return player;
        }
        public override GameObject Spawn()
        {
            return Instantiate(modelSO.GetPrefab());
        }
        
        public override void OnDestroy()
        {
            base.OnDestroy();
            Destroy(modelSO);
            Destroy(PlayerGroupTarget);
        }
    }
}

