using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
namespace RuntimeComponentAdjustTools
{
    /// <summary>
    /// 美术上target的Pivot尽量需要归一化 , rotation = Quaternion.identity , position = Vector3.zero , scale = Vector3.one 
    /// </summary>
    public class ColliderTool
    {
        /// <summary>
        /// 传入某个父物体，根据父物体及其子物体的所有模型网格添加复杂的碰撞体
        /// convex 决定了是否为当前Mesh生成的粗糙的MeshCollider，设置为true, 性能更好。
        /// 门，窗等模型，可设置为false
        /// </summary>
        public static void SetMeshColliderConvex(GameObject target , bool convex = false , bool isTrigger = false)
        {
            MeshFilter[] meshFilters = target.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter meshFilter in meshFilters)
            {
                if (meshFilter.GetComponent<Collider>() == null)
                {
                    MeshCollider meshCollider = meshFilter.gameObject.AddComponent<MeshCollider>();

                    meshCollider.convex = convex;
                    if(convex) meshCollider.isTrigger = isTrigger;
                }
            }
        }
        
        public static BoxCollider SetBoxCollider(GameObject target ,bool isTrigger = false ,Bounds bounds = default)
        {
            if(bounds.Equals(default))
            {
                bounds = BoundsTool.CalculateBounds(target);
            }
            
            Vector3 size = bounds.size;
            Vector3 center = bounds.center;

            BoxCollider collider = target.AddComponent<BoxCollider>();
            collider.size = size;
            collider.center = center - target.transform.position;
            collider.isTrigger = isTrigger;

            return collider;
        }

        public static CapsuleCollider SetCapsuleCollider(GameObject target, bool isTrigger = false , Bounds bounds = default)
        {
            if(bounds.Equals(default))
            {
                bounds = BoundsTool.CalculateBounds(target);
            }
            
            Vector3 size = bounds.size;
            Vector3 center = bounds.center;

            CapsuleCollider collider = target.AddComponent<CapsuleCollider>();
            collider.radius = Mathf.Max( size.x / 2 , size.z / 2 );
            collider.center = center - target.transform.position;
            collider.height = size.y;
            collider.isTrigger = isTrigger;
            collider.direction = 1;

            return collider;
        }
        
        public static GameObject GetBoxColliderAndBottomPivot(GameObject target, bool isTrigger = false , Bounds bounds = default)
        {
            if(bounds.Equals(default))
            {
                bounds = BoundsTool.CalculateBounds(target);
            }

            GameObject pivot = BoundsTool.GetBestBottomPivot(target, bounds);
            SetBoxCollider(pivot, isTrigger , bounds);

            return pivot;
        }

        public static GameObject GetCapsuleColliderAndBottomPivot(GameObject target, bool isTrigger = false , Bounds bounds = default)
        {
            if(bounds.Equals(default))
            {
                bounds = BoundsTool.CalculateBounds(target);
            }

            GameObject pivot = BoundsTool.GetBestBottomPivot(target, bounds);
            SetCapsuleCollider(pivot, isTrigger , bounds);

            return pivot;
        }

    }
}
