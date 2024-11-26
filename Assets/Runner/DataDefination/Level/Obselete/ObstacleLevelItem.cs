using System.Collections;
using System.Collections.Generic;
using ScriptableObjectBase;
using UnityEngine;
namespace Runner
{
    public class ObstacleLevelItem : StraightPathLevelItem
    {
        protected override void ModelConfig(ModelSO model)
        {
            model.colliderType = ModelSO.ColliderType.Box;
            model.pivotType = ModelSO.PivotType.Bottom;
            model.isTrigger = false;
            model.isStatic = true;
        }
        protected override List<StraightPathLevelItemComponent> AddComponents() 
            => new List<StraightPathLevelItemComponent>() { CreateInstance<ObstacleComponent>() };
    }
}

