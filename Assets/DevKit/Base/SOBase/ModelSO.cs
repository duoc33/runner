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
            _postModel.isStatic = isStatic;
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