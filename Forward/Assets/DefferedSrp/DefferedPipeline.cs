using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEngine.Rendering.Deffered
{
    

    public class RenderTextureManager
    {
        private static RenderTextureManager _instance;
        public static RenderTextureManager instance
        {
            get {
                if(_instance == null)
                {
                    _instance = new RenderTextureManager();
                }
                return _instance;
            
            }
        }
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

        public static int _PostProcessingColor = Shader.PropertyToID("_S");
        public static int _PostProcessingColorSwap = Shader.PropertyToID("_S2");
        readonly public static RenderTargetIdentifier _PostProcessing_RT = new RenderTargetIdentifier(_PostProcessingColor);
        readonly public static RenderTargetIdentifier _PostProcessingColorSwap_RT = new RenderTargetIdentifier(_PostProcessingColorSwap);

        public RenderTextureManager() { }

        private RenderTargetIdentifier source;
        public RenderTargetIdentifier Source
        {
            get { return source; }
        }
        private RenderTargetIdentifier dest;

        public RenderTargetIdentifier Dest
        {
            get { return dest; }
        }
        public void Swap()
        {
            var temp = source;
            source = dest;
            dest = temp;
        }

        public void GenerateRT(CommandBuffer cmd,bool IsHDR)
        {
            RenderTextureDescriptor depthDesc = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.Depth, 24);
            RenderTextureDescriptor colorDesc = new RenderTextureDescriptor(Screen.width, Screen.height, IsHDR ? RenderTextureFormat.ARGBHalf: RenderTextureFormat.ARGB32, 0);
            RenderTextureDescriptor normalDesc = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.ARGBHalf, 0);
            RenderTextureDescriptor DepthColorDesc = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.ARGBHalf, 0);
            depthDesc.sRGB = false;
            normalDesc.sRGB = false;
            DepthColorDesc.sRGB = false;
            colorDesc.sRGB = !IsHDR;

            depthDesc.dimension = colorDesc.dimension = normalDesc.dimension = DepthColorDesc.dimension = TextureDimension.Tex2D;

            cmd.GetTemporaryRT(_GAlbedo, colorDesc, FilterMode.Bilinear);
            cmd.GetTemporaryRT(_GNormal, normalDesc, FilterMode.Point);
            cmd.GetTemporaryRT(_GDepth, depthDesc, FilterMode.Point);



            cmd.GetTemporaryRT(_PostProcessingColor, colorDesc, FilterMode.Bilinear);
            cmd.GetTemporaryRT(_PostProcessingColorSwap, colorDesc, FilterMode.Bilinear);
            source = _PostProcessingColor;
            dest = _PostProcessingColorSwap;
        }

        public void SetupGBuffer(CommandBuffer cmd,bool IsHDR)
        {
            GenerateRT(cmd, IsHDR);
            cmd.BeginSample("Setup MRT");

            RenderTexture tex = null;

            RenderTargetIdentifier[] rts = new RenderTargetIdentifier[2];
            rts[0] = ColorBuffer;
            rts[1] = NormalBuffer;
            cmd.SetRenderTarget(rts, DepthBuffer);

            cmd.EndSample("Setup MRT");
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
            cmd.ReleaseTemporaryRT(_PostProcessingColor);
            cmd.ReleaseTemporaryRT(_PostProcessingColorSwap);
        }
    }

    public class DefferedPipeline : RenderPipeline 
    {
        private CommandBuffer cmd;
        private DefferedPipelineAssets mAssets;

        private static Material copyColorMaterial;

        OpaeuePass opaquePass;
        GbufferPass gBufferPass;
        LighttingPass lightPass;

        public DefferedPipeline(DefferedPipelineAssets asset)
        {
            mAssets = asset;
            cmd = new CommandBuffer();
            cmd.name = "deferred";
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
            camera.allowHDR = mAssets.IsHDR;


            if (!mAssets.IsForward)
            {
                //Setup GBuffer To RenderTarget
                SetupGBuffer(cmd, context);

                //Rendering Opaque Pass
                gBufferPass.Render(ref cullResult, ref context, cmd, camera);



                //Rendering Opauqe LightingPass

                RenderingOpaqueLighting(cmd, context, ref cullResult);
                cmd.SetRenderTarget(RenderTextureManager.instance.Source, RenderTextureManager.instance.DepthBuffer);

                context.DrawSkybox(camera);

                cmd.Clear();
                
                cmd.Blit(RenderTextureManager.instance.Source, BuiltinRenderTextureType.CameraTarget, copyColorMaterial);
                context.ExecuteCommandBuffer(cmd);
#if UNITY_EDITOR
                if (camera.cameraType == CameraType.SceneView && mAssets.DrawGizmo)
                {
                    context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
                }
#endif
                CameraFrameEnd(cmd, context);
            }
            else
            {

#if UNITY_EDITOR
                if (camera.cameraType == CameraType.SceneView && mAssets.DrawGizmo)
                {
                    context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
                }
#endif
                
                opaquePass.Render(ref cullResult,ref context, cmd, camera);
                context.DrawSkybox(camera);
            }
            

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
            cmd.Clear();
            cmd.BeginSample("Setup GBuffer");
            RenderTextureManager.instance.SetupGBuffer(cmd,mAssets.IsHDR);
            cmd.ClearRenderTarget(true, true, Color.black);
            cmd.EndSample("Setup GBuffer");
            context.ExecuteCommandBuffer(cmd);
        }

        void CameraFrameEnd(CommandBuffer cmd, ScriptableRenderContext context)
        {
            cmd.Clear();
            RenderTextureManager.instance.ReleaseRT(cmd);
            context.ExecuteCommandBuffer(cmd);
        }

        void RenderingOpaqueLighting(CommandBuffer cmd, ScriptableRenderContext context,ref CullingResults cullResult)
        {
            SetupDebugMode();
            cmd.Clear();
            cmd.BeginSample("Opaque Lighting");

            RenderTextureManager.instance.SetupGbufferToShader(cmd);

            //cmd.SetRenderTarget(RenderTextureManager.instance.Source);
            //cmd.SetRenderTarget(BuiltinRenderTextureType.CameraTarget, RenderTextureManager.instance.DepthBuffer);
            lightPass.Execute(context, cmd,ref cullResult);
            
            cmd.EndSample("Opaque Lighting");
            context.ExecuteCommandBuffer(cmd);
        }

    }


}