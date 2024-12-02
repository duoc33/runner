using System;
using RuntimeComponentAdjustTools;
using UnityEngine;

namespace ScriptableObjectBase
{
    public class ModelSO : ScriptableObject
    {
        public PivotType pivotType;
        public ColliderType colliderType;
        public bool isTrigger = false; // 是否是触发器

        public bool isConvex = false; // 是否是凸多边形
        
        public bool isStatic = false; // 是否是静态物体
        
        public bool IsNeedRescaleSize = false; // 是否需要重新设置模型大小
        public Vector3 SizeInUnit = Vector3.one; // 模型单位大小

        public GameObject PostProcess(GameObject target)
        {
            if(target == null) return null;
            _postModel = null;
            Bounds bounds = BoundsTool.CalculateBounds(target);
            _size = bounds.size;
            switch (pivotType){
                case PivotType.Center:
                    _postModel = BoundsTool.GetBestTopPivot(target,bounds);
                    break;
                case PivotType.Bottom:
                    _postModel = BoundsTool.GetBestBottomPivot(target,bounds);
                    break;
                case PivotType.Top:
                    _postModel = BoundsTool.GetBestTopPivot(target,bounds);
                    break;
                default: 
                    break;
            }
            if(_postModel == null) _postModel = target;
            _centerOffset = bounds.center - _postModel.transform.position;
            switch (colliderType)
            {
                case ColliderType.Box:
                    ColliderTool.SetBoxCollider(_postModel,isTrigger,bounds);
                    break;
                case ColliderType.Capsule:
                    ColliderTool.SetCapsuleCollider(_postModel,isTrigger,bounds);
                    break;
                case ColliderType.Mesh:
                    ColliderTool.SetMeshColliderConvex(_postModel,isTrigger,isConvex);
                    break;
                default: 
                    break;
            }

            if(IsNeedRescaleSize)
            {
                float tolerance = 0.01f; // 设定一个容差值，根据需要进行调整
                if (SizeInUnit != Vector3.zero && _size != Vector3.zero && (!_size.Equals(SizeInUnit)))
                {
                    // 检查各个维度的差距是否在容差范围内
                    if (Mathf.Abs(SizeInUnit.x - _size.x) > tolerance ||
                        Mathf.Abs(SizeInUnit.y - _size.y) > tolerance ||
                        Mathf.Abs(SizeInUnit.z - _size.z) > tolerance)
                    {
                        // 计算缩放因子
                        Vector3 scaleFactor = new Vector3(
                            SizeInUnit.x / _size.x,
                            SizeInUnit.y / _size.y,
                            SizeInUnit.z / _size.z
                        );
                        _postModel.transform.localScale = scaleFactor;
                    }
                }
            }
            foreach (Transform child in _postModel.GetComponentsInChildren<Transform>())
            {
                child.gameObject.isStatic = isStatic;
            }
            
            return _postModel;
        }
        public Vector3 GetModelSize()=>_size;
        public GameObject GetPrefab() => _postModel;
        public Vector3 GetCenterOffset() => _centerOffset;




        [Serializable]
        public enum ColliderType {
            None,
            Box,
            Capsule,
            Mesh,
        }
        [Serializable]
        public enum PivotType {
            None,
            Center,
            Bottom,
            Top
        }
        
        private GameObject _postModel;
        private Vector3 _centerOffset;
        private Vector3 _size;
    }
}