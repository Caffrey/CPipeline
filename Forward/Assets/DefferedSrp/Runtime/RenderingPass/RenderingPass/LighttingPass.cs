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
        Material lightPassMaterial;
        public static Mesh PointLightShape;
        public static Mesh SpotLightShape;
        public Material pointLightMaterial;

        public LighttingPass()
        {
            lightPassMaterial = new Material(Shader.Find("Hidden/LightPass"));
            PointLightShape = GameObject.CreatePrimitive(PrimitiveType.Sphere).GetComponent<MeshFilter>().sharedMesh;
            pointLightMaterial = new Material(Shader.Find("Hidden/PointLightPass"));
        }

        public void Execute(ScriptableRenderContext context, CommandBuffer cmd, ref CullingResults cullResult)
        {
            cmd.Clear();
            cmd.BeginSample("LightingPass");

            foreach(var light in cullResult.visibleLights)
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

                cmd.SetGlobalVector("_LightDireciton", LD);
                cmd.SetGlobalVector("_LightColor", light.finalColor);

                if(light.lightType == LightType.Point)
                {
                    Matrix4x4 trans = Matrix4x4.identity;
                    trans *= Matrix4x4.Translate(light.light.transform.position);
                    trans *= Matrix4x4.Scale(Vector3.one * light.light.range);
                    pointLightMaterial.SetColor("_PointLightColor",light.light.color);
                    pointLightMaterial.SetFloat("_PointLightRadius", light.light.range);
                    cmd.DrawMesh(PointLightShape, trans, pointLightMaterial);
                }
                else
                {
                    cmd.DrawMesh(PostProcessUtil.RectangleMesh,Matrix4x4.identity,lightPassMaterial);
                }

                context.ExecuteCommandBuffer(cmd);
                
                cmd.Clear();
            }
            cmd.EndSample("LightingPass");

        }

    }
}
