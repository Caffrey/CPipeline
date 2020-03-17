using UnityEngine;
using UnityEngine.Rendering;

namespace CPipeline.Runtime
{
    [CreateAssetMenu(menuName = "Rendering/My Pipeline")]
    public class CPipelineAsset : RenderPipelineAsset
    {
        protected override RenderPipeline CreatePipeline()
        {
            return new CRenderingPipeline();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
        }
    }
}




