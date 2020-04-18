using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


namespace CPipeline.Runtime
{
    public class COpapePass : CPipelineRenderPass
    {
        FilteringSettings m_FilterSetting;
        List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();
        bool m_IsOpaque;


        public COpapePass(string profilerTag,bool IsOpaque ,CRenderPassEvent evt, RenderQueueRange renderQueueRange,LayerMask layerMask)
        {
            m_ProfilerTag = profilerTag;
            renderPassEvent = evt;
            m_ShaderTagIdList.Add(new ShaderTagId("CForward"));
            m_ShaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
            m_FilterSetting = new FilteringSettings(renderQueueRange, layerMask);
            m_IsOpaque = IsOpaque;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
            using (new ProfilingSample(cmd, m_ProfilerTag))
            {
                cmd.Clear(); 
                Camera camera = renderingData.cameraData.camera;
                var sortFlags = (m_IsOpaque) ? renderingData.cameraData.defaultOpaqueSortFlags : SortingCriteria.CommonTransparent;
                var drawSetting = CreateDrawSetting(m_ShaderTagIdList, ref renderingData, sortFlags);
                context.DrawRenderers(renderingData.cullResult, ref drawSetting, ref m_FilterSetting);

            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}