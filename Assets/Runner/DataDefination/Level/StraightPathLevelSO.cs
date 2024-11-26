using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using RuntimeComponentAdjustTools;
using ScriptableObjectBase;
using UnityEngine;
namespace Runner
{
    public class StraightPathLevelSO : SOBase
    {
        public StraightPathLevelRuleSO Rule;
        public List<GroundLevelItem> Grounds = new List<GroundLevelItem>();
        private float total_ground = 0;

        
        public List<OnTriggerLevelItem> TriggerItems = new List<OnTriggerLevelItem>();
        private float total_trigger = 0;

        // [Range(0,1)]
        // public float ObstacleTypeProbability = 0.5f;
        // public List<ObstacleLevelItem> ObstacleItems = new List<ObstacleLevelItem>();
        // private float total_obstacle = 0;

        public List<LocomotionLevelItem> Locomotions = new List<LocomotionLevelItem>();
        private float total_locomotions = 0;

        /// <summary>
        /// 结尾出的最后Boss，或者是其他的物品
        /// </summary>
        public LocomotionLevelItem LevelEndingItem;

        private StraightPathLevelGenerator Generator;
        public override async UniTask Download()
        {
            total_ground = await DownloadItems(Grounds);
            total_trigger = await DownloadItems(TriggerItems);
            // total_obstacle = await DownloadItems(ObstacleItems);
            total_locomotions = await DownloadItems(Locomotions);
            totalTypeProbability = Rule.TriggerTypeProbability + Rule.LocomotionTypeProbability;
            if(LevelEndingItem!=null)
            {
                await LevelEndingItem.Download();
            }
        }
        public Bounds GetMapBounds() => navMeshBuilder.GetBounds();
        public override void StartMixComponents()
        {
            if(Generator!=null)
            {
                Destroy(Generator.gameObject);
            }
            navMeshBuilder = new NavMeshRelativeBuilder();
            Generator = new GameObject("Map").AddComponent<StraightPathLevelGenerator>();
        }
        private NavMeshRelativeBuilder navMeshBuilder;
        public async UniTask GenerateMap(Transform parent)
        {
            StraightPathLevelRuleSO.GridSizeValue = Rule.GridSize;
            UnityEngine.Random.InitState(Rule.Seed);
            Rule.SpawnGround(ChooseGround ,parent);
            await navMeshBuilder.BuildNavMeshAsync(parent.gameObject).ToUniTask();
            Rule.SpawnLevelItem(ChooseTpyeItem ,
                    new System.Func<StraightPathLevelItem>[] { ChooseTriggerItem , ChooseLocomotionItem } ,
                    ChooseFuncByProbability,LevelEndingItem,
                    parent);

            foreach (var item in Rule.LocomotionGroup)
            {
                LocomotionAgentGroup locomotionAgentGroup = item.gameObject.AddComponent<LocomotionAgentGroup>();
            }
        }

        private float totalTypeProbability = 0;
        private StraightPathLevelItem ChooseTpyeItem()
        {
            float randomValue = UnityEngine.Random.Range(0f, totalTypeProbability);
            if (randomValue <= Rule.TriggerTypeProbability)return ChooseTriggerItem();
            return ChooseLocomotionItem();
        }
        private Func<StraightPathLevelItem> ChooseFuncByProbability()
        {
            float randomValue = UnityEngine.Random.Range(0f, totalTypeProbability);
            if (randomValue <= Rule.TriggerTypeProbability) return ChooseTriggerItem;
            return ChooseLocomotionItem;
        }
        
        private GroundLevelItem ChooseGround() => ChooseItem(Grounds, total_ground);
        private OnTriggerLevelItem ChooseTriggerItem() => ChooseItem(TriggerItems, total_trigger);
        // private ObstacleLevelItem ChooseObstacleItem() => ChooseItem(ObstacleItems, total_obstacle);
        private LocomotionLevelItem ChooseLocomotionItem() => ChooseItem(Locomotions, total_locomotions);

        private static T ChooseItem<T>(List<T> items, float totalWeight) where T : StraightPathLevelItem {
            float randomValue = UnityEngine.Random.Range(0f, totalWeight);
            foreach (var item in items)
            {
                if(totalWeight == 0) return item;
                if (randomValue <= item.Properbility)
                {
                    return item;
                }
                randomValue -= item.Properbility;
            }
            return null; // 如果没有选择到，返回 null
        }

        private static async UniTask<float> DownloadItems<T>(List<T> items) where T : StraightPathLevelItem
        {
            float totalProperbility = 0;
            foreach (var item in items) 
            {
                totalProperbility += item.Properbility;
                await item.Download();
            }
            return totalProperbility;
        }
        public override void OnDestroy()
        {
            
            if(Generator!=null)
            {
                Destroy(Generator.gameObject);
            }

            navMeshBuilder?.DestroyNavMesh();
            
            if(Rule.LocomotionGroup != null && Rule.LocomotionGroup.Count > 0)
            {
                foreach (var item in Rule.LocomotionGroup)
                {
                    if(item!=null)
                    {
                        Destroy(item.gameObject);
                    }
                }
                Rule.LocomotionGroup?.Clear();
            }
            
            Destroy(Rule.LastLevelItem);
        }
    }
}