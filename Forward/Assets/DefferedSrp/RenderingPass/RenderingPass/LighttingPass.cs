using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Rendering.Deffered
{
    public class LighttingPass
    {
        Material lightPassMaterial;

        public LighttingPass()
        {
            lightPassMaterial = new Material(Shader.Find("Hidden/LightPass"));
        }

        public void Execute(ScriptableRenderContext context, CommandBuffer cmd, ref CullingResults cullResult)
        {
            cmd.Clear();
            cmd.BeginSample("LightingPass");
            foreach(var light in cullResult.visibleLights)
            {
                Vector4 LD = light.light.transform.forward;

                if(light.lightType == LightType.Directional)
                {
                    LD = -light.light.transform.forward;
                    LD.w = 1;
                }
                else
                {
                    LD = light.light.transform.position;
                    LD.w = 0;
                }

                cmd.SetGlobalVector("_LightDireciton", LD);
                cmd.SetGlobalVector("_LightColor", light.finalColor);


                cmd.Blit(RenderTextureManager.instance.Source, RenderTextureManager.instance.Dest, lightPassMaterial);
                
                //cmd.Blit(BuiltinRenderTextureType.CameraTarget, BuiltinRenderTextureType.CameraTarget, lightPassMaterial);
                context.ExecuteCommandBuffer(cmd);
                
                RenderTextureManager.instance.Swap();
                cmd.Clear();
            }
            cmd.EndSample("LightingPass");

        }

    }
}
