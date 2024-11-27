using Managers;
using UnityEngine;

namespace Modules.Skybox
{
    public class SphericalSkyboxData : SkyboxData
    {
        public override SkyboxType skyboxType { get; } = SkyboxType.SphericalPanoramic;
        
        [Header("Panoramic Image URL")]
        public Asset<Sprite> panoramicImage;

        public override async void Apply()
        {
            Sprite panoramicSprite = panoramicImage;
            if (panoramicSprite)
            {
                Material sphericalMaterial = new Material(Shader.Find("Skybox/Panoramic"))
                {
                    mainTexture = panoramicSprite.texture
                };
                sphericalMaterial.SetFloat("_Exposure", exposure);
                sphericalMaterial.SetFloat("_Rotation", rotation);

                RenderSettings.skybox = sphericalMaterial;
            }
            else
            {
                Debug.LogError("Panoramic image failed to load.");
            }

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