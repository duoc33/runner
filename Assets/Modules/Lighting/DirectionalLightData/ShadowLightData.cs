using System;
using UnityEngine;

namespace Modules.Lighting
{
    /// <summary>
    /// Directional light configuration with shadow settings.
    /// </summary>
    [Serializable]
    public class ShadowLightData : DirectionalLightData
    {
        public override LightMode lightMode { get; } = LightMode.Shadow;

        [Header("Shadow Settings")]
        public LightShadows shadowType = LightShadows.Soft;
        public float shadowStrength = 1f;
        public float shadowDistance = 50f;
        public int shadowResolution = 1024;

        public override void Apply(Light light)
        {
            light.type = LightType.Directional;
            light.color = lightColor;
            light.intensity = intensity;

            if (light.transform != null)
            {
                light.transform.rotation = Quaternion.LookRotation(direction);
            }

            light.shadows = shadowType;
            light.shadowStrength = shadowStrength;
            QualitySettings.shadowDistance = shadowDistance;
            QualitySettings.shadowResolution = (ShadowResolution)shadowResolution;
        }
    }
}