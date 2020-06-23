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
            _DepthBuffer = RenderTexture.GetTemporary(Screen.width, Screen.height, 24, RenderTextureFormat.Depth, RenderTextureReadWrite.Linear);
            _ColorBuffer = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Default);
            _NormalBuffer = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
            _DepthColorBuffer = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.RHalf, RenderTextureReadWrite.Linear);
        }
        public void ReleaseRT()
        {
            RenderTexture.ReleaseTemporary(_DepthBuffer);
            RenderTexture.ReleaseTemporary(_ColorBuffer);
            RenderTexture.ReleaseTemporary(_NormalBuffer);
            RenderTexture.ReleaseTemporary(_DepthColorBuffer);
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
           // gBufferPass.SetupMRT(cmd, mRRManager,context);
            gBufferPass.Render(ref cullResult, ref context, cmd, camera);
            //opaquePass.Render(ref cullResult, ref context, cmd, camera);

            context.DrawSkybox(camera);

#if UNITY_EDITOR
            context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
#endif

            context.Submit();
        }

    }
}