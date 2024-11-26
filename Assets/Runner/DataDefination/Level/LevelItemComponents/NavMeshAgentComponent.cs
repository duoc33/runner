using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace Runner
{
    /// <summary>
    /// Agent Step Height 够了 ，navMeshSurface.GetBuildSettings()全局掌控
    /// </summary>
    public class NavMeshAgentComponent : StraightPathLevelItemComponent
    {
        /// <summary>
        /// 最大速度，游戏中的一单元/s ,
        /// </summary>
        public float MaxSpeed = 3.5f; // 2 ~ 8 , 人类的速度范围

        public float WalkSpeed = 1f; // 普通速度 , 一定是小于MaxSpeed的

        /// <summary>
        /// 每秒转动多少度
        /// </summary>
        public float AngularSpeed = 120;

        /// <summary>
        /// 加速度，游戏中的一单元/s^2 , Speed = 3.5, Acceleration = 8 , 也就是 Speed /Acceleration =   0.4375 秒到达最大速度 , 至于距离什么的物理公式算一下吧。
        /// </summary>
        public float Acceleration = 8; // 人类的加速度范围，不太了解。

        // /// <summary>
        // /// 代表导航代理（如AI角色）的半径，影响其在路径规划中的碰撞范围。可以用Bounds自适应该大小
        // /// </summary>
        // private float Radius;
        // /// <summary>
        // /// 模型高度匹配较好，同理使用bounds参数，设置高度范围
        // /// </summary>
        // private float Height;
        public void Decorate(GameObject spawned , Vector3 modelSize)
        {
            spawned.TryGetComponent(out NavMeshAgent agent);
            agent ??= spawned.AddComponent<NavMeshAgent>();
            agent.speed = MaxSpeed;
            agent.angularSpeed = AngularSpeed;
            agent.acceleration = Acceleration;

            agent.autoBraking = true;
            agent.autoTraverseOffMeshLink = true;
            agent.autoRepath = true;
            agent.areaMask = NavMesh.AllAreas;
            agent.radius = Mathf.Max(modelSize.x / 2.0f, modelSize.z / 2.0f);
            agent.height = modelSize.y;
            if (MaxSpeed < WalkSpeed)
            {
                WalkSpeed = MaxSpeed / 2.0f;
            }
            AgentController controller = spawned.AddComponent<AgentController>();
            controller.WalkSpeed = WalkSpeed;
            controller.MaxSpeed = MaxSpeed;
        }

        public override void DecorateWhenInstantiate(GameObject spawned, StraightPathLevelItem info)
        {
            Vector3 modelSize = info.model.GetModelSize();
            Decorate(spawned, modelSize);
        }

        public override void DecorateWhenPostProcess(GameObject target, StraightPathLevelItem info)
        {
        }
    }
}