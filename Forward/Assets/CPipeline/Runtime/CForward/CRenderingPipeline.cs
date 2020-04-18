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
        private CPipelineAsset m_PipelineConfig;

        RenderingData m_RenderContext;
        #region Pass

        CameraRender m_CameraRender;

        #endregion


        public CRenderingPipeline(CPipelineAsset asset)
        {
            m_PipelineConfig = asset;
            m_RenderContext = new RenderingData();
            m_CameraRender = new CameraRender();
        }

        void SetupRenderContext(ref ScriptableRenderContext context,ref CullingResults cullResult,Camera camera)
        {
            m_RenderContext.cmd = cmd;
            m_RenderContext.asset = m_PipelineConfig;
            m_RenderContext.cameraData.camera = camera;
            m_RenderContext.context = context;
            m_RenderContext.cullResult = cullResult;
        }


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

        static ShaderTagId[] unlitShaderTagId = {
            new ShaderTagId("Always"),
            new ShaderTagId("OpapeLit"),
            new ShaderTagId("TransparentLit"),
            new ShaderTagId("Transparent"),
            new ShaderTagId("Clip"),
            new ShaderTagId("VertexLM"),
            new ShaderTagId("SRPDefaultUnlits")
        };

        protected void RenderCamera(ScriptableRenderContext context, Camera camera)
        {
            m_CameraRender.Render(context, camera);
            return;



            BeginCameraRendering(context,camera);
            if(camera.cameraType == CameraType.SceneView)
            {
                ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
            }

           
            //setup camera



            //culling
            ScriptableCullingParameters cullingParams;
            camera.TryGetCullingParameters(out cullingParams);
            //setup shadow
            cullingParams.shadowDistance = Mathf.Min(m_PipelineConfig.ShadowSetting.maxDistance,camera.farClipPlane);

            
#if UNITY_EDITOR
            if (camera.cameraType == CameraType.SceneView)
            {
                ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
            }
#endif

            CullingResults cullResult = context.Cull(ref cullingParams);

            SetupRenderContext(ref context, ref cullResult,camera);



#if UNITY_EDITOR
            RenderGizmo(context, camera, GizmoSubset.PreImageEffects);
#endif

            RenderShadow(ref m_RenderContext);

            context.SetupCameraProperties(camera);
            cmd.Clear();
            cmd.ClearRenderTarget(true, true, Color.clear);
            context.ExecuteCommandBuffer(cmd);
            context.Submit();
            RenderObject(context,camera, ref cullResult);

            //draw skybox
            context.DrawSkybox(camera);

#if UNITY_EDITOR
            RenderGizmo(context, camera, GizmoSubset.PostImageEffects);
#endif


            Cleanup(ref m_RenderContext);
            context.Submit();

            EndCameraRendering(context,camera);
        }

        void Cleanup(ref RenderingData context)
        {
            m_lightData.Cleanup(ref context);
        }

        void RenderShadow(ref RenderingData context)
        {
            //setup light
            m_lightData.setupLight(context.context, ref context.cullResult, ref m_PipelineConfig.ShadowSetting);
           // m_lightData.RenderShadow(ref context);
        }

        void RenderObject(ScriptableRenderContext context, Camera camera , ref CullingResults cullResult)
        { 
          
            //draw opaque
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


            //draw transparent
            SortingSettings transparentSortSetting = new SortingSettings(camera)
            {
                criteria = SortingCriteria.CommonTransparent
            };

            DrawingSettings transparentDrawSetting = new DrawingSettings(unlitShaderTagId[0], transparentSortSetting);
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


