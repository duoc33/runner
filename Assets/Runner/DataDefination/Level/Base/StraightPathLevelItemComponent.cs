using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Runner
{
    public abstract class StraightPathLevelItemComponent : ScriptableObject 
    {
        public abstract void DecorateWhenPostProcess(GameObject target , StraightPathLevelItem info);
        public abstract void DecorateWhenInstantiate(GameObject spawned , StraightPathLevelItem info);
    }
}