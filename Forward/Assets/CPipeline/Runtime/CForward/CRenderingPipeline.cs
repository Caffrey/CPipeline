using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CPipeline.Runtime
{
    //TODO 
    /*
     * 0.RENDERING UNLIT  OK
     * 0.RENDERING LIT 
     * mutlip light now SetGlobalVectorArray ok
     * setup perobject light ,direction light ok
     * and point light ,no shadow
     * setup light buffer in shader
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

        CLightData m_lightData = new CLightData();


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
            cmd.Clear();
            //setup camera

            context.SetupCameraProperties(camera);



            //culling
            ScriptableCullingParameters cullingParams;
            camera.TryGetCullingParameters(out cullingParams);

#if UNITY_EDITOR
            if (camera.cameraType == CameraType.SceneView)
            {
                ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
            }
#endif



            var cullResult = context.Cull(ref cullingParams);

#if UNITY_EDITOR
            RenderGizmo(context, camera, GizmoSubset.PreImageEffects);
#endif

            RenderObject(context,camera, ref cullResult);

            //draw skybox
            context.DrawSkybox(camera);

#if UNITY_EDITOR
            RenderGizmo(context, camera, GizmoSubset.PostImageEffects);
#endif
            context.Submit();

            EndCameraRendering(context,camera);
        }

        void RenderObject(ScriptableRenderContext context, Camera camera , ref CullingResults cullResult)
        { 
          

            //setup light
            m_lightData.setupLight(context, ref cullResult);

            //draw opaque
            SortingSettings opaqueSortSetting = new SortingSettings(camera)
            {
                criteria = SortingCriteria.CommonOpaque
            };

            DrawingSettings opaqueDrawSetting = new DrawingSettings(unlitShaderTagId, opaqueSortSetting)
            {
                perObjectData = PerObjectData.LightIndices
            };

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


        }

#if UNITY_EDITOR
        void RenderGizmo(ScriptableRenderContext context, Camera camera, GizmoSubset type)
        {
            if (Handles.ShouldRenderGizmos() ||camera.cameraType == CameraType.SceneView)
            {

                context.DrawGizmos(camera, type);
                context.DrawGizmos(camera, type);
            }
        }

#endif

    }
}


