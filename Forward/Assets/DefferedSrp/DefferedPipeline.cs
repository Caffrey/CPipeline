using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEngine.Rendering.Deffered
{

    public class RenderTextureManager
    {
        public int DepthBuffer
        {
            get { return _GDepth; }
        }

        public int ColorBuffer
        {
            get { return _GAlbedo; }
        }

        public int NormalBuffer
        {
            get { return _GNormal; }
        }

        public static int _GAlbedo = Shader.PropertyToID("_GAlbedo");
        public static int _GNormal = Shader.PropertyToID("_GNormal");
        public static int _GDepth = Shader.PropertyToID("_GDepth");
        readonly public static RenderTargetIdentifier _GAlbedo_RT = new RenderTargetIdentifier(_GAlbedo);
        readonly public static RenderTargetIdentifier _GNormal_RT = new RenderTargetIdentifier(_GNormal);
        readonly public static RenderTargetIdentifier _GDepth_RT = new RenderTargetIdentifier(_GDepth);

        public void GenerateRT(CommandBuffer cmd)
        {
            RenderTextureDescriptor depthDesc = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.Depth, 24);
            RenderTextureDescriptor colorDesc = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.ARGBHalf, 0);
            RenderTextureDescriptor normalDesc = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.ARGBHalf, 0);
            RenderTextureDescriptor DepthColorDesc = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.ARGBHalf, 0);
            depthDesc.sRGB = false;
            normalDesc.sRGB = false;
            DepthColorDesc.sRGB = false;
            depthDesc.dimension = colorDesc.dimension = normalDesc.dimension = DepthColorDesc.dimension = TextureDimension.Tex2D;

            cmd.GetTemporaryRT(_GAlbedo, colorDesc, FilterMode.Bilinear);
            cmd.GetTemporaryRT(_GNormal, normalDesc, FilterMode.Point);
            cmd.GetTemporaryRT(_GDepth, depthDesc, FilterMode.Point);        
        }

        public void SetupGbufferToShader(CommandBuffer cmd)
        {
            cmd.SetGlobalTexture(_GAlbedo, _GAlbedo_RT);
            cmd.SetGlobalTexture(_GNormal, _GNormal_RT);
            cmd.SetGlobalTexture(_GDepth, _GDepth_RT);
        }

        public void ReleaseRT(CommandBuffer cmd)
        {
            cmd.Clear();
            cmd.ReleaseTemporaryRT(_GAlbedo);
            cmd.ReleaseTemporaryRT(_GNormal);
            cmd.ReleaseTemporaryRT(_GDepth);
        }
    }


    public class DefferedPipeline : RenderPipeline 
    {
        private CommandBuffer cmd;
        private DefferedPipelineAssets mAssets;

        private static Material copyColorMaterial;

        OpaeuePass opaquePass;
        GbufferPass gBufferPass;
        RenderTextureManager mRTManager;

        public DefferedPipeline(DefferedPipelineAssets asset)
        {
            mAssets = asset;
            cmd = new CommandBuffer();
            cmd.name = "Deffered";
            mRTManager = new RenderTextureManager();


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

        void DrawCamera(ScriptableRenderContext context, Camera camera)
        {

            //culling
            ScriptableCullingParameters cullingParams;
            camera.TryGetCullingParameters(out cullingParams);
            CullingResults cullResult = context.Cull(ref cullingParams);
            context.SetupCameraProperties(camera, false);

            //setup mrt
            CommandBuffer cmd1 = new CommandBuffer() { name = "mrt" };

            mRTManager.GenerateRT(cmd1);
            gBufferPass.SetupMRT(cmd1, mRTManager,context);
            cmd1.ClearRenderTarget(true, true, Color.black);
            context.ExecuteCommandBuffer(cmd1);
            cmd1.Release();

            context.DrawSkybox(camera);


            gBufferPass.Render(ref cullResult, ref context, cmd, camera);


            CommandBuffer cmd2 = new CommandBuffer() { name = "setbuffer" }; 
            mRTManager.SetupGbufferToShader(cmd2);
            context.ExecuteCommandBuffer(cmd2);
            cmd2.Release();
            //opaquePass.Render(ref cullResult, ref context, cmd, camera);

            CommandBuffer cmd3 = new CommandBuffer() { name = "blit" };
            cmd3.Clear();
            cmd3.BeginSample("Final Blit");

            SetupDebugMode();

            cmd3.Blit(BuiltinRenderTextureType.CameraTarget, BuiltinRenderTextureType.CameraTarget, copyColorMaterial);
            cmd3.EndSample("Final Blit");
            context.ExecuteCommandBuffer(cmd3);
            cmd3.Release();



#if UNITY_EDITOR
            if (camera.cameraType == CameraType.SceneView)
            {
                context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
            }
#endif
            CommandBuffer cmd4 = new CommandBuffer() { name = "release" };
            mRTManager.ReleaseRT(cmd4);
            context.ExecuteCommandBuffer(cmd4);
            cmd4.Release();
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


    }

}