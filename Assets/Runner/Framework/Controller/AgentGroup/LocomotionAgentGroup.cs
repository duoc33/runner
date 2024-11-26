using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Runner
{
    public class LocomotionAgentGroup : AbstractAgentGroupController
    {
        private List<BehavioursController> _selfGroup;
        private Transform playerTarget;
        void Start()
        {
            playerTarget = Data.PlayerGroupTarget;
        }
        protected override List<BehavioursController> GetAnotherGroup()
        {
            if(playerTarget == null) return null;
            PlayerAgentGroup agentGroup = playerTarget.GetComponent<PlayerAgentGroup>();
            if (agentGroup == null) return null;
            return agentGroup.SelfGroup;
        }

        protected override List<BehavioursController> GetSelfGroup()
        {
            if( _selfGroup == null )
            {
                _selfGroup = GetComponentsInChildren<BehavioursController>()?.ToList();
            }
            return _selfGroup;
        }
        protected override void OnUpdate()
        {
            
        }
        protected override bool RunningCondition() 
        {
            if(playerTarget==null)
            {
                playerTarget = Data.PlayerGroupTarget;
            }
            return playerTarget == null? false : CheckDistanceTo(playerTarget);
        } 
        protected override void WhenSelfCountEmpty()
        {
           return;
        }
    }
}

