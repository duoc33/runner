using Managers;
using UnityEngine;

namespace Modules.Lighting
{
    public class DirectionalLight : ExecutorBehaviour<DirectionalLightData>
    {
        private Light _light;
        private float _intensityDirection = 1f; // For animated intensity

        protected override void StartUp()
        {
            _light = PHUtils.TryAddComponent<Light>(gameObject);
            
            ApplyLightSettings();
        }
        
        private void ApplyLightSettings()
        {
            m_data.Apply(_light);
        }
        
        private void Update()
        {
            if (m_data is AnimatedLightData animatedData)
            {
                AnimateLight(animatedData);
            }
        }
        
        private void AnimateLight(AnimatedLightData animatedData)
        {
            // Rotate the light
            transform.Rotate(Vector3.up, animatedData.rotationSpeed * Time.deltaTime);

            // Change intensity
            _light.intensity += _intensityDirection * animatedData.intensitySpeed * Time.deltaTime;
            if (_light.intensity > animatedData.maxIntensity || _light.intensity < animatedData.minIntensity)
            {
                _intensityDirection *= -1f; // Reverse direction
            }
        }
    }
}