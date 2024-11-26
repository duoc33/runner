using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Evolution
{
    //遗传算法中的个体，具体逻辑需继承该类扩展
    public class GeneticUnit : MonoBehaviour
    {
        public WeightBiasMemory memory;
        public float FitNess;
        public bool isOver;
        public virtual void ReStart()
        {
            isOver = false;
            FitNess = 0;
        }
        // private void FixedUpdate()
        // {
        //     if (isEndTrain) //如果选择结束训练，则保留当前最好的个体
        //     {
        //         SaveBest();
        //     }
        //     else if (TrainUnit.isOver) //如果当前训练单位的训练结束
        //     {
        //         parents[curIndex].fitness = TrainUnit.FitNess;
        //         TrainUnit.ReStart();
        //         //轮流将当前父本中个体权重与偏置赋给训练单位进行决策
        //         if (++curIndex < AllPopulation)
        //         {
        //             WeightBiasMemory.DeepCopyAllLayerWB(ref parents[curIndex].WB, ref TrainUnit.memory.WeiBiasArray);
        //         }
        //         //……
        //     }
        // }

        // //计算轮盘赌概率分布
        // private void CalcRouletteWheel()
        // {
        //     float totalFitness = 0f;
        //     for (int i = 0; i < parents.Length; i++)
        //     {
        //         totalFitness += parents[i].fitness;
        //     }
        //     float cumulativeSum = 0f;
        //     for (int i = 0; i < cumulativeProbabilities.Length; i++)
        //     {
        //         cumulativeSum += (parents[i].fitness / totalFitness);
        //         cumulativeProbabilities[i] = cumulativeSum;
        //     }
        // }

        // //轮盘赌随机下标
        // private int GetRouletteRandom()
        // {
        //     float rand = Random.value;
        //     // 选择个体
        //     for (int i = 0; i < cumulativeProbabilities.Length; i++)
        //     {
        //         if (rand < cumulativeProbabilities[i])
        //         {
        //             return i;
        //         }
        //     }
        //     // 如果没有找到，返回最后一个个体（通常不会发生）
        //     return cumulativeProbabilities.Length - 1;
        // }

        // private void GetChild()
        // {
        //     int p1, p2;
        //     for (int i = 0; i < parents.Length; i += 2)
        //     {
        //         p2 = p1 = GetRouletteRandom();
        //         var curWB = parents[i].WB;
        //         while (p1 == p2 && parents.Length > 1)
        //         {
        //             p2 = GetRouletteRandom();
        //         }
        //         for (int j = 0; j < curWB.Length; ++j)
        //         {
        //             var curW = curWB[j].weights;
        //             for (int k = 0; k < curW.Length; ++k)
        //             {
        //                 if (Random.value < 0.5)
        //                 {
        //                     children[i].WB[j].weights[k] = parents[p2].WB[j].weights[k];
        //                     if (i + 1 < children.Length)
        //                     {
        //                         children[i + 1].WB[j].weights[k] = parents[p1].WB[j].weights[k];
        //                     }
        //                 }
        //                 else
        //                 {
        //                     children[i].WB[j].weights[k] = parents[p1].WB[j].weights[k];
        //                     if (i + 1 < children.Length)
        //                     {
        //                         children[i + 1].WB[j].weights[k] = parents[p2].WB[j].weights[k];
        //                     }
        //                 }
        //                 if (Random.value < mutationRate) //随机变异，mutationRate为变异率
        //                 {
        //                     //mutationScale为变异的幅度，即变异带来的数值增减幅度
        //                     children[i].WB[j].weights[k] += Random.Range(-mutationScale, mutationScale);
        //                 }
        //                 if (i + 1 < children.Length && Random.value < mutationRate)
        //                 {
        //                     children[i + 1].WB[j].weights[k] += Random.Range(-mutationScale, mutationScale);
        //                 }
        //             }
        //             var curB = curWB[j].bias;
        //             for (int k = 0; k < curB.Length; ++k)
        //             {
        //                 if (Random.value < 0.5)
        //                 {
        //                     children[i].WB[j].bias[k] = parents[p2].WB[j].bias[k];
        //                     if (i + 1 < children.Length)
        //                     {
        //                         children[i + 1].WB[j].bias[k] = parents[p1].WB[j].bias[k];
        //                     }
        //                 }
        //                 else
        //                 {
        //                     children[i].WB[j].bias[k] = parents[p1].WB[j].bias[k];
        //                     if (i + 1 < children.Length)
        //                     {
        //                         children[i + 1].WB[j].bias[k] = parents[p2].WB[j].bias[k];
        //                     }
        //                 }
        //                 if (Random.value < mutationRate) //随机变异，mutationRate为变异率
        //                 {
        //                     //mutationScale为变异的幅度，即变异带来的数值增减幅度
        //                     children[i].WB[j].bias[k] += Random.Range(-mutationScale, mutationScale);
        //                 }
        //                 if (i + 1 < children.Length && Random.value < mutationRate)
        //                 {
        //                     children[i + 1].WB[j].bias[k] += Random.Range(-mutationScale, mutationScale);
        //                 }
        //             }
        //         }
        //     }
        // }

        // //在父代和子代组成的整体中选出适应度高的新父代
        // private void GetBest()
        // {
        //     for (int i = 0; i < totalPopulation.Length; ++i)
        //     {
        //         if (i < AllPopulation)
        //             totalPopulation[i] = parents[i];
        //         else
        //             totalPopulation[i] = children[i - AllPopulation];
        //     }
        //     Array.Sort(totalPopulation, (a, b) => b.fitness.CompareTo(a.fitness));
        // }


    }
}