using System;
using JsonSubTypes;
using Managers;
using Newtonsoft.Json;
using UnityEngine;

namespace Modules.Lighting
{
    /// <summary>
    /// Enum to represent light modes.
    /// </summary>
    public enum LightMode
    {
        Basic,
        Shadow,
        Animated
    }

    [Serializable]
    [JsonConverter(typeof(JsonSubtypes), "lightMode")]
    [JsonSubtypes.KnownSubType(typeof(BasicLightData), LightMode.Basic)]
    [JsonSubtypes.KnownSubType(typeof(ShadowLightData), LightMode.Shadow)]
    [JsonSubtypes.KnownSubType(typeof(AnimatedLightData), LightMode.Animated)]
    public abstract class DirectionalLightData : ExecutorData
    {
        public virtual LightMode lightMode { get; }

        [Header("Common Settings")]
        public Color lightColor = Color.white;
        public float intensity = 1f;
        public Vector3 direction = Vector3.down;


        public override Type GetMonoType()
        {
            return typeof(DirectionalLight);
        }
        
        public abstract void Apply(Light light);
    }
}