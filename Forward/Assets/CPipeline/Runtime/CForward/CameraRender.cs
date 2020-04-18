using UnityEngine;
using UnityEngine.Rendering;
namespace CPipeline.Runtime
{
    public class CameraRender
    {
        #region Pipeline Setting

        static ShaderTagId[] unlitShaderTagId = {
            new ShaderTagId("Always"),
            new ShaderTagId("OpapeLit"),
            new ShaderTagId("TransparentLit"),
            new ShaderTagId("Transparent"),
            new ShaderTagId("Clip"),
            new ShaderTagId("VertexLM"),
            new ShaderTagId("SRPDefaultUnlits")
        };
        #endregion

        ScriptableRenderContext context;
        Camera camera;
        CullingResults cullingResults;

        COpapePass mOpapePass;
        CTransparentPass mTransparentPass;




        const string MainBufferName = "Render Camera";
        CommandBuffer cmd = new CommandBuffer()
        {
            name = MainBufferName
        };

        public void Render(ScriptableRenderContext context, Camera camera)
        {
            this.context = context;
            this.camera = camera;

            if (!Cull())
            {
                return;
            }

            Setup();
            DrawVisibleGeometry();
            Submit();
        }


        bool Cull()
        {
            if (camera.TryGetCullingParameters(out ScriptableCullingParameters p))
            {
                cullingResults = this.context.Cull(ref p);
                return true;
            }
            return false;
        }

        void Setup()
        {
            context.SetupCameraProperties(this.camera);

            cmd.BeginSample(MainBufferName);
            cmd.ClearRenderTarget(true, true, Color.clear);
            ExecuteBuffer();
        }

        void SetupRendering()
        {

        }

        void RenderQueue()
        {

        }

        void Submit()
        {
            cmd.EndSample(MainBufferName);
            context.Submit();
        }

        void ExecuteBuffer()
        {
            this.context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }

        void DrawVisibleGeometry()
        {
            //cull


            //draw opape



            //draw transparent



















            this.context.DrawSkybox(this.camera);
        }


    }
}