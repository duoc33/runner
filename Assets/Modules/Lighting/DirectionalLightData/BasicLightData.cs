using System;
using UnityEngine;

namespace Modules.Lighting
{
    /// <summary>
    /// Basic directional light configuration.
    /// </summary>
    [Serializable]
    public class BasicLightData : DirectionalLightData
    {
        public override LightMode lightMode { get; } = LightMode.Basic;

        
        public override void Apply(Light light)
        {
            light.type = LightType.Directional;
            light.color = lightColor;
            light.intensity = intensity;

            if (light.transform != null)
            {
                light.transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
}