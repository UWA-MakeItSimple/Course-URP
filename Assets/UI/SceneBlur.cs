//namespace UnityEngine.Rendering.Universal
//{
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SceneBlur : ScriptableRendererFeature
{
    [Serializable]
    public class Settings
    {
        public RenderPassEvent m_RenderPassEvent;
        public Vector2 m_BlurAmount;
    }

    public Settings settings;
    private Material m_BlurMaterial;
    private Vector2 currentBlurAmount;

    class SceneBlurPass : ScriptableRenderPass
    {
        private RenderTextureDescriptor m_OpaqueDesc;
        private RenderTargetIdentifier m_CamerColorTexture;

        private Material m_BlurMaterial;
        private Vector2 m_BlurAmount;

        public SceneBlurPass(Material blurMaterial, Vector2 blurAmount)
        {
            base.profilingSampler = new ProfilingSampler(nameof(SceneBlurPass));
            m_BlurMaterial = blurMaterial;
            m_BlurAmount = blurAmount;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cameraTextureDescriptor.msaaSamples = 1;
            m_OpaqueDesc = cameraTextureDescriptor;
        }

        public void SetUp(RenderTargetIdentifier destination)
        {
            m_CamerColorTexture = destination;
        }

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }

        public void UpdateBlurAmount(Vector2 newBlurAmount)
        {
            m_BlurAmount = newBlurAmount;
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();
            //Profiler范围构造函数
            using (new ProfilingScope(cmd, profilingSampler)) ;
            {
                m_OpaqueDesc.width /= 4;
                m_OpaqueDesc.height /= 4;
                int blurredID1 = Shader.PropertyToID("_BlurRT1");
                int blurredID2 = Shader.PropertyToID("_BlurRT2");
                cmd.GetTemporaryRT(blurredID1, m_OpaqueDesc, FilterMode.Bilinear);
                cmd.GetTemporaryRT(blurredID2, m_OpaqueDesc, FilterMode.Bilinear);
                cmd.Blit(m_CamerColorTexture, blurredID1);
                cmd.SetGlobalVector("offsets", new Vector4(m_BlurAmount.x / Screen.width, 0, 0, 0));
                cmd.Blit(blurredID1, blurredID2, m_BlurMaterial);
                cmd.SetGlobalVector("offsets", new Vector4(0, m_BlurAmount.y / Screen.height, 0, 0));
                cmd.Blit(blurredID2, blurredID1, m_BlurMaterial);
                cmd.SetGlobalVector("offsets", new Vector4(m_BlurAmount.x * 2 / Screen.width, 0, 0, 0));
                cmd.Blit(blurredID1, blurredID2, m_BlurMaterial);
                cmd.SetGlobalVector("offsets", new Vector4(0, m_BlurAmount.y * 2 / Screen.height, 0, 0));
                cmd.Blit(blurredID2, blurredID1, m_BlurMaterial);
                cmd.Blit(blurredID1, m_CamerColorTexture);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
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
        m_SceneBlurPass = new SceneBlurPass(m_BlurMaterial, currentBlurAmount);
        m_SceneBlurPass.renderPassEvent = settings.m_RenderPassEvent;
        m_BlurMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("UWA/Blur"));
        currentBlurAmount = settings.m_BlurAmount;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        //编辑器下Scene窗口的相机
        if (!renderingData.cameraData.isSceneViewCamera)
        {
            m_SceneBlurPass.SetUp(renderer.cameraColorTarget);
            renderer.EnqueuePass(m_SceneBlurPass);
        }
    }

    void Update()
    {
        if (m_SceneBlurPass != null)
        {
            if (currentBlurAmount != settings.m_BlurAmount)
            {
                currentBlurAmount = settings.m_BlurAmount;
                m_SceneBlurPass.UpdateBlurAmount(currentBlurAmount);
            }
        }
    }
}
//}


