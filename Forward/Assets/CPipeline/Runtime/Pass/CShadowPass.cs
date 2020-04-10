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
        { name = "fuck"};
        
        public void Render(ref RenderContenxt context)
        {
            cmd.BeginSample("Shadow");
            cmd.Clear();

            ShadowDrawingSettings drawSetting = new ShadowDrawingSettings(context.cullResult,0);

            int texSize = (int)context.asset.ShadowSetting.directional.altasSize;

            var ren = new RenderTargetIdentifier(BuiltinRenderTextureType.CameraTarget);
            cmd.GetTemporaryRT(dirShadowAtlasId, texSize, texSize,24, FilterMode.Bilinear, RenderTextureFormat.Shadowmap);
            cmd.SetRenderTarget(dirShadowAtlasId, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            cmd.ClearRenderTarget(true, false, Color.clear);
            context.context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            

           // cmd.SetRenderTarget(ren);
          //  context.context.ExecuteCommandBuffer(cmd);
            
            cmd.EndSample("Shadow"); 
        }       

        public void CleanUp(ref RenderContenxt context)
        {
            context.cmd.Clear();
            context.cmd.ReleaseTemporaryRT(dirShadowAtlasId);
            context.context.ExecuteCommandBuffer(context.cmd);
        }


    }
}
