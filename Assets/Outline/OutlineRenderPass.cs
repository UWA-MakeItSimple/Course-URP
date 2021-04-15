namespace UnityEngine.Rendering.Universal
{

    public class OutlineRenderPass : ScriptableRenderPass
    {
        RenderTargetIdentifier source;
        RenderTargetIdentifier destination;

        public FilterMode m_Filtermode { get; set; }
        public OutlineRenderFeature.Settings m_Settings;

        int sourceId;
        int destinationId;
        int temporaryRTId = Shader.PropertyToID("_TempRT");

        string m_ProfilerTag;

        public OutlineRenderPass(string tag)
        {
            m_ProfilerTag = tag;
        }

        //渲染相机前调用，Renderer会调用此方法
        //用于配置渲染目标、清楚状态、创建临时渲染纹理目标
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor blitTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            blitTargetDescriptor.depthBufferBits = 0;

            var renderer = renderingData.cameraData.renderer;

            sourceId = -1;
            source = renderer.cameraColorTarget;

            destinationId = temporaryRTId;
            cmd.GetTemporaryRT(destinationId, blitTargetDescriptor, m_Filtermode);
            destination = new RenderTargetIdentifier(destinationId);

        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
            Blit(cmd, source, destination, m_Settings.blitMaterial, m_Settings.blitMaterialPassIndex);
            Blit(cmd, destination, source);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (destinationId != -1)
                cmd.ReleaseTemporaryRT(destinationId);

            if (source == destination && sourceId != -1)
                cmd.ReleaseTemporaryRT(sourceId);
        }
    }
}