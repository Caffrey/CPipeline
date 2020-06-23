using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Rendering.Deffered
{
    [CreateAssetMenu(menuName = "Rendering/Deffered Pipeline")]
    public class DefferedPipelineAssets : RenderPipelineAsset
    {

        protected override RenderPipeline CreatePipeline()
        {
            return new DefferedPipeline(this);
        }

    }
}