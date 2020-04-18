using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace CPipeline.Runtime
{
    public enum AntialiasingMode
    {
        None,
        FastApproximateAntialiasing,
        SubpixelMorphologicalAntiAliasing,
        //TemporalAntialiasing
    }

    // Only used for SMAA right now
    public enum AntialiasingQuality
    {
        Low,
        Medium,
        High
    }


    public struct RenderingData
    {
        public ScriptableRenderContext context;
        public CPipelineAsset asset; 
        public CommandBuffer cmd;
        public CameraData cameraData;
        public CullingResults cullResult;
        public PerObjectData perObjectData;
        public LightData lightData;
        public ShadowData shadowData;

        public bool supportsDynamicBatching;

    }


    public struct LightData
    {
        public int mainLightIndex;
        public int additionalLightsCount;
        public int maxPerObjectAdditionalLightsCount;
        public NativeArray<VisibleLight> visibleLights;
        public bool shadeAdditionalLightsPerVertex;
        public bool supportsMixedLighting;
    }

    public struct CameraData
    {
        public Camera camera;
        public RenderTextureDescriptor cameraTargetDescriptor;
        public float renderScale;
        public bool isSceneViewCamera;
        public bool isDefaultViewport;
        public bool isHdrEnabled;
        public bool requiresDepthTexture;
        public bool requiresOpaqueTexture;

        public SortingCriteria defaultOpaqueSortFlags;

        public bool isStereoEnabled;

        public float maxShadowDistance;
        public bool postProcessEnabled;

       // public IEnumerator<Action<RenderTargetIdentifier, CommandBuffer>> captureActions;

        public LayerMask volumeLayerMask;
        public Transform volumeTrigger;

        public bool isStopNaNEnabled;
        public bool isDitheringEnabled;
        public AntialiasingMode antialiasing;
        public AntialiasingQuality antialiasingQuality;
    }

    public struct ShadowData
    {
        public bool supportsMainLightShadows;
        public bool requiresScreenSpaceShadowResolve;
        public int mainLightShadowmapWidth;
        public int mainLightShadowmapHeight;
        public int mainLightShadowCascadesCount;
        public Vector3 mainLightShadowCascadesSplit;
        public bool supportsAdditionalLightShadows;
        public int additionalLightsShadowmapWidth;
        public int additionalLightsShadowmapHeight;
        public bool supportsSoftShadows;
        public int shadowmapDepthBufferBits;
        public List<Vector4> bias;
    }



}
