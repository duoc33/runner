using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CAlgorithm
{
    /// <summary>
    /// 使用容器记录某个情形下的最优值。
    /// </summary>
    public class KnapsackSolver
    {
        /// <summary>
        /// 最大价值
        /// </summary>
        /// <param name="CostCapacity"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static int Solve(int CostCapacity, List<KnapsackItem> items){
            int[] dp = new int[CostCapacity + 1];
            for (int i = 0; i < items.Count ; i++)
                for (int c = CostCapacity ; c >= items[i].Cost ; c--)
                    dp[c] = Mathf.Max(dp[c] , items[i].Value + dp[c - items[i].Cost]);
            return dp[CostCapacity];
        }

        /// <summary>
        /// 给定浮动容量和物品集合，返回最大价值的物品集合（物品索引）
        /// </summary>
        public static List<KnapsackItem> SolveList(int CostCapacity, List<KnapsackItem> items){
            int[] dp = new int[CostCapacity + 1];
            for (int i = 0; i < items.Count ; i++)
                for (int c = CostCapacity ; c >= items[i].Cost ; c--)
                    dp[c] = Mathf.Max(dp[c] , items[i].Value + dp[c - items[i].Cost]);


            // 回溯找到选中的物品
            List<KnapsackItem> selectedItems = new List<KnapsackItem>();
            int remainingCapacity = CostCapacity;

            for (int i = items.Count - 1; i >= 0; i--)
            {
                // 检查当前物品是否被选中
                if (remainingCapacity >= items[i].Cost && dp[remainingCapacity] == items[i].Value + dp[remainingCapacity - items[i].Cost])
                {
                    selectedItems.Add(items[i]);
                    remainingCapacity -= items[i].Cost; // 减少剩余容量
                }
            }

            return selectedItems;
        }

        public static List<int> SolveListIndex(int CostCapacity, List<KnapsackItem> items){
            int[] dp = new int[CostCapacity + 1];
            for (int i = 0; i < items.Count ; i++)
                for (int c = CostCapacity ; c >= items[i].Cost ; c--)
                    dp[c] = Mathf.Max(dp[c] , items[i].Value + dp[c - items[i].Cost]);



            // 回溯找到选中的物品
            List<int> selectedItems = new List<int>();
            int remainingCapacity = CostCapacity;

            for (int i = items.Count - 1; i >= 0; i--)
            {
                // 检查当前物品是否被选中
                if (remainingCapacity >= items[i].Cost && dp[remainingCapacity] == items[i].Value + dp[remainingCapacity - items[i].Cost])
                {
                    selectedItems.Add(i);
                    remainingCapacity -= items[i].Cost; // 减少剩余容量
                }
            }
            return selectedItems;
        }
        
        /// <summary>
        /// 背包描述
        /// </summary>
        public struct KnapsackItem
        {
            public int Value;
            public int Cost;
        }
    }
}

