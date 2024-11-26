using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace RuntimeComponentAdjustTools
{
    public class BoundsTool
    {
        // /// <summary>
        // /// 根据BoxCollider或Mesh,将Transform放置在Y方向上的最佳的位置
        // /// </summary>
        // /// <param name="target"></param>
        // /// <param name="hitPos"></param>
        // public static Vector3 PlaceModel(Transform target, Vector3 PlacePos , Bounds bounds = default)
        // {
        //     if(bounds.Equals(default))
        //     {
        //         bounds = CalculateBounds(target.gameObject);
        //         if(bounds.Equals(default))
        //         {
        //             return target.position;
        //         }
        //     }

        //     float diffHeight = (target.position.y - bounds.min.y) * 0.999f; // 修正高度差 , 主要是为了和地面有一定的接触

        //     PlacePos += target.up * diffHeight;

        //     return PlacePos;
        // }

        // public static void PlaceModelAccordingNormalAndYDirect(Transform target, Vector3 placePos, Vector3 placeNormal, bool isNormalAlign = true)
        // {
        //     // 确保法线是单位向量
        //     placeNormal.Normalize();
        //     if (isNormalAlign)
        //     {
        //         // 将目标的上方向与目标法线对齐
        //         target.rotation = Quaternion.FromToRotation(target.up, placeNormal) * target.rotation;
        //     }
        //     target.position = PlaceModel(target, placePos);
        // }

        // /// <summary>
        // /// 让Target的Y轴与hitPos的法线对齐方向，再将物体放置在hit位置
        // /// </summary>
        // /// <param name="target"></param>
        // /// <param name="PlacePos"></param>
        // /// <param name="PlaceNormal"></param>
        // public static (Vector3, Quaternion) GetModelAccordingNormalAndY(Transform target, Vector3 placePos, Vector3 placeNormal)
        // {
        //     // 确保法线是单位向量
        //     placeNormal.Normalize();

        //     // 将目标的上方向与目标法线对齐
        //     // Quaternion rotation = Quaternion.FromToRotation(target.up, placeNormal);
        //     // target.rotation = rotation * target.rotation;

        //     // PlaceModel(target,placePos);

        //     return (PlaceModel(target, placePos), Quaternion.FromToRotation(target.up, placeNormal));
        // }

        /// <summary>
        /// 缩放模型大小
        /// </summary>
        /// <param name="target"></param>
        /// <param name="scalePerUnit"></param>
        public static void SetModelInUnit(Transform target, float scalePerUnit = 1 , Bounds bounds = default)
        {
            if (scalePerUnit == 0) return;
            if (target.gameObject.isStatic)
            {
                return;
            }
            if (bounds.Equals(default))
            {
                bounds = CalculateBounds(target.gameObject);
            }

            Vector3 currentSize = bounds.size; // 当前尺寸


            // 计算缩放因子
            float maxCurrentSize = Mathf.Max(currentSize.x, currentSize.y, currentSize.z);

            float scaleFactor = scalePerUnit * 1.0f / maxCurrentSize;
            if (scaleFactor == Mathf.Infinity)
            {
                scaleFactor = 1;
            }

            // 应用缩放
            target.localScale = Vector3.one * scaleFactor;
        }
        public static void SetModelInUnit(Transform target, Vector3 modelSize , float scalePerUnit = 1)
        {
            if (scalePerUnit == 0) return;
            if (target.gameObject.isStatic)
            {
                return;
            }

            // 计算缩放因子
            float maxCurrentSize = Mathf.Max(modelSize.x, modelSize.y, modelSize.z);

            float scaleFactor = scalePerUnit * 1.0f / maxCurrentSize;
            if (scaleFactor == Mathf.Infinity)
            {
                scaleFactor = 1;
            }

            // 应用缩放
            target.localScale = Vector3.one * scaleFactor;
        }

        /// <summary>
        /// 计算物体的目标位置，使几何中心的正下方对齐目标点 Pos。
        /// </summary>
        /// <param name="pos">目标点</param>
        /// <returns>新的位置</returns>
        public static Vector3 CalculatePositionModelPivotDownward(Vector3 pos , Vector3 centerOffset , Vector3 modelSize)
        {
            return pos + Vector3.up * (modelSize.y / 2.0f) - centerOffset;
        }
        /// <summary>
        /// 计算物体的目标位置，使几何中心的正下方对齐目标点 Pos。
        /// </summary>
        /// <param name="pos">目标点</param>
        /// <returns>新的位置</returns>
        public static Vector3 CalculatePositionModelPivotUpward(Vector3 pos , Vector3 centerOffset , Vector3 modelSize)
        {
            return pos + Vector3.down * (modelSize.y / 2.0f) - centerOffset;
        }
        /// <summary>
        /// 计算物体的目标位置，使几何中心的正下方对齐目标点 Pos。
        /// </summary>
        /// <param name="pos">目标点</param>
        /// <returns>新的位置</returns>
        public static Vector3 CalculatePositionModelPivotCenter(Vector3 pos , Vector3 centerOffset)
        {
            return pos - centerOffset;
        }

        public static Vector3 CalculatePositionModelPivotForward(Vector3 pos , Vector3 centerOffset , Vector3 modelSize)
        {
            return pos + Vector3.forward * (modelSize.z / 2.0f) - centerOffset;
        }

        public static GameObject GetBestBottomPivot(GameObject target, Bounds bounds = default)
        {
            if (bounds == default)
            {
                bounds = CalculateBounds(target);
            }
            Vector3 bottomPos = bounds.center - Vector3.up * bounds.size.y / 2.0f ;
            GameObject pivot = new GameObject(target.name);
            pivot.transform.position = bottomPos;
            target.transform.SetParent(pivot.transform);
            return pivot;
        }

        public static GameObject GetBestCenterPivot(GameObject target, Bounds bounds = default)
        {
            if (bounds == default)
            {
                bounds = CalculateBounds(target);
            }
            Vector3 center = bounds.center;
            GameObject pivot = new GameObject(target.name);
            pivot.transform.position = center;
            target.transform.SetParent(pivot.transform);
            return pivot;
        }

        public static GameObject GetBestTopPivot(GameObject target, Bounds bounds = default)
        {
            if (bounds == default)
            {
                bounds = CalculateBounds(target);
            }
            Vector3 Top = bounds.center + Vector3.up * bounds.size.y / 2.0f ;
            GameObject pivot = new GameObject(target.name);
            pivot.transform.position = Top;
            target.transform.SetParent(pivot.transform);
            return pivot;
        }

        
        public static void FitToBounds(Camera camera, GameObject gameObject, float distance)
        {
            Bounds bounds = CalculateBounds(gameObject);
            FitToBounds(camera, bounds, distance);
        }
        
        /// <summary>
        /// 根据相机的属性和物体的大小，计算出相机与物体之间的合适距离，以便于将整个物体都显示在摄像机的视野内。
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="bounds"></param>
        /// <param name="distance"></param>
        public static void FitToBounds(Camera camera, Bounds bounds, float distance)
        {
            float magnitude = bounds.extents.magnitude;
            // 将 magnitude 除以视场的正切值（来计算在某个距离下相机需要移动的量），然后再乘以一个距离因子 distance，最终得到 num。
            float num = magnitude / (2f * Mathf.Tan(0.5f * camera.fieldOfView * (Mathf.PI / 180f))) * distance;
            if (!float.IsNaN(num))
            {
                camera.transform.position = new Vector3(bounds.center.x, bounds.center.y, bounds.center.z + num);
                camera.transform.LookAt(bounds.center);
            }
        }
        
        public static void FitToBounds(Camera camera, Bounds bounds, Quaternion rotation, float distance)
        {
            float magnitude = bounds.extents.magnitude;//从中心出发的距离边界最大值 ， 把想象为摄像机的视野一半的垂直长度
                                                       // camera.fieldOfView 是Vertical角度，它的Tan值，被extents.magnitude除，得到摄像机的视场距离模型边界一个距离的度量。
            float num = magnitude / (2f * Mathf.Tan(0.5f * camera.fieldOfView * (Mathf.PI / 180f))) * distance;
            if (!float.IsNaN(num))
            {
                camera.transform.position = bounds.center - rotation * Vector3.forward * num;
                camera.transform.LookAt(bounds.center);
            }
        }

        public static Bounds CalculateBounds(GameObject gameObject, bool localSpace = false)
        {
            Vector3 position = gameObject.transform.position;
            Quaternion rotation = gameObject.transform.rotation;
            Vector3 localScale = gameObject.transform.localScale;

            if (localSpace)
            {
                gameObject.transform.position = Vector3.zero;
                gameObject.transform.rotation = Quaternion.identity;
                gameObject.transform.localScale = Vector3.one;
            }

            Bounds result = default;
            Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
            if (componentsInChildren.Length != 0)
            {
                Bounds bounds = componentsInChildren[0].bounds;
                result.center = bounds.center;
                result.extents = bounds.extents;
                for (int i = 1; i < componentsInChildren.Length; i++)
                {
                    Renderer renderer = componentsInChildren[i];
                    Bounds bounds2 = renderer.bounds;
                    result.Encapsulate(bounds2);
                }
                // 如果边界的大小非常小，说明可能没有有效的物体在该边界内，或该边界的计算不够精确。
                if (result.size.magnitude < 0.001f)
                {
                    result = CalculatePreciseBounds(gameObject);
                }
            }
            else
            {
                result = CalculatePreciseBounds(gameObject);
            }

            if (localSpace)
            {
                gameObject.transform.position = position;
                gameObject.transform.rotation = rotation;
                gameObject.transform.localScale = localScale;
            }

            return result;
        }

        /// <summary>
        /// 重新计算模型Bounds，
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static Bounds CalculatePreciseBounds(GameObject gameObject)
        {
            Bounds result = default;
            bool flag = false;
            MeshFilter[] componentsInChildren = gameObject.GetComponentsInChildren<MeshFilter>();
            if (componentsInChildren.Length != 0)
            {
                result = GetMeshBounds(componentsInChildren[0].gameObject, componentsInChildren[0].sharedMesh);
                flag = true;
                for (int i = 1; i < componentsInChildren.Length; i++)
                {
                    result.Encapsulate(GetMeshBounds(componentsInChildren[i].gameObject, componentsInChildren[i].sharedMesh));
                }
            }

            SkinnedMeshRenderer[] componentsInChildren2 = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (componentsInChildren2.Length != 0)
            {
                Mesh mesh = new Mesh();
                //如果MeshFilter为0个，这里就需要找出SkinnedMeshRenderer第一个，进行初始化result
                if (!flag)
                {
                    componentsInChildren2[0].BakeMesh(mesh);
                    result = GetMeshBounds(componentsInChildren2[0].gameObject, mesh);
                }

                for (int j = 1; j < componentsInChildren2.Length; j++)
                {
                    //生成一个新的临时网格 ,计算出来的 SkinnedMeshRenderer的 Mesh 不会受到动画的影响（TPOS）。
                    componentsInChildren2[j].BakeMesh(mesh);
                    result = GetMeshBounds(componentsInChildren2[j].gameObject, mesh);
                }

                Object.Destroy(mesh);
            }

            return result;
        }

        /// <summary>
        /// mesh 就是 gameObject上的组件，或重新烘焙出来的
        /// 该方法根据Mesh顶点，当前物体对象，计算了该mesh的世界空间bounds
        /// </summary>
        private static Bounds GetMeshBounds(GameObject gameObject, Mesh mesh)
        {
            Bounds result = default;
            Vector3[] vertices = mesh.vertices;
            if (vertices.Length != 0)
            {
                result = new Bounds(gameObject.transform.TransformPoint(vertices[0]), Vector3.zero);
                for (int i = 1; i < vertices.Length; i++)
                {
                    result.Encapsulate(gameObject.transform.TransformPoint(vertices[i]));
                }
            }
            return result;
        }
    }
}
