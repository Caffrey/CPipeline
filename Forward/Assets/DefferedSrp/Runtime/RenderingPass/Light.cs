using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Rendering.Deffered
{

   


    public class Light 
    {
        public static Mesh PointLightShape;
        public static Mesh SpotLightShape;

        public void InitLight()
        {
            PointLightShape = GameObject.CreatePrimitive(PrimitiveType.Sphere).GetComponent<MeshFilter>().mesh;

        }

        public void LightingSetup(ScriptableRenderContext context, CommandBuffer cmd, ref CullingResults cullResult)
        {

        }
    }
}