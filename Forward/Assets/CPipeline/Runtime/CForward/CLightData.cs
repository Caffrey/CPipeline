using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace CPipeline.Runtime
{
    public class CLightData 
    {
        public static int MAX_REALTIME_LIGHT_COUNT = 64;
        Vector4[] lightColors = new Vector4[MAX_REALTIME_LIGHT_COUNT];
        Vector4[] lightDirection = new Vector4[MAX_REALTIME_LIGHT_COUNT];
        Vector4[] lightAttenuation = new Vector4[MAX_REALTIME_LIGHT_COUNT];

        CommandBuffer lightCmd;

        public CLightData()
        {
            lightCmd = new CommandBuffer();
        }


        public void setupLight(ScriptableRenderContext context, ref CullingResults cullResults)
        {
            NativeArray<VisibleLight> lights = cullResults.visibleLights;
            int lightCount = lights.Length;
            
            for(int i = 0; i < lightCount; i++)
            {
                VisibleLight light = lights[i];
                Vector4 attenuation = Vector4.zero;

                if(light.lightType == LightType.Directional)
                {
                    lightColors[i] = light.finalColor;                
                    lightDirection[i] = -light.localToWorldMatrix.GetColumn(2);
                    lightColors[i].w = 0;
                    attenuation.w = 1;
                }
                else if(light.lightType == LightType.Point || light.lightType == LightType.Spot)
                {
                    lightColors[i] = light.finalColor;
                  
                    lightDirection[i] = light.localToWorldMatrix.GetColumn(3);
                    lightColors[i].w = 1;
                    //Point Light Fade Out Reference: https://catlikecoding.com/unity/tutorials/scriptable-render-pipeline/lights/
                    attenuation.x = 1f / Mathf.Max(0.000001f, light.range * light.range);



                }


                lightAttenuation[i] = attenuation;

            }

            lightCmd.SetGlobalInt(ShaderProperty.VISIBLE_LIGHT_COUNT, lightCount);
            lightCmd.SetGlobalVectorArray(ShaderProperty.VISIBLE_LIGHT_COLOR, lightColors);
            lightCmd.SetGlobalVectorArray(ShaderProperty.VISIBLE_LIGHT_DIRECTION, lightDirection);
            lightCmd.SetGlobalVectorArray(ShaderProperty.VISIBLE_LIGHT_ATTENUATION, lightAttenuation);
            context.ExecuteCommandBuffer(lightCmd);
            lightCmd.Clear();
        }


    }
}
