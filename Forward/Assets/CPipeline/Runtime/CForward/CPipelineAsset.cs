using UnityEngine;
using UnityEngine.Rendering;

namespace CPipeline.Runtime
{
    [CreateAssetMenu(menuName = "Rendering/My Pipeline")]
    public class CPipelineAsset : RenderPipelineAsset
    {
        [SerializeField]
        public CShadowSetting ShadowSetting = default;

        public bool DynamicBatching = true;
        public bool GPUInstancing = true;
        public bool SPRBatcher = true;

        protected override RenderPipeline CreatePipeline()
        {
            return new CRenderingPipeline(this);
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




