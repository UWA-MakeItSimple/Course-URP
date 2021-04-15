namespace UnityEngine.Rendering.Universal
{

    public class OutlineRenderFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class Settings
        {
            public RenderPassEvent m_RenderPassEvent = RenderPassEvent.AfterRenderingOpaques;
            public Material blitMaterial = null;
            public int blitMaterialPassIndex = -1;
        }

        public Settings m_Settings = new Settings();
        OutlineRenderPass m_OutlineRenderPass;

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_OutlineRenderPass.renderPassEvent = m_Settings.m_RenderPassEvent;
            m_OutlineRenderPass.m_Settings = m_Settings;
            renderer.EnqueuePass(m_OutlineRenderPass);
        }

        public override void Create()
        {
            m_OutlineRenderPass = new OutlineRenderPass(name);
        }
    }
}