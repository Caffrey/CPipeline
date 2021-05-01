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
        public bool DrawGizmo = true;
        public bool IsForward = false;
        public bool IsHDR = true;

        protected override RenderPipeline CreatePipeline()
        {
            return new DefferedPipeline(this);
        }

    }
}