using System;
using UnityEngine;

namespace Modules.Skybox
{
    [Serializable]
    public class GradientSkyboxData : SkyboxData
    {
        public override SkyboxType skyboxType { get; } = SkyboxType.Gradient;

        [Header("Gradient Settings")] 
        public Color topColor = Color.white;
        public Color bottomColor = Color.yellow;
        public float saturation = 1f;
        public float intensity = 1f;
        
        public override void Apply()
        {
            Material gradientMaterial = new Material(Shader.Find("MoreMountains/MMSkybox"));
            gradientMaterial.SetColor("_TopColor", topColor);
            gradientMaterial.SetColor("_BottomColor", bottomColor);
            gradientMaterial.SetFloat("_Saturation", saturation);
            gradientMaterial.SetFloat("_Intensity", intensity);

            RenderSettings.skybox = gradientMaterial;

            ApplyCommonSettings();
        }

        private void ApplyCommonSettings()
        {
            RenderSettings.ambientMode = ambientMode;
            RenderSettings.ambientLight = ambientLightColor;
            RenderSettings.reflectionIntensity = reflectionIntensity;
            RenderSettings.defaultReflectionResolution = reflectionResolution;
            DynamicGI.UpdateEnvironment();
        }
    }
}