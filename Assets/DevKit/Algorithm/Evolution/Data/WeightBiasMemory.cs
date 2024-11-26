using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Evolution
{
    /// <summary>
    /// 
    /// </summary>
    public class WeightBiasMemory : ScriptableObject
    {
        [Serializable]
        public struct LayerWeightAndBias
        {
            public int inputCount;
            public int outputCount;
            public float[] weights;
            public float[] bias;
        }
        [Tooltip("各全连层的权重和偏置")]
        public LayerWeightAndBias[] WeiBiasArray;

        [Tooltip("全连接层的compute shader")]
        public ComputeShader affine;

        [Tooltip("激活函数的compute shader")]
        public ComputeShader activateFunc;

        [Tooltip("损失函数的compute shader")]
        public ComputeShader lossFunc;

        [Tooltip("当前损失函数在反向传播时是否要载入上次输出，用于sigmoid等函数")]
        public bool isLoadLastOutput;

        [Header("随机初始化权重")]
        [Tooltip("是否要随机初始化")]
        public bool isRandomWeightAndBias = false;

        [Tooltip("当前权重是否是训练成功后的")]
        public bool isFinishedWeightAndBias = false;

        [Tooltip("随机初始化的最大值和最小值")]
        public float minRandValue = -1, maxRandValue = 1;

        [Tooltip("是否随机化权重")]
        public bool isRandomBias = false;

        private void OnValidate()
        {
            if (isRandomWeightAndBias && !isFinishedWeightAndBias)
            {
                RandomWeightAndBias(ref WeiBiasArray, minRandValue, maxRandValue, isRandomBias);
                isRandomWeightAndBias = false;
            }
        }

        /// <summary>
        /// 随机初始化权重和偏置
        /// </summary>
        /// <param name="WeiBiasArray">被随机化的数层权重和偏置</param>
        /// <param name="minRandValue">最小随机值</param>
        /// <param name="maxRandValue">最大随机值</param>
        /// <param name="isRandomBias">偏置是否也要随机化，如果false则置0</param>
        public static void RandomWeightAndBias(ref LayerWeightAndBias[] WeiBiasArray, float minRandValue,
            float maxRandValue, bool isRandomBias = false)
        {
            var rand = new System.Random();
            foreach (var wb in WeiBiasArray)
            {
                float range = maxRandValue - minRandValue;
                // 初始化权重
                for (int i = 0; i < wb.weights.Length; ++i)
                {
                    wb.weights[i] = (float)(rand.NextDouble() * range + minRandValue); // 使用指定范围生成随机数
                }
                // 初始化偏置
                for (int i = 0; i < wb.bias.Length; ++i)
                {
                    wb.bias[i] = isRandomBias ? (float)(rand.NextDouble() * range + minRandValue) : 0;
                }
            }
        }

        /// <summary>
        /// 深拷贝所有层的权重与偏置
        /// </summary>
        /// <param name="source">拷贝源</param>
        /// <param name="target">目标处</param>
        public static void DeepCopyAllLayerWB(ref LayerWeightAndBias[] source, ref LayerWeightAndBias[] target)
        {
            for (int i = 0, j; i < source.Length; ++i)
            {
                var wb = target[i];
                for (j = 0; j < wb.weights.Length; ++j)
                {
                    wb.weights[j] = source[i].weights[j];
                }
                for (j = 0; j < wb.bias.Length; ++j)
                {
                    wb.bias[j] = source[i].bias[j];
                }
            }
        }

        /// <summary>
        /// 交换所有层的权重与偏置
        /// </summary>
        public static void DeepSwap(ref LayerWeightAndBias[] a, ref LayerWeightAndBias[] b)
        {
            float tp;
            for (int i = 0, j; i < a.Length; ++i)
            {
                var wb = b[i];
                for (j = 0; j < wb.weights.Length; ++j)
                {
                    tp = wb.weights[j];
                    wb.weights[j] = a[i].weights[j];
                    a[i].weights[j] = tp;
                }
                for (j = 0; j < wb.bias.Length; ++j)
                {
                    tp = wb.bias[j];
                    wb.bias[j] = a[i].bias[j];
                    a[i].bias[j] = tp;
                }
            }
        }
    }
}