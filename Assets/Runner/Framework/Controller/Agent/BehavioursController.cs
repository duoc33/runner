using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Runner
{
    public class BehavioursController : MonoBehaviour
    {
        public Vector3 ModelSize;
        public int CanAffordDamageCount = 1;
        public int Damge = 1;
        private int _canAffordDamageCount;
        private AgentController agent;
        private HumanAnimatorController animator;
        
        void Start()
        {
            _canAffordDamageCount = Mathf.Max(1,CanAffordDamageCount);
            Damge = Mathf.Max(1,Damge);
            IsDeath = false;
            agent = GetComponent<AgentController>();
            animator = GetComponent<HumanAnimatorController>();
        }
        public void Move(GameObject target , float stopdistance = 0.5f)
        {
            agent?.SetRun();
            agent?.SetDestination(target.transform.position,stopdistance,false);
            animator?.SetSpeed(agent.GetCurrentNormalizedSpeed());
        }
        public void Attack(Action<BehavioursController> OnAttackHandle = null)
        {
            agent?.SetStopping();
            animator?.PlayAttack();
            OnAttackHandle?.Invoke(this);
        }
        public void AffordDamage(int damge)
        {
            _canAffordDamageCount -= 1;
            if(_canAffordDamageCount<=0)
            {
                Death();
            }
        }
        public void Death(float DestoryTime = 1.8f)
        {
            agent?.SetStopping();
            animator?.PlayDeath();
            Destroy(gameObject, DestoryTime);
            IsDeath = true;
        }

        private bool IsDeath = false;

        //
        public bool AgentGroupDetect(ref List<BehavioursController> enemies , Action<BehavioursController> OnAttackHandle = null)
        {
            if(IsDeath) return true;
            if(animator.IsAttackState()) return false;
            
            // 找到最近的敌人
            BehavioursController closestTarget = GetClosestEnemy( ref enemies);

            if (closestTarget != null)
            {
                float distance = Vector3.Distance(transform.position, closestTarget.transform.position);
                float selfRadius = Mathf.Max(ModelSize.x,ModelSize.z) / 2;
                float targetRadius = Mathf.Max(closestTarget.ModelSize.x,closestTarget.ModelSize.z) / 2;
                if (distance <= selfRadius + targetRadius + 0.2f) // 攻击距离
                {
                    Attack(OnAttackHandle);
                    AffordDamage(closestTarget.Damge);
                    closestTarget.Attack(OnAttackHandle);
                    closestTarget.AffordDamage(Damge);
                }
                else
                {
                    Move(closestTarget.gameObject,selfRadius + targetRadius);
                }
            }
            return false;
        }
        
        private BehavioursController GetClosestEnemy(ref List<BehavioursController> enemies)
        {
            BehavioursController closestEnemy = null;
            List<BehavioursController> targetsToRemove = new List<BehavioursController>();
            
            float shortestDistance = Mathf.Infinity;
            foreach (var target in enemies)
            {
                if (target == null) continue;
                
                if(target.IsDeath)
                {
                    targetsToRemove.Add(target);
                    continue;
                }
                float distance = Vector3.Distance(transform.position, target.transform.position);
                
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestEnemy = target;
                }
            }
            
            foreach (var target in targetsToRemove)
            {
                enemies.Remove(target);
            }
            
            targetsToRemove.Clear();

            return closestEnemy;
        }
    }


}

