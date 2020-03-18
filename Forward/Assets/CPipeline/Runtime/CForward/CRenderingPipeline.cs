using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace CPipeline.Runtime
{



    //TODO 
    /*
     * 0.RENDERING UNLIT  OK
     * 0.RENDERING LIT 
     * 
     * 1.RENDERING OP OK 
     * 2.RENDERING TRANSPARENT OK
     * 3.BASIC RENDERING SHADER LIB
     * 4.PBR RENDERING LIB
     * 
     */
    public class CRenderingPipeline : RenderPipeline
    {
        CommandBuffer cmd = new CommandBuffer
        {
            name = "SRP"
        };
        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
           BeginFrameRendering(context,cameras);

           foreach(var camera in cameras)
            {
                RenderCamera(context, camera);
            }

            EndFrameRendering(context, cameras);
        }

        static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlits");

        protected void RenderCamera(ScriptableRenderContext context, Camera camera)
        {
            BeginCameraRendering(context,camera);
            if(camera.cameraType == CameraType.SceneView)
            {
                ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
            }
            cmd.Clear();
            cmd.ClearRenderTarget(true, true, Color.clear);
            context.ExecuteCommandBuffer(cmd);

            //setup camera
            context.SetupCameraProperties(camera);

            //draw skybox
            context.DrawSkybox(camera);

            //culling
            ScriptableCullingParameters cullingParams;
            camera.TryGetCullingParameters(out cullingParams);

            var cullResult = context.Cull(ref cullingParams);

            //setup light
            SetupLight(ref cullResult);



            //draw opaque

            SortingSettings opaqueSortSetting = new SortingSettings(camera)
            {
                criteria = SortingCriteria.CommonOpaque
            };

            DrawingSettings opaqueDrawSetting = new DrawingSettings(unlitShaderTagId, opaqueSortSetting);
            FilteringSettings opaqueFilteringSeting = new FilteringSettings(RenderQueueRange.opaque);

            context.DrawRenderers(cullResult, ref opaqueDrawSetting, ref opaqueFilteringSeting);


            //draw transparent
            SortingSettings transparentSortSetting = new SortingSettings(camera)
            {
                criteria = SortingCriteria.CommonTransparent
            };

            DrawingSettings transparentDrawSetting = new DrawingSettings(unlitShaderTagId, transparentSortSetting);
            FilteringSettings transparentFilteringSeting = new FilteringSettings(RenderQueueRange.transparent);
            context.DrawRenderers(cullResult, ref transparentDrawSetting, ref transparentFilteringSeting);

            //render unlit


            context.Submit();

            EndCameraRendering(context,camera);
        }

        public void SetupLight(ref CullingResults cullResults)
        {

            NativeArray<VisibleLight> lights = cullResults.visibleLights;

            Vector4[] lightColors = new Vector4[lights.Length];
            Vector4[] lightDirection = new Vector4[lights.Length];

           

            cmd.SetGlobalFloat("_lightLength", lights.Length);
            for(int i = 0; i < lights.Length; i++)
            {
                lightColors[i] = lights[i].finalColor;
                lightDirection[i] = lights[i].localToWorldMatrix.GetColumn(2);
            }
           
            cmd.SetGlobalVectorArray("_lightColors", lightColors);
            cmd.SetGlobalVectorArray("_lightDirection", lightDirection);
        }

    }
}


