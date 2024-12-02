using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ScriptableObjectBase;
using UnityEngine;
namespace Runner
{
    public class VFXSO : SOBase
    {
        public string OnPlayerHitVFX;
        public Vector3 PlayerHitVFXScaleOffset= Vector3.one;
        public string OnPlayerDeathVFX;
        public Vector3 PlayerDeathVFXScaleOffset = Vector3.one;
        public string OnPlayerSpawnVFX;
        public Vector3 PlayerSpawnVFXScaleOffset = Vector3.one;
        public string OnPlayerEnterTriggerVFX;
        public Vector3 PlayerEnterTriggerVFXScaleOffset = Vector3.one;

        // private GameObject playerHitVFX;
        // private GameObject playerDeathVFX;
        // private GameObject playerSpawnVFX;
        // private GameObject playerEnterTriggerVFX;

        private VFXPool hitPool;
        private VFXPool deathPool;
        private VFXPool spawnPool;
        private VFXPool triggerPool;
        private TimeSpan delay;
        private float destroyTime = 2f;
        public override async UniTask Download()
        {
            delay = TimeSpan.FromSeconds(destroyTime);
            hitPool = InitPool(await DownloadPrefab(OnPlayerHitVFX),PlayerHitVFXScaleOffset);
            deathPool = InitPool(await DownloadPrefab(OnPlayerDeathVFX),PlayerDeathVFXScaleOffset);
            spawnPool = InitPool(await DownloadPrefab(OnPlayerSpawnVFX),PlayerSpawnVFXScaleOffset);
            triggerPool = InitPool(await DownloadPrefab(OnPlayerEnterTriggerVFX),PlayerEnterTriggerVFXScaleOffset);
            
            // playerHitVFX = await DownloadPrefab(OnPlayerHitVFX);
            // playerDeathVFX = await DownloadPrefab(OnPlayerDeathVFX);
            // playerSpawnVFX = await DownloadPrefab(OnPlayerSpawnVFX);
            // playerEnterTriggerVFX = await DownloadPrefab(OnPlayerEnterTriggerVFX);
        }
        public void PlayPlayerHitVFX(Vector3 pos)
        {
            PlayVFX(hitPool, pos).SuppressCancellationThrow().Forget();
        }
        public void PlayDeathVFX(Vector3 pos)
        {
            PlayVFX(deathPool, pos).SuppressCancellationThrow().Forget();
        }
        public void PlayEnterTriggerVFX(Vector3 pos)
        {
            PlayVFX(triggerPool, pos).SuppressCancellationThrow().Forget();
        }
        public void PlaySpawnVFX(Transform player , Vector3 localPos)
        {
            PlayVFX(spawnPool , player , localPos).SuppressCancellationThrow().Forget();
        }

        /// <summary>
        /// 主要是应用于，有可能马上销毁的物体而产生特效的需求。
        /// </summary>
        /// <param name="vfx"></param>
        /// <param name="TargetGameObjectPos"></param>
        /// <param name="modelSize"></param>
        /// <param name="destroyTime"></param>
        private async UniTask PlayVFX(VFXPool vFXPool, Vector3 pos)
        {
            if (vFXPool == null)
            {
                return;
            }
            GameObject go = vFXPool.Get(pos);
            await UniTask.Delay(delay);
            vFXPool.Release(go);
        }

        private async UniTask PlayVFX(VFXPool vFXPool, Transform parent , Vector3 localPos)
        {
            if (vFXPool == null)
            {
                return;
            }
            GameObject go = vFXPool.Get(localPos, parent);
            await UniTask.Delay(delay);
            vFXPool.Release(go);
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            if(hitPool!=null)
            {
                Destroy(hitPool.gameObject);
            }
            if(deathPool!=null)
            {
                Destroy(deathPool.gameObject);
            }
            if(spawnPool!=null)
            {
                Destroy(spawnPool.gameObject);
            }
            if(triggerPool!=null)
            {
                Destroy(triggerPool.gameObject);
            }
        }


        private VFXPool InitPool(GameObject prefab,Vector3 scaleOffset)
        {
            if(prefab == null)
            {
                return null;
            }
            GameObject gameObject = new GameObject("VFXPool");
            VFXPool vFXPool = gameObject.AddComponent<VFXPool>();
            vFXPool.Prefab = prefab;
            vFXPool.ScaleOffset = scaleOffset;
            return vFXPool;
        }






         // private void PlayVFX(GameObject vfx, Vector3 pos , Vector3 offset = default , float scale = 1)
        // {
        //     if (vfx == null)
        //     {
        //         return;
        //     }
        //     // 实例化VFX对象
        //     GameObject vfxInstance = Instantiate(vfx);
        //     vfxInstance.transform.position = pos;
        //     ParticleSystem particleSystem = vfxInstance.GetComponent<ParticleSystem>();
        //     if (particleSystem != null)
        //     {
        //         // SetScale( vfxInstance.transform , modelSize );
        //         particleSystem.Play();
        //     }
        //     Destroy(vfxInstance, destroyTime);
        // }
        // private void SetScale(Transform target , Vector3 size)
        // {
        //     Vector3 originalSize = target.GetComponent<Renderer>()?.bounds.size ?? Vector3.one; // 获取特效对象的初始尺寸
            
        //     float scaleA = Mathf.Max(size.x, size.y, size.z);
            
        //     Vector3 scale = Vector3.one;
            
        //     if (originalSize != Vector3.zero)
        //     {
        //         scale = new Vector3(
        //             scaleA / originalSize.x,
        //             scaleA / originalSize.y,
        //             scaleA / originalSize.z
        //         );
        //     }
            
        //     target.localScale = scale;

        // }

        // private void AdjustParticleSystem(ParticleSystem particleSystem, Vector3 modelSize)
        // {
        //     // 计算缩放比例，基于目标模型尺寸
        //     float scaleFactor = Mathf.Min(modelSize.x, modelSize.y, modelSize.z); // 取模型最小轴为基准
        //     if(scaleFactor == 0) return;

        //     // 调整粒子系统的主要属性
        //     var mainModule = particleSystem.main;
        //     mainModule.simulationSpace = ParticleSystemSimulationSpace.Local;
        //     mainModule.scalingMode = ParticleSystemScalingMode.Hierarchy;

        //     mainModule.startSize = new ParticleSystem.MinMaxCurve(mainModule.startSize.constantMin * scaleFactor, mainModule.startSize.constantMax * scaleFactor);
        //     mainModule.startSpeed = new ParticleSystem.MinMaxCurve(mainModule.startSpeed.constantMin * scaleFactor, mainModule.startSpeed.constantMax * scaleFactor);

        //     // 调整发射器范围
        //     var shapeModule = particleSystem.shape;
        //     shapeModule.scale = new Vector3(
        //         shapeModule.scale.x * modelSize.x,
        //         shapeModule.scale.y * modelSize.y,
        //         shapeModule.scale.z * modelSize.z
        //     );

        //     // 递归调整子粒子系统
        //     foreach (Transform child in particleSystem.transform)
        //     {
        //         ParticleSystem childParticleSystem = child.GetComponent<ParticleSystem>();
        //         if (childParticleSystem != null)
        //         {
        //             AdjustParticleSystem(childParticleSystem, modelSize);
        //         }
        //     }
        // }

    }
}