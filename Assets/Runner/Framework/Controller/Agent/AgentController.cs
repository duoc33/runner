using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace Runner
{
    public class AgentController : MonoBehaviour
    {
        public float WalkSpeed;
        public float MaxSpeed;
        private NavMeshAgent agent;
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
        }
        public void SetWalk()=> agent.speed = WalkSpeed - 0.01f;
        public void SetRun() => agent.speed = MaxSpeed;
        
        public void SetStopping()
        {
            agent.speed = 0;
            agent.velocity = Vector3.zero;
            agent.destination = transform.position;
        }
        public void SetDestination(Vector3 destination, float stoppingDistance = 0 ,bool IgnoreStoppingDistance = false)
        {
            if(!agent.isOnNavMesh) return ;
            if(IgnoreStoppingDistance)
            {
                agent.stoppingDistance = 0;
            }
            else{
                agent.stoppingDistance = stoppingDistance;
            }
            agent.SetDestination(destination);
        }
        
        public bool IsStopped()=> agent.isStopped || agent.velocity.magnitude < 0.01f;
        
        public float GetCurrentNormalizedSpeed() => MapThree(agent.velocity.magnitude,WalkSpeed,MaxSpeed,0f,0.5f,1f);
        
        float MapThree(float va, float va1, float va2, float start, float mid, float end)
        {
            va = Mathf.Abs(va);
            float mappedValue = 0f;
            if (va < va1)
            {
                mappedValue = Mathf.Lerp(start, mid, va / va1);
            }
            else if (va >= va1 && va <= va2)
            {
                mappedValue = Mathf.Lerp(mid, end, (va - va1) / (va2 - va));
            }
            else if (va > va2)
            {
                mappedValue = 1f;
            }
            return mappedValue;
        }
    }
}

