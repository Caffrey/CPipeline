using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Rendering.Deffered
{
    public class ShadowPass
    {

        public ShadowPass()
        {

        }

        ShadowDrawingSettings m_SadowSetting;
        bool RenderShadow = false;


        public void SetupShadowSetting(ref RenderFrameContext context)
        {

            RenderShadow = false;
            for(int i = 0; i < context.CullingResult.visibleLights.Length; i++)
            {
                var visibleLight = context.CullingResult.visibleLights[i];
                if (visibleLight.lightType == LightType.Directional && visibleLight.light.shadows != LightShadows.None)
                {
                    m_SadowSetting = new ShadowDrawingSettings(context.CullingResult, i);
                    RenderShadow = true;
                }
            }          
        }

        public void DrawShadow(ScriptableRenderContext context, CommandBuffer cmd)
        {
            if (!RenderShadow)
                return;
            context.DrawShadows(ref m_SadowSetting);
        }

    }
}