using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using ScriptableObjectBase;
using UnityEngine;
namespace Runner
{
    public class GroundLevelItem : StraightPathLevelItem
    {
        protected override void ModelConfig(ModelSO model)
        {
            model.colliderType = ModelSO.ColliderType.Box;
            model.pivotType = ModelSO.PivotType.Bottom;
            model.isTrigger = false;
            model.isStatic = true;
        }
        protected override void HandleOnInstantiate(GameObject gameObject, ModelSO model)
        {
            float tolerance = 0.01f; // 设定一个容差值，根据需要进行调整
            Vector3 GridSize = StraightPathLevelRuleSO.GridSizeValue;
            Vector3 modelSize = model.GetModelSize();
            if (GridSize != Vector3.zero && modelSize!= Vector3.zero && (!modelSize.Equals(GridSize)))
            {
                // 检查各个维度的差距是否在容差范围内
                if (Mathf.Abs(GridSize.x - modelSize.x) > tolerance ||
                    Mathf.Abs(GridSize.y - modelSize.y) > tolerance ||
                    Mathf.Abs(GridSize.z - modelSize.z) > tolerance)
                {
                    // 计算缩放因子
                    Vector3 scaleFactor = new Vector3(
                        GridSize.x / modelSize.x,
                        GridSize.y / modelSize.y,
                        GridSize.z / modelSize.z
                    );

                    // 应用缩放
                    gameObject.transform.localScale = scaleFactor;
                }
            }
        }
    }
}

