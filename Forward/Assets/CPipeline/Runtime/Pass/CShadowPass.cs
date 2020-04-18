using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CPipeline.Runtime
{
    //setup light shadow property ,in lightsetting
    //setup shadow pass rendering
    //add shader caster
    //add shader use shadow
    public class CShadowPass
    {

        const string bufferName = "Shadow";
        public const int MAX_SHADOW_DIRECTIONAL_LIGHT_COUNT = 1;
        static int dirShadowAtlasId = Shader.PropertyToID("_DirectionalShadowAtlas");
        CommandBuffer cmd = new CommandBuffer()
        { name = "Shadow Pass"};

        public CShadowPass()
        {
        }
         
        public void Render(ref RenderingData context)
        {
            cmd.BeginSample("Shadow");
            cmd.Clear();

            ShadowDrawingSettings drawSetting = new ShadowDrawingSettings(context.cullResult,0);
           
            Matrix4x4 view;
            Matrix4x4 proj;
            ShadowSplitData splitData;
            context.cullResult.ComputeDirectionalShadowMatricesAndCullingPrimitives(0, 0, 1, Vector3.zero, (int)context.asset.ShadowSetting.directional.altasSize, 0f, out view, out proj, out splitData);
            drawSetting.splitData = splitData;
            cmd.SetViewProjectionMatrices(view, proj);

            int texSize = (int)context.asset.ShadowSetting.directional.altasSize;

            var ren = new RenderTargetIdentifier(BuiltinRenderTextureType.CameraTarget);
            cmd.GetTemporaryRT(dirShadowAtlasId, texSize, texSize,24, FilterMode.Bilinear, RenderTextureFormat.Shadowmap);
            cmd.SetRenderTarget(dirShadowAtlasId, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            cmd.ClearRenderTarget(true, false, Color.clear);
            context.context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            context.context.DrawShadows(ref drawSetting);

           // cmd.SetRenderTarget(ren);
          //  context.context.ExecuteCommandBuffer(cmd);
            
            cmd.EndSample("Shadow"); 
        }       

        public void CleanUp(ref RenderingData context)
        {
            context.cmd.Clear();
            context.cmd.ReleaseTemporaryRT(dirShadowAtlasId);
            context.context.ExecuteCommandBuffer(context.cmd);
        }


    }
}
