using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEngine.Rendering.Deffered
{

    public class RenderTextureManager
    {
        public RenderTexture DepthBuffer
        {
            get { return _DepthBuffer; }
        }

        public RenderTexture ColorBuffer
        {
            get { return _ColorBuffer; }
        }

        public RenderTexture NormalBuffer
        {
            get { return _NormalBuffer; }
        }
        public RenderTexture DepthColorBuffer
        {
            get { return _DepthColorBuffer; }
        }

        RenderTexture _DepthBuffer;
        RenderTexture _ColorBuffer;
        RenderTexture _NormalBuffer;
        RenderTexture _DepthColorBuffer;
        
        public RenderTextureManager()
        {

        }
        public void GenerateRT()
        {
            RenderTextureDescriptor depthDesc = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.Depth, 24);
            RenderTextureDescriptor colorDesc = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.ARGBHalf, 24);
            RenderTextureDescriptor normalDesc = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.ARGBHalf, 0);
            RenderTextureDescriptor DepthColorDesc = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.ARGBHalf, 0);
            depthDesc.sRGB = false;
            normalDesc.sRGB = false;
            DepthColorDesc.sRGB = false;
            depthDesc.dimension = colorDesc.dimension = normalDesc.dimension = DepthColorDesc.dimension = TextureDimension.Tex2D;


            _DepthBuffer = RenderTexture.GetTemporary(depthDesc);
            _ColorBuffer = RenderTexture.GetTemporary(colorDesc);
            _NormalBuffer = RenderTexture.GetTemporary(normalDesc);
            _DepthColorBuffer = RenderTexture.GetTemporary(DepthColorDesc);
        }
        public void ReleaseRT()
        {
            if(_DepthBuffer)
            {
                RenderTexture.ReleaseTemporary(_DepthBuffer);
                RenderTexture.ReleaseTemporary(_ColorBuffer);
                RenderTexture.ReleaseTemporary(_NormalBuffer);
                RenderTexture.ReleaseTemporary(_DepthColorBuffer);
            }
            _DepthBuffer = null;
            _ColorBuffer = null;
            _NormalBuffer = null;
            _DepthBuffer = null;

        }

    }


    public class DefferedPipeline : RenderPipeline 
    {
        private CommandBuffer cmd;
        private DefferedPipelineAssets mAssets;

        OpaeuePass opaquePass;
        GbufferPass gBufferPass;
        RenderTextureManager mRRManager;



        public DefferedPipeline(DefferedPipelineAssets asset)
        {
            mAssets = asset;
            cmd = new CommandBuffer();
            cmd.name = "Deffered";
            mRRManager = new RenderTextureManager();
            
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

                mRRManager.ReleaseRT();
                mRRManager.GenerateRT();
                DrawCamera(context, camera);
                mRRManager.ReleaseRT();
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
            //gBufferPass.SetupMRT(cmd, mRRManager,context);
            gBufferPass.Render(ref cullResult, ref context, cmd, camera);
            //opaquePass.Render(ref cullResult, ref context, cmd, camera);

            context.DrawSkybox(camera);

#if UNITY_EDITOR
            if(camera.cameraType == CameraType.SceneView)
            {
                context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
            }
#endif

            context.Submit();
        }

    }
}