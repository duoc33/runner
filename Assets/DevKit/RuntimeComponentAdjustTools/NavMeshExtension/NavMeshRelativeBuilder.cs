using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace RuntimeComponentAdjustTools
{
    public class NavMeshIgnore : MonoBehaviour { }
    /// <summary>
    /// 通过将所有要烘焙的物体，放在一个父物体下，即可自动Runtime烘焙NavMesh。
    /// </summary>
    public class NavMeshRelativeBuilder
    {
        /// <summary>
        /// 代理对象的高度，小于这个高度，才会烘焙
        /// </summary>
        public float AgentHeight = 2;
        /// <summary>
        /// 垂直方向上，代理能够步行的高度。
        /// </summary>
        public float AgentClimb = 0.4f;
        /// <summary>
        /// 代理的宽度，换句话说就是烘焙的边缘距离。
        /// </summary>
        public float AgentRadius = 0.5f;
        [Range(0,60)]
        public float AgentSlope = 45f; //度数

        public Func<GameObject,bool> IgnoreFunc = null; //自定义会略烘焙的个体。

        NavMeshData m_NavMesh;
        NavMeshDataInstance m_Instance;
        Bounds m_bounds;
        List<NavMeshBuildSource> m_sources;

        public Bounds GetBounds() => m_bounds;



        /// <summary>
        /// terrain.gameobject, terrain  包含地形时，m_bounds就不是真正意义上地图的bounds
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="terrain"></param>
        /// <param name="isUseMesh"></param>
        private void InitNavMesh(GameObject gameObject, Terrain terrain = null, bool isUseMesh = true)
        {
            DestroyNavMesh();
            m_sources ??= new List<NavMeshBuildSource>();
            m_NavMesh = new NavMeshData();
            m_Instance = NavMesh.AddNavMeshData(m_NavMesh);
            m_bounds = BoundsTool.CalculateBounds(gameObject);
            CollectSources(ref m_sources, gameObject, isUseMesh);
            if (terrain != null)
            {
                var source = new NavMeshBuildSource();
                source.shape = NavMeshBuildSourceShape.Terrain;
                source.sourceObject = terrain.terrainData;
                source.transform = terrain.transform.localToWorldMatrix;
                source.area = 0;
                m_sources.Add(source);
            }
        }


        public void BuildNavMesh(GameObject gameObject, Terrain terrain = null, bool isUseMesh = true)
        {
            InitNavMesh(gameObject, terrain, isUseMesh);
            var defaultBuildSettings = NavMesh.GetSettingsByID(0);
            defaultBuildSettings.agentHeight = AgentHeight;
            defaultBuildSettings.agentClimb = AgentClimb;
            defaultBuildSettings.agentRadius = AgentRadius;
            defaultBuildSettings.agentSlope = AgentSlope;
            NavMeshBuilder.UpdateNavMeshData(m_NavMesh, defaultBuildSettings, m_sources, m_bounds);
        }

        public AsyncOperation BuildNavMeshAsync(GameObject gameObject, Terrain terrain = null, bool isUseMesh = true)
        {
            InitNavMesh(gameObject, terrain, isUseMesh);
            var defaultBuildSettings = NavMesh.GetSettingsByID(0);
            defaultBuildSettings.agentHeight = AgentHeight;
            defaultBuildSettings.agentClimb = AgentClimb;
            defaultBuildSettings.agentRadius = AgentRadius;
            defaultBuildSettings.agentSlope = AgentSlope;
            return NavMeshBuilder.UpdateNavMeshDataAsync(m_NavMesh, defaultBuildSettings, m_sources, m_bounds);
        }

        public void DestroyNavMesh()
        {
            m_Instance.Remove();
            m_NavMesh = null;
            m_sources?.Clear();
            m_bounds = default;
        }

        /// <summary>
        /// gameObject 包含所有需要烘焙物体的父物体
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="gameObject"></param>
        /// <param name="isUseMesh"></param>
        static void CollectSources(ref List<NavMeshBuildSource> sources, GameObject gameObject, bool isUseMesh = true)
        {
            sources.Clear();

            if (isUseMesh)
            {
                foreach (var meshFilter in gameObject.GetComponentsInChildren<MeshFilter>())
                {
                    if (meshFilter == null || meshFilter.sharedMesh == null) continue;
                    if(meshFilter.GetComponent<NavMeshIgnore>() != null) continue;
                    NavMeshBuildSource source = CreateMeshSource(meshFilter.sharedMesh, meshFilter.transform.localToWorldMatrix);
                    sources.Add(source);
                }
            }
            else
            {
                var colliders = gameObject.GetComponentsInChildren<Collider>();
                foreach (var collider in colliders)
                {
                    if(collider.GetComponent<NavMeshIgnore>()!=null) continue;
                    if(collider is MeshCollider)
                    {
                        var meshCollider = collider as MeshCollider;
                        NavMeshBuildSource source = CreateMeshSource(meshCollider.sharedMesh, meshCollider.transform.localToWorldMatrix);
                        sources.Add(source);
                    }
                    else
                    {
                        var source = new NavMeshBuildSource();
                        source.component = collider;
                        source.transform = collider.transform.localToWorldMatrix;
                        if (collider is BoxCollider) source.shape = NavMeshBuildSourceShape.Box;
                        else if (collider is SphereCollider) source.shape = NavMeshBuildSourceShape.Sphere;
                        else if (collider is CapsuleCollider) source.shape = NavMeshBuildSourceShape.Capsule;
                        sources.Add(source);
                    }
                }
            }
        }
        static NavMeshBuildSource CreateMeshSource(Mesh mesh, Matrix4x4 transform)
        {
            var source = new NavMeshBuildSource();
            source.shape = NavMeshBuildSourceShape.Mesh;
            source.sourceObject = mesh;
            source.transform = transform;
            source.area = 0;
            return source;
        }
    }
}
