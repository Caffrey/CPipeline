using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


namespace UnityEngine.Rendering.Deffered
{
    public class GbufferPass
    { 
        static ShaderTagId[] unlitShaderTagId = {
            new ShaderTagId("GBuffer"),
        };

        public void SetupMRT(CommandBuffer cmd, RenderTextureManager rtmanager, ScriptableRenderContext context)
        {
            cmd.Clear();
            cmd.BeginSample("Setup MRT");


            cmd.SetRenderTarget(rtmanager.ColorBuffer.colorBuffer, rtmanager.DepthBuffer.depth, 1);

            context.ExecuteCommandBuffer(cmd);

            cmd.EndSample("Setup MRT");
        }

        public void Render(ref CullingResults cullResult, ref ScriptableRenderContext context, CommandBuffer cmd, Camera camera)
        {

            SortingSettings opaqueSortSetting = new SortingSettings(camera)
            {
                criteria = SortingCriteria.CommonOpaque
            };

            DrawingSettings opaqueDrawSetting = new DrawingSettings(unlitShaderTagId[0], opaqueSortSetting)
            {
                perObjectData = PerObjectData.LightIndices
            };

            FilteringSettings opaqueFilteringSeting = new FilteringSettings(RenderQueueRange.opaque);

            context.DrawRenderers(cullResult, ref opaqueDrawSetting, ref opaqueFilteringSeting);
        }

    }
}
