namespace UnityEngine.Rendering.Universal
{

    public class SceneBlur : ScriptableRendererFeature
    {
        [SerializeField]
        public class Settings
        {
            public RenderPassEvent m_RenderPassEvent;
        }

        private Material m_BlurMaterial;

        class SceneBlurPass : ScriptableRenderPass
        {
            private RenderTextureDescriptor m_OpaqueDesc;

            public SceneBlurPass()
            {
                base.profilingSampler = new ProfilingSampler(nameof(SceneBlurPass));
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                cameraTextureDescriptor
            }

            // This method is called before executing the render pass.
            // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
            // When empty this render pass will render to the active camera render target.
            // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
            // The render pipeline will ensure target setup and clearing happens in a performant manner.
            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
            {
            }

            // Here you can implement the rendering logic.
            // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
            // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
            // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                CommandBuffer cmd = CommandBufferPool.Get();
                //Profiler·¶Î§¹¹Ôìº¯Êý
                using (new ProfilingScope(cmd, profilingSampler)) ;
                {

                }
            }

            // Cleanup any allocated resources that were created during the execution of this render pass.
            public override void OnCameraCleanup(CommandBuffer cmd)
            {
            }
        }

        SceneBlurPass m_SceneBlurPass;

        /// <inheritdoc/>
        public override void Create()
        {
            m_SceneBlurPass = new SceneBlurPass();
            m_SceneBlurPass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
            m_BlurMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("UWA/Blur"));
            
        }

        // Here you can inject one or multiple render passes in the renderer.
        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(m_SceneBlurPass);
        }
    }
}


