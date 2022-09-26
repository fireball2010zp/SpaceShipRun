using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Rendering/SpaceRunPipelineRenderAsset")] // создание файла настроек через меню Create
public class SpaceRunPipelineRenderAsset : RenderPipelineAsset
{
    protected override RenderPipeline CreatePipeline()
    {
        return new SpaceRunPipelineRender(); // возвращает действующий экземпл€р         конвейера
    }
}
