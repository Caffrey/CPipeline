using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEngine.Rendering.Deffered
{
    public class DefferedPipeline : RenderPipeline 
    {
        private DefferedPipelineAssets mAssets;

        private static Material copyColorMaterial;

        OpaeuePass opaquePass;
        GbufferPass gBufferPass;
        LighttingPass lightPass;
        ShadowPass shadowPass;

        public DefferedPipeline(DefferedPipelineAssets asset)
        {
            mAssets = asset;
            copyColorMaterial = new Material(Shader.Find("Hidden/copyColor"))
            {
                hideFlags = HideFlags.HideAndDontSave
            };

            InitPass();
        }

        void InitPass()
        {
            opaquePass = new OpaeuePass();
            gBufferPass = new GbufferPass();
            lightPass = new LighttingPass();
            shadowPass = new ShadowPass();
        }

        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            BeginFrameRendering(context, cameras);
            foreach(var camera in cameras)
            {
                BeginCameraRendering(context, camera);
                DrawCamera(context, camera);
                EndCameraRendering(context, camera);
            }
            EndFrameRendering(context, cameras);
        }

        RenderFrameContext m_RenderContext;
        void SetupFrame(ScriptableRenderContext context, Camera camera)
        {
            m_RenderContext = new RenderFrameContext(ref context);

            //Setup Camera
            ScriptableCullingParameters cullingParams;
            camera.TryGetCullingParameters(out cullingParams);
            context.SetupCameraProperties(camera, false);
            camera.allowHDR = mAssets.IsHDR;

            //Setup Culling
            m_RenderContext.CullingResult = context.Cull(ref cullingParams);
            m_RenderContext.camera = camera;
        }

        void PreRender(ref RenderFrameContext context)
        {
            //Render Shadow
            //Compute Stuff
        }

      
        void RenderGeometry(ref RenderFrameContext context)
        {
            //Render Opaticy Geometry
            //Setup GBuffer To RenderTarget
            SetupGBuffer(context.CommandBuffer, context.SRPContext);

            //Rendering Opaque Pass
            gBufferPass.Render(ref context.CullingResult, ref context.SRPContext, context.CommandBuffer, context.camera);

        }

        void RenderTransparent(ref RenderFrameContext context)
        {

        }

        void RenderLighting(ref RenderFrameContext context)
        {
            RenderTextureManager.instance.DepthResolve(context.SRPContext, context.CommandBuffer);
            RenderTextureManager.instance.SetupLightingPassGbuffer(context.SRPContext, context.CommandBuffer);
            RenderTextureManager.instance.SetupLightingPassRenderTarget(context.SRPContext, context.CommandBuffer);    
            lightPass.Execute(context.SRPContext, context.CommandBuffer, ref context.CullingResult); 
        }
         
        void RenderRealTransparent(ref RenderFrameContext context)
        {
            //Render Alpha Blend

            //Render Particle 
        }

      
        void RenderPostProcessing(ref RenderFrameContext context)
        {
            context.CommandBuffer.Blit(RenderTextureManager._PostProcessing_RT, BuiltinRenderTextureType.CameraTarget);
            context.ExecuteCommandBuffer();
            context.ClearCommandBuffer();
            //PostProcess

            //Bloom
            //Dof
            //Radial Blur
            //Uber
        }

        void RenderSceneView(ref RenderFrameContext context)
        {
#if UNITY_EDITOR
                if (context.camera.cameraType == CameraType.SceneView && mAssets.DrawGizmo)
                {
                    context.SRPContext.DrawGizmos(context.camera, GizmoSubset.PostImageEffects);
                }
#endif
        }

        void RenderSky(ref RenderFrameContext context)
        {
            context.SRPContext.DrawSkybox(context.camera);
        }

        void EndFrame(ref RenderFrameContext context)
        {
            RenderTextureManager.instance.ReleaseRT(context.CommandBuffer);
            context.ExecuteCommandBuffer();
            context.ClearCommandBuffer();
        }
         
        void DrawCamera(ScriptableRenderContext context, Camera camera)
        {

            SetupFrame(context, camera);
            PreRender(ref m_RenderContext);
            RenderShadow(ref m_RenderContext);
            RenderGeometry(ref m_RenderContext);        
            RenderTransparent(ref m_RenderContext);
            RenderLighting(ref m_RenderContext);
            RenderSky(ref m_RenderContext);
            RenderRealTransparent(ref m_RenderContext);           
            RenderPostProcessing(ref m_RenderContext);
            RenderSceneView(ref m_RenderContext);
            EndFrame(ref m_RenderContext);
            context.Submit();
        }

        public void SetupDebugMode()
        {
            copyColorMaterial.shaderKeywords = null;
            switch (mAssets.debugMode)
            {
                case DefferedPipelineAssets.DebugMode.None:
                case DefferedPipelineAssets.DebugMode.Albedo:

                    break;

                case DefferedPipelineAssets.DebugMode.Normal:
                    copyColorMaterial.EnableKeyword("_DEBUG_Normal");
                    break;

                case DefferedPipelineAssets.DebugMode.Depth:
                    copyColorMaterial.EnableKeyword("_DEBUG_DEPTH");
                    break;
                case DefferedPipelineAssets.DebugMode.Depth01:
                    copyColorMaterial.EnableKeyword("_DEBUG_DEPTH01");
                    break;

                case DefferedPipelineAssets.DebugMode.DepthEye:
                    copyColorMaterial.EnableKeyword("_DEBUG_DEPTH_EYE");
                    break;
            }
        }

        void SetupGBuffer(CommandBuffer cmd, ScriptableRenderContext context)
        {
            RenderTextureManager.instance.SetupGBuffer(context,cmd, mAssets.IsHDR);                    
        }

        void RenderShadow(ref RenderFrameContext context)
        {
            shadowPass.SetupShadowSetting(ref context);
            shadowPass.DrawShadow(context.SRPContext, context.CommandBuffer);
        }

        

    }


}