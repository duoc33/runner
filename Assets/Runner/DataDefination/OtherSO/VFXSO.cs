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
        public string OnPlayerDeathVFX;
        public string OnPlayerSpawnVFX;
        public string OnPlayerEnterTriggerVFX;

        private GameObject playerHitVFX;
        private GameObject playerDeathVFX;
        private GameObject playerSpawnVFX;
        private GameObject playerEnterTriggerVFX;
        public override async UniTask Download()
        {
            playerHitVFX = await DownloadPrefab(OnPlayerHitVFX);
            playerDeathVFX = await DownloadPrefab(OnPlayerDeathVFX);
            playerSpawnVFX = await DownloadPrefab(OnPlayerSpawnVFX);
            playerEnterTriggerVFX = await DownloadPrefab(OnPlayerEnterTriggerVFX);
        }

        public void PlayPlayerHitVFX(Vector3 pos , Vector3 offset = default, Vector3 modelSize = default)
        {
            PlayVFX(playerHitVFX, pos + offset , modelSize);
        }

        public void PlayDeathVFX(Vector3 pos , Vector3 offset = default ,Vector3 modelSize = default)
        {
            PlayVFX(playerDeathVFX, pos + offset , modelSize);
        }

        public void PlaySpawnVFX(Transform player,Vector3 offset = default ,Vector3 modelSize = default)
        {
            PlayVFX(playerSpawnVFX, player , offset , modelSize);
        }

        public void PlayEnterTriggerVFX(Vector3 pos , Vector3 offset = default ,Vector3 modelSize = default)
        {
            PlayVFX(playerEnterTriggerVFX, pos + offset , modelSize);
        }

        /// <summary>
        /// 主要是应用于，有可能马上销毁的物体而产生特效的需求。
        /// </summary>
        /// <param name="vfx"></param>
        /// <param name="TargetGameObjectPos"></param>
        /// <param name="modelSize"></param>
        /// <param name="destroyTime"></param>
        private void PlayVFX(GameObject vfx, Vector3 TargetGameObjectPos , Vector3 modelSize = default, float destroyTime = 2f)
        {
            if (vfx == null)
            {
                return;
            }
            // 实例化VFX对象
            GameObject vfxInstance = Instantiate(vfx, TargetGameObjectPos, Quaternion.identity);
            ParticleSystem particleSystem = vfxInstance.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                // SetScale( vfxInstance.transform , modelSize );
                particleSystem.Play();
            }
            Destroy(vfxInstance, destroyTime);
        }

        private void PlayVFX(GameObject vfx, Transform parent ,Vector3 offset = default ,Vector3 modelSize = default ,float destroyTime = 2f)
        {
            if (vfx == null)
            {
                return;
            }
            // 实例化VFX对象
            GameObject vfxInstance = Instantiate(vfx , parent);
            vfxInstance.transform.localPosition = Vector3.zero + offset;
            vfxInstance.transform.localRotation = Quaternion.identity;   
            ParticleSystem particleSystem = vfxInstance.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                // SetScale( vfxInstance.transform , modelSize );
                particleSystem.Play();
            }
            Destroy(vfxInstance, destroyTime);
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
        private void SetScale(Transform target , Vector3 size)
        {
            Vector3 originalSize = target.GetComponent<Renderer>()?.bounds.size ?? Vector3.one; // 获取特效对象的初始尺寸
            
            float scaleA = Mathf.Max(size.x, size.y, size.z);
            
            Vector3 scale = Vector3.one;
            
            if (originalSize != Vector3.zero)
            {
                scale = new Vector3(
                    scaleA / originalSize.x,
                    scaleA / originalSize.y,
                    scaleA / originalSize.z
                );
            }
            
            target.localScale = scale;

        }

        private void AdjustParticleSystem(ParticleSystem particleSystem, Vector3 modelSize)
        {
            // 计算缩放比例，基于目标模型尺寸
            float scaleFactor = Mathf.Min(modelSize.x, modelSize.y, modelSize.z); // 取模型最小轴为基准
            if(scaleFactor == 0) return;

            // 调整粒子系统的主要属性
            var mainModule = particleSystem.main;
            mainModule.simulationSpace = ParticleSystemSimulationSpace.Local;
            mainModule.scalingMode = ParticleSystemScalingMode.Hierarchy;

            mainModule.startSize = new ParticleSystem.MinMaxCurve(mainModule.startSize.constantMin * scaleFactor, mainModule.startSize.constantMax * scaleFactor);
            mainModule.startSpeed = new ParticleSystem.MinMaxCurve(mainModule.startSpeed.constantMin * scaleFactor, mainModule.startSpeed.constantMax * scaleFactor);

            // 调整发射器范围
            var shapeModule = particleSystem.shape;
            shapeModule.scale = new Vector3(
                shapeModule.scale.x * modelSize.x,
                shapeModule.scale.y * modelSize.y,
                shapeModule.scale.z * modelSize.z
            );

            // 递归调整子粒子系统
            foreach (Transform child in particleSystem.transform)
            {
                ParticleSystem childParticleSystem = child.GetComponent<ParticleSystem>();
                if (childParticleSystem != null)
                {
                    AdjustParticleSystem(childParticleSystem, modelSize);
                }
            }
        }

    }
}