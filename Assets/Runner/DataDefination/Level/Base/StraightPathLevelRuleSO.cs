using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using RuntimeComponentAdjustTools;
using UnityEngine;
using UnityEngine.AI;
namespace Runner
{
    public class StraightPathLevelRuleSO : ScriptableObject
    {
        public int Seed = 0;
        public Vector2Int MapSize ;     // 地图Size
        public Vector3 GridSize;       // 格子Size

        [JsonIgnore]
        [NonSerialized]
        public static Vector3 GridSizeValue;
        // [JsonIgnore]
        // [NonSerialized]
        // public static bool ForceSacleValue;

        [Range(0,1)]
        public float TriggerTypeProbability = 0.5f;

        [Range(0,1)]
        public float LocomotionTypeProbability = 0.5f;
        /// <summary>
        /// 生成间隔
        /// </summary>
        public float SpawnColumnSpacing = 10;
        /// <summary>
        /// 生成的物体同行间隔
        /// </summary>
        public float SpawnRowSpacing = 0.3f;

        /// <summary>
        /// 终点预留空间长度
        /// </summary>
        public float EndingRemainLength = 10;
        
        public GenerateRule RuleType = GenerateRule.MixAllTypeInRow;


        public int TriggerItemRowCountOnStart = 1;

        /// <summary>
        /// StrictSequentialSpawn 规则下的连续生成次数 , 1 则是默认的值，交替生成
        /// </summary>
        public int TriggerTypeSequenceSpawnCount = 1;

        public Vector3 AimStartPos() => new Vector3( ( GetMapRealWidth() - GridSize.x ) / 2 , GridSize.y , 0); 
        public float GetMapRealLength() => MapSize.y * GridSize.z;
        public float GetMapRealWidth() => MapSize.x * GridSize.x;

        public enum GenerateRule
        {
            /// <summary>
            /// 同一个行生成区域是否混合生成所有类型
            /// </summary>
            MixAllTypeInRow ,
            /// <summary>
            /// 是否根据类型概率交替生成不同类型的物品
            /// </summary>
            ConsecutiveAllTypeByProbability ,
            /// <summary>
            /// 先从Trigger类型开始,严格的交替生成Trigger和Locomotion类型的物品
            /// </summary>
            StrictSequentialSpawn ,
            /// <summary>
            /// 随机类型的顺序并
            /// </summary>
            RandomOrderSequentialSpawn ,
        }

        /// <summary>
        /// 地板生成
        /// </summary>
        /// <param name="ChooseGroundItem"></param>
        /// <param name="parent"></param>
        public void SpawnGround(Func<StraightPathLevelItem> ChooseGroundItem,Transform parent)
        {
            for (int i = 0; i < MapSize.x; i++)
            {
                for (int j = 0; j < MapSize.y; j++)
                {
                    Vector3 pos = new Vector3(i * GridSize.x, 0 , j * GridSize.z);
                    StraightPathLevelItem item = ChooseGroundItem.Invoke();
                    item.Spawn(pos,parent);
                }
            }
        }

        [JsonIgnore]
        [NonSerialized]
        public List<Transform> LocomotionGroup = new List<Transform>();

        [JsonIgnore]
        [NonSerialized]
        public List<List<EnterTriggerItem>> Triggers;

        /// <summary>
        /// 地板上的场景物体生成
        /// </summary>
        /// <param name="ChooseTypeItemFunc"></param>
        /// <param name="ListSpawnFuncs"></param>
        /// <param name="ChooseChooseFunc"></param>
        /// <param name="Boss"></param>
        /// <param name="parent"></param>
        public void SpawnLevelItem(Func<StraightPathLevelItem> ChooseTypeItemFunc , 
            Func<StraightPathLevelItem>[] ListSpawnFuncs , 
            Func<Func<StraightPathLevelItem>> ChooseChooseFunc,StraightPathLevelItem Boss ,Transform parent)
        {
            Vector3 pos = Vector3.zero;
            SpawnColumnSpacing = Mathf.Max(1,SpawnColumnSpacing);
            float length = GetMapRealLength() - EndingRemainLength;
            float width = GetMapRealWidth();
            float leftStartX = - GridSize.x / 2.0f;
            float BottomStartZ = - GridSize.z / 2.0f;
            float currentLength = 0;
            List<StraightPathLevelItem> tempRecords = new List<StraightPathLevelItem>();
            float spacing = SpawnRowSpacing;
            float midX = ( leftStartX + width ) / 2.0f;
            int index = 0;
            float spawnCount = 0;

            int triggerCountInInit = TriggerItemRowCountOnStart;
            bool Init = false;


            int triggerIndex = 0;
            Triggers = new List<List<EnterTriggerItem>>();

            while (currentLength < length)
            {
                currentLength += SpawnColumnSpacing;
                if(currentLength >= length) break;
                
                float currentPosX = leftStartX;
                float remainsWidth = width;
                float totalWidthForItem = 0;

                if (triggerCountInInit > 0)
                {
                    while (remainsWidth >= 0)
                    {
                        StraightPathLevelItem LevelItem = ListSpawnFuncs[0].Invoke();

                        if (LevelItem == null) break;

                        float itemWidth = LevelItem.model.GetModelSize().x;

                        remainsWidth -= itemWidth + spacing;

                        if (remainsWidth < 0) break;

                        totalWidthForItem += itemWidth + spacing;
                        
                        tempRecords.Add(LevelItem);
                    }

                    float offsetinit = midX - (leftStartX + totalWidthForItem) / 2;

                    Triggers.Add(new List<EnterTriggerItem>());

                    foreach (StraightPathLevelItem item in tempRecords)
                    {
                        float itemWidth = item.model.GetModelSize().x;
                        currentPosX += itemWidth;
                        pos = new Vector3(currentPosX - itemWidth / 2 + offsetinit, GridSize.y, currentLength + BottomStartZ);
                        
                        EnterTriggerItem enterTriggerItem = item.Spawn(pos, parent).GetComponent<EnterTriggerItem>();
                        if(enterTriggerItem!=null)
                        {
                            enterTriggerItem.TriggerIndex = triggerIndex;
                            Triggers[triggerIndex].Add(enterTriggerItem);
                        }
                        
                        currentPosX += spacing;
                    }

                    triggerIndex ++;

                    tempRecords.Clear();
                    triggerCountInInit -= 1;

                    continue;
                }

                if(!Init)
                {
                    Init = true;

                    if (RuleType == GenerateRule.RandomOrderSequentialSpawn)
                    {
                        RandomTool.Shuffle(ListSpawnFuncs);
                    }
                }


                Func<StraightPathLevelItem> ChooseFunc = null;
                if(RuleType == GenerateRule.ConsecutiveAllTypeByProbability)
                {
                    ChooseFunc = ChooseChooseFunc.Invoke();
                    // if(ChooseFunc)
                }
                
                while ( remainsWidth >= 0 )
                {
                    StraightPathLevelItem LevelItem = GetItemByRule(index,RuleType,ChooseTypeItemFunc,ListSpawnFuncs,ChooseFunc);
                    
                    if (LevelItem == null) break;

                    float itemWidth = LevelItem.model.GetModelSize().x;
                    
                    remainsWidth -= itemWidth + spacing;
                    
                    if(remainsWidth < 0) break ;
                    
                    totalWidthForItem += itemWidth + spacing;
                    tempRecords.Add(LevelItem);
                }

                float offset = midX - (leftStartX + totalWidthForItem) / 2 ;
                Transform group = null;

                Triggers.Add(new List<EnterTriggerItem>());
                foreach (StraightPathLevelItem item in tempRecords)
                {
                    float itemWidth = item.model.GetModelSize().x;
                    currentPosX += itemWidth ;
                    pos = new Vector3(currentPosX - itemWidth / 2 + offset, GridSize.y, currentLength + BottomStartZ);
                    if(item is LocomotionLevelItem)
                    {
                        if(group == null)
                        {
                            group = new GameObject("LocomotionGroup").transform;
                            LocomotionGroup.Add(group);
                            group.position = new Vector3(midX, GridSize.y, currentLength + BottomStartZ);
                        }
                        item.Spawn(pos, group);
                    }
                    else
                    {
                        // OnTriggerLevelItem
                        EnterTriggerItem enterTriggerItem = item.Spawn(pos, parent).GetComponent<EnterTriggerItem>();
                        if(enterTriggerItem!=null)
                        {
                            enterTriggerItem.TriggerIndex = triggerIndex;
                            Triggers[triggerIndex].Add(enterTriggerItem);
                        }
                    }
                    currentPosX += spacing ;
                }
                triggerIndex ++;
                
                if(index == 0) // 证明是Trigger类型
                {
                    spawnCount ++ ;
                    if(spawnCount >= Mathf.Max (1 , TriggerTypeSequenceSpawnCount)){
                        spawnCount = 0 ;
                    }
                    else{
                        index = -1;
                    }
                }

                index += 1;
                
                index %= ListSpawnFuncs.Length;
                
                tempRecords.Clear();
            }

            if(Boss != null)
            {
                currentLength -= SpawnColumnSpacing;
                pos = new Vector3( midX, GridSize.y, BottomStartZ + currentLength + (GetMapRealLength() - currentLength) / 2.0f);
                
                if (Boss is LocomotionLevelItem)
                {
                    Transform group = new GameObject("LocomotionGroup").transform;
                    group.position = pos;
                    LastLevelItem = Boss.Spawn(pos, group);
                    LastLevelItem.GetComponent<NavMeshAgent>().enabled = false;
                    LocomotionGroup.Add(group);
                }else{
                    LastLevelItem = Boss.Spawn(pos, parent);
                }
            }
        }
        [NonSerialized]
        [JsonIgnore]
        public GameObject LastLevelItem;
        private StraightPathLevelItem GetItemByRule(int curerntIndex , GenerateRule rule , 
            Func<StraightPathLevelItem> ChooseTypeItemFunc , 
            Func<StraightPathLevelItem>[] ListSpawnFuncs,
            Func<StraightPathLevelItem> ChooseChooseFunc)
        {
            switch (rule)
            {
                case GenerateRule.StrictSequentialSpawn:
                case GenerateRule.RandomOrderSequentialSpawn:
                    return ListSpawnFuncs[curerntIndex].Invoke();
                case GenerateRule.ConsecutiveAllTypeByProbability:
                    return ChooseChooseFunc.Invoke();
                case GenerateRule.MixAllTypeInRow:
                    return ChooseTypeItemFunc.Invoke();
            }
            return null;
        }
        private void OnDestroy() {
            LocomotionGroup?.Clear();
            Destroy(LastLevelItem);
        }
    }
}
