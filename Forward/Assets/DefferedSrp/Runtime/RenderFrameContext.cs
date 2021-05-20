using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Rendering.Deffered
{
    public class RenderFrameContext
    {
        public CullingResults CullingResult;
        public ScriptableRenderContext SRPContext;
        public Camera camera;
        public CommandBuffer CommandBuffer;

        public RenderFrameContext(ref ScriptableRenderContext context)
        {
            this.SRPContext = context;
            CommandBuffer = new CommandBuffer();
            CommandBuffer.name = "Deffered";
        }

        public void ClearCommandBuffer()
        {
            CommandBuffer.Clear();
        }
        public void ExecuteCommandBuffer()
        {
            SRPContext.ExecuteCommandBuffer(CommandBuffer);
        }

        void BeginRecordCommand() { }

        void EndRecordCommand() { }
    }
}