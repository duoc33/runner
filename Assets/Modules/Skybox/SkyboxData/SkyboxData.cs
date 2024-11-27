using System;
using Cysharp.Threading.Tasks;
using JsonSubTypes;
using Managers;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Rendering;

namespace Modules.Skybox
{
    /// <summary>
    /// Enum to represent Skybox types.
    /// </summary>
    public enum SkyboxType
    {
        CubeMap,
        SphericalPanoramic,
        Gradient
    }
    
    [Serializable]
    [JsonConverter(typeof(JsonSubtypes), "skyboxType")]
    [JsonSubtypes.KnownSubType(typeof(CubeMapSkyboxData), SkyboxType.CubeMap)]
    [JsonSubtypes.KnownSubType(typeof(SphericalSkyboxData), SkyboxType.SphericalPanoramic)]
    [JsonSubtypes.KnownSubType(typeof(GradientSkyboxData), SkyboxType.Gradient)]
    public abstract class SkyboxData : ExecutorData
    {
        public virtual SkyboxType skyboxType { get; }

        [Header("Common Settings")]
        public float exposure = 1f;
        public float rotation = 0f;

        [Header("Environment Settings")]
        public Color ambientLightColor = Color.white;
        public AmbientMode ambientMode = AmbientMode.Skybox;
        public float reflectionIntensity = 1f;
        public int reflectionResolution = 128;
        
        
        public override Type GetMonoType()
        {
            return typeof(Skybox);
        }
    }
}