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
            PlayVFX(hitPool, pos).SuppressCancellationThrow();
        }
        public void PlayDeathVFX(Vector3 pos)
        {
            PlayVFX(deathPool, pos).SuppressCancellationThrow();
        }
        public void PlayEnterTriggerVFX(Vector3 pos)
        {
            PlayVFX(triggerPool, pos).SuppressCancellationThrow();
        }
        public void PlaySpawnVFX(Transform player , Vector3 localPos)
        {
            PlayVFX(spawnPool , player , localPos).SuppressCancellationThrow();
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

    }
}