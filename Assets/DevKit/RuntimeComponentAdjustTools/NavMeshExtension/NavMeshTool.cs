
using System;
using UnityEngine;
using UnityEngine.AI;
namespace RuntimeComponentAdjustTools
{
    public class NavMeshTool
    {
        public static void AddNavMeshComponents(GameObject src , Vector3 modelSize , Vector3 centerOffset , NavMeshObstacleShape shape = NavMeshObstacleShape.Box)
        {
            NavMeshObstacle navMeshObstacle = src.AddComponent<NavMeshObstacle>();
            navMeshObstacle.shape = shape;
            navMeshObstacle.size = modelSize;
            navMeshObstacle.radius = Mathf.Max(modelSize.x / 2.0f , modelSize.z / 2.0f );
            navMeshObstacle.height = modelSize.y;
            navMeshObstacle.center = centerOffset;
        }

        #region src 触发找其他点 , 且默认src在NavMesh上 , 按官方说法这么做也有性能考量
        /// <summary>
        /// 在某点为中心的球形内，寻找一个NavMesh有效位置
        /// detectRaidus，其最大距离应是代理高度的 2 倍。
        /// </summary>
        public static bool GetRandomSpherePointWithinNavMesh(Vector3 src, float SphereRadius, out Vector3 result, float detectRaidus = 1.0f, int maxIterations = 10)
        {
            // 随机生成一个方向，距离为10单位
            Vector3 dir = UnityEngine.Random.insideUnitSphere;

            if(GetNavMeshPointByDir(src , dir , SphereRadius, out result,detectRaidus,maxIterations))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 在某点为出发点的某个方向上，寻找一个NavMesh有效位置
        /// </summary>
        public static bool GetNavMeshPointByDir(Vector3 src , Vector3 dir, float distance, out Vector3 result,float detectRaidus = 1.0f,int maxIterations = 10)
        {
            // 计算目标点
            Vector3 targetPoint = src + dir.normalized * distance;

            // 尝试在目标点找到NavMesh有效位置
            NavMeshHit hit;
            if (NavMesh.SamplePosition(targetPoint, out hit, detectRaidus, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;  // 找到有效的NavMesh位置
            }

            // 如果目标点无效，可以沿着方向回退一段距离，直到找到有效的NavMesh点
            float step = distance / maxIterations;  // 步长可以根据需要调整
            for (int i = 0; i < maxIterations; i++) // 最大回退次数为10次
            {
                targetPoint -= dir.normalized * step;
                if (NavMesh.SamplePosition(targetPoint, out hit, detectRaidus, NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;  // 找到有效的NavMesh位置
                }
            }

            // 如果在回退过程中依然没有找到有效点，返回原点
            result = src;
            return false;  // 没有找到有效的NavMesh点
        }
        

        /// <summary>
        /// 找最近的边缘点
        /// </summary>
        /// <param name="src"></param>
        /// <param name="edge"></param>
        /// <returns></returns>
        public static bool FindClosestEdgePos(Vector3 src, out Vector3 edge)
        {
            if (NavMesh.FindClosestEdge(src, out var hit, NavMesh.AllAreas))
            {
                edge = hit.position;
                return true;
            }
            edge = Vector3.zero;
            return false;
        }


        /// <summary>
        /// 找到代理路径点
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public static Vector3[] GetNavMeshPath(Vector3 src, Vector3 dest)
        {
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(src, dest, NavMesh.AllAreas, path))
            {
                return path.corners;
            }
            return null;
        }
        #endregion

        #region 在整个NavMesh范围内随机找到一个点 , 增长detectRadius的方式
        public static bool GetRandomPositionInBounds(Bounds bounds, out Vector3 pos , int maxIterations = 10 )
        {
            Vector3 randomPoint = new Vector3(
                UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
                UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
            );
            return GetNearPosition(randomPoint, out pos , bounds.size.y , 5);
        }

        public static bool GetNearPosition(Vector3 pos , out Vector3 result , float detectRadius = 1.0f , int maxIterations = 0 ,float Multiplier = 2.0f , float step = 0f)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(pos, out hit, detectRadius , NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
            for (int i = 0; i < maxIterations; i++)
            {
                detectRadius *= Multiplier;
                detectRadius -= step;
                if (NavMesh.SamplePosition(pos, out hit, detectRadius, NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }
            result = pos;
            return false;
        }

        #endregion
    
    }
}


