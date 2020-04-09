using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CPipeline.Runtime
{
    //setup light shadow property ,in lightsetting
    //setup shadow pass rendering
    //add shader caster
    //add shader use shadow
    public class CShadowPass
    {
        const string bufferName = "Shadow";
        public const int MAX_SHADOW_DIRECTIONAL_LIGHT_COUNT = 1;
        
        public void Render(ScriptableRenderContext context, Camera camera, ref CullingResults cullResult)
        {

        }

        public void ExecuteBuffer()
        {

        }


    }
}
