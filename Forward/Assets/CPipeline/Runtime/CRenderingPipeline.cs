using UnityEngine;
using UnityEngine.Rendering;

namespace CPipeline.Runtime
{

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


