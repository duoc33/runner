using System;
using Managers;
using UnityEngine;

namespace Modules.Skybox
{
    /// <summary>
    /// Data for Cubemap Skybox.
    /// </summary>
    [Serializable]
    public class CubeMapSkyboxData : SkyboxData
    {
        public override SkyboxType skyboxType { get; } = SkyboxType.CubeMap;

        [Header("CubeMap Image URLs")]
        public Asset<Texture2D> front;
        public Asset<Texture2D> back;
        public Asset<Texture2D> left;
        public Asset<Texture2D> right;
        public Asset<Texture2D> up;
        public Asset<Texture2D> down;
        

        public override void Apply()
        {
            Texture2D frontTex = front;
            Texture2D backTex = back;
            Texture2D leftTex = left;
            Texture2D rightTex = right;
            Texture2D upTex = up;
            Texture2D downTex = down;

            if (frontTex && backTex && leftTex && rightTex && upTex && downTex)
            {
                Cubemap cubeMap = new Cubemap(frontTex.width, TextureFormat.RGBA32, false);
                cubeMap.SetPixels(frontTex.GetPixels(), CubemapFace.PositiveZ);
                cubeMap.SetPixels(backTex.GetPixels(), CubemapFace.NegativeZ);
                cubeMap.SetPixels(leftTex.GetPixels(), CubemapFace.NegativeX);
                cubeMap.SetPixels(rightTex.GetPixels(), CubemapFace.PositiveX);
                cubeMap.SetPixels(upTex.GetPixels(), CubemapFace.PositiveY);
                cubeMap.SetPixels(downTex.GetPixels(), CubemapFace.NegativeY);
                cubeMap.Apply();

                Material cubemapMaterial = new Material(Shader.Find("Skybox/Cubemap"));
                cubemapMaterial.SetTexture("_Tex", cubeMap);

                RenderSettings.skybox = cubemapMaterial;
            }
            else
            {
                Debug.LogError("CubeMap textures failed to load.");
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