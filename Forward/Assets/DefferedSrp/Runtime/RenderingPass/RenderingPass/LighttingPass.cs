using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Rendering.Deffered
{
    public struct LightData
    {
        public LightType type;
        public float Radius;
        public float SpotLightAngel;
        public Color color;

    }
    public class LighttingPass
    {
        public static Mesh SpotLightShape;
        Material pointLightMaterial;
        Material directionalLightMaterial;
        Material spotLightMaterial;
        public LighttingPass()
        {
            directionalLightMaterial = new Material(Shader.Find("Hidden/LightPass"));
            pointLightMaterial = new Material(Shader.Find("Hidden/PointLightPass"));
            spotLightMaterial = new Material(Shader.Find("Hidden/SpotLightPass"));
        }

        public void Execute(ScriptableRenderContext context, CommandBuffer cmd, ref CullingResults cullResult)
        {
            cmd.BeginSample("Opaque Lighting");
            foreach (var light in cullResult.visibleLights)
            {
                Vector4 LD = light.light.transform.forward;

                if(light.lightType == LightType.Directional)
                {
                    LD = -light.light.transform.forward;
                    LD.w = 1;
                }
                else
                {
                    LD = light.light.transform.position;
                    LD.w = 0;
                }
                if(light.lightType == LightType.Point)
                {
                    Matrix4x4 trans = Matrix4x4.identity;
                    trans *= Matrix4x4.Translate(light.light.transform.position);
                    trans *= Matrix4x4.Scale(Vector3.one * light.light.range);                
                    LD.w = light.light.range;

                    pointLightMaterial.SetVector("_LightDireciton", LD);
                    pointLightMaterial.SetColor("_LightColor", light.finalColor);
                    cmd.DrawMesh(PostProcessUtil.SphereMesh, trans, pointLightMaterial,0,0);

                }
                else if(light.lightType == LightType.Spot)
                {
                    
                    LD.w = light.light.range;
                   
                    float outerRad = Mathf.Deg2Rad * 0.5f * light.spotAngle;
                    float outerCos = Mathf.Cos(outerRad);
                    float outerTan = Mathf.Tan(outerRad);
                    float innerCos = Mathf.Cos(Mathf.Atan(((46f / 64f) * outerTan)));
                    float angleRange = Mathf.Max(innerCos - outerCos, 0.001f);

                    Vector2 atten = new Vector2();
                    atten.x = 1f / angleRange;
                    atten.y = -outerCos * atten.x;

                    spotLightMaterial.SetVector("lightAttenuation", atten);
                    spotLightMaterial.SetColor("_LightColor", light.finalColor);
                    spotLightMaterial.SetVector("_LightDireciton", LD);
                    spotLightMaterial.SetVector("_SpotLightDirection", light.light.transform.localToWorldMatrix.GetColumn(2));

                    cmd.DrawMesh(PostProcessUtil.HemiSphereMeSH, light.localToWorldMatrix, spotLightMaterial, 0, 0);
                }
                else
                {
                    directionalLightMaterial.SetVector("_LightDireciton", LD);
                    directionalLightMaterial.SetVector("_LightColor", light.finalColor);
                    cmd.DrawMesh(PostProcessUtil.RectangleMesh,Matrix4x4.identity,directionalLightMaterial);
                }                
            }

            cmd.EndSample("Opaque Lighting");
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

        }

    }
}
