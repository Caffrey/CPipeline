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

                if(light.lightType == LightType.Directional)
                {
                    lightColors[i] = light.finalColor;
                    lightColors[i].w = 0;
                    lightDirection[i] = -light.localToWorldMatrix.GetColumn(2);
                }
                else if(light.lightType == LightType.Point || light.lightType == LightType.Spot)
                {
                    lightColors[i] = light.finalColor;
                    lightColors[i].w = 1;
                    lightDirection[i] = light.localToWorldMatrix.GetColumn(3);
                }

            }

            lightCmd.SetGlobalInt(ShaderProperty.VISIBLE_LIGHT_COUNT, lightCount);
            lightCmd.SetGlobalVectorArray(ShaderProperty.VISIBLE_LIGHT_COLOR, lightColors);
            lightCmd.SetGlobalVectorArray(ShaderProperty.VISIBLE_LIGHT_DIRECTION, lightDirection);
            context.ExecuteCommandBuffer(lightCmd);
            lightCmd.Clear();
        }


    }
}
