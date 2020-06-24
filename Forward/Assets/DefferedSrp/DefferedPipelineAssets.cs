using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Rendering.Deffered
{
    [CreateAssetMenu(menuName = "Rendering/Deffered Pipeline")]
    public class DefferedPipelineAssets : RenderPipelineAsset
    {
        public enum DebugMode
        {
            None,
            Albedo,
            Normal,
            Depth,
            Depth01,
            DepthEye,
        }


        public DebugMode debugMode = DebugMode.None;

        protected override RenderPipeline CreatePipeline()
        {
            return new DefferedPipeline(this);
        }

    }
}