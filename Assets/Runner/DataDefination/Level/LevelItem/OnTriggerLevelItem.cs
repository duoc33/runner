using System.Collections;
using System.Collections.Generic;
using RuntimeComponentAdjustTools;
using ScriptableObjectBase;
using UnityEngine;
namespace Runner
{
    public class OnTriggerLevelItem : StraightPathLevelItem
    {
        public int PlayerCountChangeValue = 1;
        public bool IsNeedTextShow = true;
        public bool IsObstacle = false;
        public bool IsNeedDestroy = false;
        public float DestroyTime = 0f;
        protected override void ModelConfig(ModelSO model)
        {
            model.colliderType = ModelSO.ColliderType.Box;
            model.pivotType = ModelSO.PivotType.Bottom;
            model.isTrigger = true;
            model.isStatic = true;
        }
        protected override List<StraightPathLevelItemComponent> AddComponents()
        {
            List<StraightPathLevelItemComponent> components = new List<StraightPathLevelItemComponent>();            

            var scoreAndPlayerCountChangeComponent = CreateInstance<ScoreAndPlayerCountChangeComponent>();
            scoreAndPlayerCountChangeComponent.PlayerCountChangeValue = PlayerCountChangeValue;
            scoreAndPlayerCountChangeComponent.IsNeedTextShow = IsNeedTextShow;
            scoreAndPlayerCountChangeComponent.IsNeedDestroy = IsNeedDestroy;
            scoreAndPlayerCountChangeComponent.DestroyTime = DestroyTime;
            
            components.Add(scoreAndPlayerCountChangeComponent);
            if (IsObstacle)
            {
                components.Add(CreateInstance<ObstacleComponent>());
            }

            return components;
        }
        protected override void HandleOnInstantiate(GameObject gameObject, ModelSO model)
        {
            base.HandleOnInstantiate(gameObject, model);
        }
        protected override void HandlePostProcess(GameObject gameObject, ModelSO model)
        {
            gameObject.AddComponent<NavMeshIgnore>();
        }
    }
}
