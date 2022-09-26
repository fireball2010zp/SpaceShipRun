using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Rendering/SpaceRunPipelineRenderAsset")] // �������� ����� �������� ����� ���� Create
public class SpaceRunPipelineRenderAsset : RenderPipelineAsset
{
    protected override RenderPipeline CreatePipeline()
    {
        return new SpaceRunPipelineRender(); // ���������� ����������� ���������         ���������
    }
}
