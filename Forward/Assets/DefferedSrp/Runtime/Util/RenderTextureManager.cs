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
            get
            {
                if (_instance == null)
                {
                    _instance = new RenderTextureManager();
                }
                return _instance;

            }
        }

        public static int _GAlbedo = Shader.PropertyToID("_GAlbedo");
        public static int _GNormal = Shader.PropertyToID("_GNormal");
        public static int _GDepth = Shader.PropertyToID("_GDepth");
        public static int _DefferedDepth = Shader.PropertyToID("_DefferedDepth");
        public static int _PostProcessingColor = Shader.PropertyToID("_S");
        
        public static RenderTargetIdentifier _GAlbedo_RT = new RenderTargetIdentifier(_GAlbedo);
        public static RenderTargetIdentifier _GNormal_RT = new RenderTargetIdentifier(_GNormal);
        public static RenderTargetIdentifier _GDepth_RT = new RenderTargetIdentifier(_GDepth);
        public static RenderTargetIdentifier DefferedDepth = new RenderTargetIdentifier(_DefferedDepth);
        public static RenderTargetIdentifier _PostProcessing_RT = new RenderTargetIdentifier(_PostProcessingColor);


        public RenderTextureManager() { }

     

        public void DepthResolve(ScriptableRenderContext context, CommandBuffer cmd)
        {
            RenderTextureDescriptor desc = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.R16, 0);
            desc.sRGB = false;
            desc.dimension = TextureDimension.Tex2D;
            cmd.GetTemporaryRT(_DefferedDepth, desc, FilterMode.Point);
            cmd.Blit(_GDepth_RT, DefferedDepth);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }

        public void GenerateRT(CommandBuffer cmd, bool IsHDR)
        {
            RenderTextureDescriptor depthDesc = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.Depth, 24, 0);
            RenderTextureDescriptor colorDesc = new RenderTextureDescriptor(Screen.width, Screen.height, IsHDR ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32, 0);
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
            
        }

        public void SetupGBuffer(ScriptableRenderContext context,CommandBuffer cmd, bool IsHDR)
        {
            GenerateRT(cmd, IsHDR);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            RenderTargetIdentifier[] rts = new RenderTargetIdentifier[2];
            rts[0] = _GAlbedo_RT;
            rts[1] = _GNormal_RT;
            cmd.SetRenderTarget(rts, _GDepth_RT);
            cmd.ClearRenderTarget(true, true, Color.black);     
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }

        public void SetupLightingPassGbuffer(ScriptableRenderContext context, CommandBuffer cmd)
        {
            cmd.SetGlobalTexture(_GAlbedo, _GAlbedo_RT);
            cmd.SetGlobalTexture(_GNormal, _GNormal_RT);
            cmd.SetGlobalTexture(_DefferedDepth, DefferedDepth);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }

        public void SetupLightingPassRenderTarget(ScriptableRenderContext context, CommandBuffer cmd)
        {
            cmd.SetRenderTarget(_PostProcessing_RT, _GDepth_RT);
            cmd.ClearRenderTarget(false, true, Color.black);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }

        public void ReleaseRT(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(_GAlbedo);
            cmd.ReleaseTemporaryRT(_GNormal);
            cmd.ReleaseTemporaryRT(_GDepth);
            cmd.ReleaseTemporaryRT(_PostProcessingColor);
            cmd.ReleaseTemporaryRT(_DefferedDepth);
        }
    }
}