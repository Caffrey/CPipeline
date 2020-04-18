using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CPipeline.Runtime {

    public enum CRenderPassEvent
    {
        BeforeRendering = 0,
        BeforeRenderingShadows = 50,
        AfterRenderingShadows = 100,
        BeforeRenderingPrepasses = 150,
        AfterRenderingPrePasses = 200,
        BeforeRenderingOpaques = 250,
        AfterRenderingOpaques = 300,
        BeforeRenderingSkybox = 350,
        AfterRenderingSkybox = 400,
        BeforeRenderingTransparents = 450,
        AfterRenderingTransparents = 500,
        BeforeRenderingPostProcessing = 550,
        AfterRenderingPostProcessing = 600,
        AfterRendering = 1000,
    }


    public abstract class CPipelineRenderPass
    {
        public CRenderPassEvent renderPassEvent { get; set; }

        public RenderTargetIdentifier colorAttachment
        {
            get => m_ColorAttachment;
        }

        public RenderTargetIdentifier depthAttachment
        {
            get => m_DepthAttachment;
        }

        public ClearFlag clearFlag
        {
            get => m_ClearFlag;
        }

        public Color clearColor
        {
            get => m_ClearColor;
        }


        protected string m_ProfilerTag;
        RenderTargetIdentifier m_ColorAttachment = BuiltinRenderTextureType.CameraTarget;
        RenderTargetIdentifier m_DepthAttachment = BuiltinRenderTextureType.CameraTarget;
        ClearFlag m_ClearFlag = ClearFlag.None;
        Color m_ClearColor = Color.black;
        internal bool overrideCameraTarget { get; set; }
        internal bool isBlitRenderPass { get; set; }

        public CPipelineRenderPass() 
        {
            renderPassEvent = CRenderPassEvent.AfterRenderingOpaques;
            m_ColorAttachment = BuiltinRenderTextureType.CameraTarget;
            m_DepthAttachment = BuiltinRenderTextureType.CameraTarget;
            m_ClearFlag = ClearFlag.None;
            m_ClearColor = Color.black;
        }


        public void ConfigureTarget(RenderTargetIdentifier colorAttachment, RenderTargetIdentifier depthAttachment)
        {
            overrideCameraTarget = true;
            m_ColorAttachment = colorAttachment;
            m_DepthAttachment = depthAttachment;
        }

        public void ConfigureTarget(RenderTargetIdentifier colorAttachment)
        {
            overrideCameraTarget = true;
            m_ColorAttachment = colorAttachment;
            m_DepthAttachment = BuiltinRenderTextureType.CameraTarget;
        }

        public void ConfigureClear(ClearFlag clearFlag, Color clearColor)
        {
            m_ClearFlag = clearFlag;
            m_ClearColor = clearColor;
        }

        public DrawingSettings CreateDrawSetting(ShaderTagId shaderTagId,ref RenderingData renderingData, SortingCriteria sortingCriteria)
        {
            Camera camera = renderingData.cameraData.camera;
            SortingSettings sortSet = new SortingSettings(camera) { criteria = sortingCriteria };
            DrawingSettings settings = new DrawingSettings(shaderTagId, sortSet)
            {
                perObjectData = renderingData.perObjectData,
                mainLightIndex = renderingData.lightData.mainLightIndex,
                enableDynamicBatching = renderingData.supportsDynamicBatching,
                enableInstancing = camera.cameraType == CameraType.Preview ? false : true,
                
            };
            return settings;
        }

        public DrawingSettings CreateDrawSetting(List<ShaderTagId> shaderTagIdList, ref RenderingData renderingData, SortingCriteria sortingCriteria)
        {
            if (shaderTagIdList == null || shaderTagIdList.Count == 0)
            {
                Debug.LogWarning("ShaderTagId list is invalid. DrawingSettings is created with default pipeline ShaderTagId");
                return CreateDrawSetting(new ShaderTagId("UniversalPipeline"), ref renderingData, sortingCriteria);
            }

            DrawingSettings settings = CreateDrawSetting(shaderTagIdList[0], ref renderingData, sortingCriteria);
            for (int i = 1; i < shaderTagIdList.Count; ++i)
                settings.SetShaderPassName(i, shaderTagIdList[i]);
            return settings;
        }

        public virtual void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {

        }

    }
}