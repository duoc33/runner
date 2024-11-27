using System;
using Managers;

namespace Modules.PostProcessing
{
    public class PostProcessingData : ExecutorData
    {
        public BloomConfig bloomConfig;
        public VignetteConfig vignetteConfig;
        public ColorAdjustmentsConfig colorAdjustmentsConfig;
        public DepthOfFieldConfig depthOfFieldConfig;
        public ChromaticAberrationConfig chromaticAberrationConfig;
        public LensDistortionConfig lensDistortionConfig;
        public MotionBlurConfig motionBlurConfig;
        public WhiteBalanceConfig whiteBalanceConfig;
        public ChannelMixerConfig channelMixerConfig;
        public FilmGrainConfig filmGrainConfig;
        
        public override void Apply()
        {

        }


        public override Type GetMonoType()
        {
            return typeof(PostProcessing);
        }
        
    }
    
    

}
