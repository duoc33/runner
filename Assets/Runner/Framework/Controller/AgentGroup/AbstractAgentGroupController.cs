using System.Collections.Generic;
using UnityEngine;
namespace Runner
{
    /// <summary>
    /// 群体代理行为抽象类
    /// </summary>
    public abstract class AbstractAgentGroupController : RunnerController
    {
        private float CheckDistance = 10f;
        protected abstract bool RunningCondition();
        protected abstract void WhenSelfCountEmpty();
        protected abstract List<BehavioursController> GetSelfGroup();
        protected abstract List<BehavioursController> GetAnotherGroup();
        protected abstract void OnUpdate();
        protected virtual void OnAttackProcess(BehavioursController self)
        {
            
        }
        private List<BehavioursController> members;
        protected override void Awake()
        {
            base.Awake();
            CheckDistance = Data.levelSO.Rule.SpawnColumnSpacing / 2.0f;
        }
        void Update() => Run();
        private void Run()
        {
            if(!RunningCondition()) return;
            members =  GetSelfGroup();
            if( members == null || members.Count == 0) 
            {
                WhenSelfCountEmpty();
                return;
            }

            // 清理已死亡的角色
            members?.RemoveAll(member => member == null);

            // 获取敌人群组并指挥攻击
            List<BehavioursController> enemies = GetAnotherGroup();
            
            if (enemies!=null && enemies.Count > 0)
            {
                CommandAttack(ref enemies);
            }
            OnUpdate();
        }
        protected bool CheckDistanceTo(Transform target)=> Vector3.Distance(transform.position,target.position) < CheckDistance;

        private void CommandAttack(ref List<BehavioursController> group)
        {
            List<BehavioursController> remove = new List<BehavioursController>();
            foreach (var member in members)
            {
                if (member == null) continue;

                if(member.AgentGroupDetect( ref group , OnAttackProcess))
                {
                    remove.Add(member);
                }
            }
            foreach (var member in remove)
            {
                members.Remove(member);
            }
        }
    }
}
