// using System;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RuntimeComponentAdjustTools
{
    public class RandomTool
    {
        // static System.Random rand = new System.Random();
        /// <summary>
        /// 洗牌随机，打乱容器
        /// </summary>
        public static void Shuffle<T>(T[] array)
        {
            int n = array.Length;
            for (int i = n - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);  // 生成一个随机索引 , // 交换位置
                T temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }

        /// <summary>
        /// 加权选择随机 Weighted Choice , 根据权重随机选择一个元素
        /// </summary>
        public static T GetWeightedChoice<T>(List<(T item, int weight)> items)
        {
            // rand
            int totalWeight = 0;

            // 计算所有权重的总和
            foreach (var (item, weight) in items)
            {
                totalWeight += weight;
            }

            // 生成一个在总权重范围内的随机数
            int randomValue = UnityEngine.Random.Range(0, totalWeight);

            // 遍历列表并根据随机数选择元素
            foreach (var (item, weight) in items)
            {
                randomValue -= weight;
                if (randomValue < 0)
                {
                    return item;
                }
            }

            return default;  // 如果没有选中，返回默认值
        }

        /// <summary>
        /// 轮盘赌选择
        /// </summary>
        public static bool Roulette(float probability)
        {
            // 确保概率在0到1之间
            if (probability < 0 || probability > 1)
            {
                return false;
            }

            float randomValue = UnityEngine.Random.value;

            // 如果随机数小于给定的概率，视为"失败"
            return randomValue >= probability;
        }
    }
}

