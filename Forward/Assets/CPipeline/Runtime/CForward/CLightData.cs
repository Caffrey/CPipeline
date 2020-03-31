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
        Vector4[] lightSportDirection = new Vector4[MAX_REALTIME_LIGHT_COUNT];

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
                    //Light Fade Out Reference: https://catlikecoding.com/unity/tutorials/scriptable-render-pipeline/lights/
                    attenuation.x = 1f / Mathf.Max(0.000001f, light.range * light.range);
                    attenuation.w = 1;
                    if (light.lightType == LightType.Spot)
                    {
                        Vector4 v = light.localToWorldMatrix.GetColumn(2);
                        v *= -1;
                        v.w = 1;
                        lightSportDirection[i] = v;
                        //求出inner 和outer角度的cos
                        float outerRad = Mathf.Deg2Rad * 0.5f * light.spotAngle;
                        float cosOuter = Mathf.Cos(outerRad);
                        float outerTan = Mathf.Tan(outerRad);
                        float cosInner = Mathf.Cos(Mathf.Atan((46f / 64f) * outerTan));

                        float angleRange = Mathf.Max(cosInner - cosOuter,0.001f);
                        attenuation.z = 1f / angleRange;
                        attenuation.w = -cosOuter * attenuation.z;


                    }


                }


                lightAttenuation[i] = attenuation;

            }

            lightCmd.SetGlobalInt(ShaderProperty.VISIBLE_LIGHT_COUNT, lightCount);
            lightCmd.SetGlobalVectorArray(ShaderProperty.VISIBLE_LIGHT_COLOR, lightColors);
            lightCmd.SetGlobalVectorArray(ShaderProperty.VISIBLE_LIGHT_DIRECTION, lightDirection);
            lightCmd.SetGlobalVectorArray(ShaderProperty.VISIBLE_LIGHT_ATTENUATION, lightAttenuation);
            lightCmd.SetGlobalVectorArray(ShaderProperty.VISIBLE_LIGHT_SPOT_DIRECTION, lightSportDirection);
            
            context.ExecuteCommandBuffer(lightCmd);
            lightCmd.Clear();
        }


    }
}
