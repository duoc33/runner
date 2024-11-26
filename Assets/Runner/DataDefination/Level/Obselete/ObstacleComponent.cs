using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace Runner
{
    public class ObstacleComponent : StraightPathLevelItemComponent
    {
        public override void DecorateWhenInstantiate(GameObject spawned, StraightPathLevelItem info)
        {
        }
        public override void DecorateWhenPostProcess(GameObject target, StraightPathLevelItem info)
        {
            NavMeshObstacle navMeshObstacle = target.AddComponent<NavMeshObstacle>();
            navMeshObstacle.shape = NavMeshObstacleShape.Box;
            navMeshObstacle.size = info.model.GetModelSize();
            navMeshObstacle.center = info.model.GetCenterOffset();
        }
    }
}
