using UnityEngine;
using UnityEngine.Rendering;

namespace CPipeline.Runtime
{
    //TODO 
    /*
     * 0.RENDERING UNLIT
     * 0.RENDERING LIT
     * 
     * 1.RENDERING OP
     * 2.RENDERING TRANSPARENT
     * 3.BASIC RENDERING SHADER LIB
     * 4.PBR RENDERING LIB
     * 
     */
    public class CRenderingPipeline : RenderPipeline
    {

        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
           BeginFrameRendering(context,cameras);

           foreach(var camera in cameras)
            {
                RenderCamera(context, camera);
            }

            EndFrameRendering(context, cameras);
        }

        static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");

        protected void RenderCamera(ScriptableRenderContext context, Camera camera)
        {
            BeginCameraRendering(context,camera);

            //setup camera
            context.SetupCameraProperties(camera);

            //draw skybox
            context.DrawSkybox(camera);


            //culling
            ScriptableCullingParameters cullingParams;
            camera.TryGetCullingParameters(out cullingParams);

            var cullResult = context.Cull(ref cullingParams);



            //draw geo

            SortingSettings sortSetting = new SortingSettings(camera);
            DrawingSettings drawSetting = new DrawingSettings(unlitShaderTagId, sortSetting);
            FilteringSettings filteringSeting = new FilteringSettings(RenderQueueRange.all);


            context.DrawRenderers(cullResult, ref drawSetting, ref filteringSeting);
            
            



            //render unlit









            context.Submit();

            EndCameraRendering(context,camera);
        }


    }
}


