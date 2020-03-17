using UnityEngine;
using UnityEngine.Rendering;

namespace CPipeline.Runtime
{
    //TODO 
    /*
     * 0.RENDERING UNLIT
     * 0.RENDERING LIT
     * 
     * 1.RENDERING OP
     * 2.RENDERING TRANSPARENT
     * 3.BASIC RENDERING SHADER LIB
     * 4.PBR RENDERING LIB
     * 
     */
    public class CRenderingPipeline : RenderPipeline
    {

        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            BeginFrameRendering(context,cameras);

            BeginCameraRendering(context, cameras[0]);

            context.SetupCameraProperties(cameras[0]);
            context.DrawSkybox(cameras[0]);
            context.Submit();

            EndCameraRendering(context, cameras[0]);


            EndFrameRendering(context, cameras);
        }

    }
}


