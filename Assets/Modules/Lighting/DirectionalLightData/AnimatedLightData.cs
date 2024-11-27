using System;
using UnityEngine;

namespace Modules.Lighting
{
    /// <summary>
    /// Directional light configuration with animation settings.
    /// </summary>
    [Serializable]
    public class AnimatedLightData : DirectionalLightData
    {
        public override LightMode lightMode { get; } = LightMode.Animated;

        [Header("Animation Settings")]
        public float rotationSpeed = 10f;
        public float intensitySpeed = 0.5f;
        public float minIntensity = 0.5f;
        public float maxIntensity = 2f;

        public override void Apply(Light light)
        {
            // Animation-specific logic is handled at runtime (see controller).
        }
    }
}