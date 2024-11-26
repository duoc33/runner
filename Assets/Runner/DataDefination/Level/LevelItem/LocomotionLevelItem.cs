using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using RuntimeComponentAdjustTools;
using ScriptableObjectBase;
using UnityEngine;
namespace Runner
{
    public class LocomotionLevelItem : StraightPathLevelItem
    {
        public float MaxSpeed = 8;
        public float WalkSpeed = 1.5f;
        public float Acceleration = 8;
        public int DamageToOther = 1;
        public int CanAffordDamage = 1;
        public HumanAnimationSO NPCAnimationSO;
        public ModelSO.ColliderType colliderType = ModelSO.ColliderType.Capsule;
        protected override void ModelConfig(ModelSO model)
        {
            model.colliderType = colliderType;
            model.pivotType = ModelSO.PivotType.Bottom;
            model.isTrigger = false;
            model.isStatic = false;
        }
        protected override void HandlePostProcess(GameObject gameObject, ModelSO model)
        {
            
        }
        protected override async UniTask<GameObject> DowloadAnimation(GameObject target)
        {
            if(NPCAnimationSO!=null)
            {   
                await NPCAnimationSO.Download();
                NPCAnimationSO.Apply(target);
            }
            return target;
        }
        protected override void HandleOnInstantiate(GameObject gameObject, ModelSO model)
        {
            NavMeshAgentComponent Agent = CreateInstance<NavMeshAgentComponent>();
            Agent.Acceleration = Acceleration;
            Agent.MaxSpeed = MaxSpeed;
            Agent.WalkSpeed = WalkSpeed;
            Agent.Decorate(gameObject, model.GetModelSize());
            BehavioursController behavioursController = gameObject.AddComponent<BehavioursController>();
            behavioursController.CanAffordDamageCount = CanAffordDamage;
            behavioursController.Damge = DamageToOther; 
            behavioursController.ModelSize = model.GetModelSize();
        }
    }
}

