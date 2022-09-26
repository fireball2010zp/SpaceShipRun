using UnityEngine;
using UnityEngine.Rendering;
public class SpaceRunPipelineRender : RenderPipeline
{
    CameraRenderer _cameraRenderer;
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    // вызывается каждый новый кадр, получает ScriptableRenderContext, который передаёт
    // в графический движок и массив всех камер в сцене в порядке, в котором они предоставлены
    {
        _cameraRenderer = new CameraRenderer();
        CamerasRender(context, cameras);
    }

    private void CamerasRender(ScriptableRenderContext context, Camera[] cameras)
    {
        foreach (var camera in cameras)
        {
            _cameraRenderer.Render(context, camera);
        }
    }


}
