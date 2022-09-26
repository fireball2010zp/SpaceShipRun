using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;

partial class CameraRenderer // ��� ���������� ������ ��������� ������
{
    // Start is called before the first frame update
    private ScriptableRenderContext _context;
    private Camera _camera;
    private CommandBuffer _commandBuffer;
    private const string bufferName = "Camera Render";
    private CullingResults _cullingResult;
    private static readonly List<ShaderTagId> drawingShaderTagIds = new List<ShaderTagId> { new ShaderTagId("SRPDefaultUnlit"), };
    // ������������ Unlit �������, ��� ������ ���� ���������� ��� ���� ������� �������� � �����

    public void Render(ScriptableRenderContext context, Camera camera)
    {
        _camera = camera;
        _context = context;
        UIGO();
        if (!Cull(out var parameters))
        {
            return;
        }
        Settings(parameters);
        DrawVisible();
        DrawUnsupportedShaders();
        DrawGizmos();
        Submit();
    }

    private void Submit()
    {
        _commandBuffer.EndSample(bufferName);
        ExecuteCommandBuffer(); // ����������� ������ ������ � ��������
        _context.Submit();
    }

    private void DrawVisible()
    {
        var drawingSettings =
            CreateDrawingSettings(drawingShaderTagIds, SortingCriteria.CommonOpaque, out var sortingSettings);
        // ��������� �������� � ������� ��������: �� ��������� � ����� �������
        var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

        _context.DrawRenderers(_cullingResult, ref drawingSettings, ref filteringSettings);

        _context.DrawSkybox(_camera); // ����������� skybox

        sortingSettings.criteria = SortingCriteria.CommonTransparent;
        drawingSettings.sortingSettings = sortingSettings;
        filteringSettings.renderQueueRange = RenderQueueRange.transparent;

        _context.DrawRenderers(_cullingResult, ref drawingSettings, ref filteringSettings);
    }

    private void Settings(ScriptableCullingParameters parameters)
    {
        _commandBuffer = new CommandBuffer
        { name = _camera.name };
        _cullingResult = _context.Cull(ref parameters); // ���� Cull ���������� true, �� ��������� ���������� � �������� ��� ��������� ���������� ��������� CullingResults
        _context.SetupCameraProperties(_camera); // �������� unity_MatrixVP � ��������� ������ �������� ������ ���������� � ��������
        _commandBuffer.ClearRenderTarget(true, true, Color.clear); // ������� �������� ������� �� ������ ������
        _commandBuffer.BeginSample(bufferName);
        ExecuteCommandBuffer();
    }

    private DrawingSettings CreateDrawingSettings(List<ShaderTagId> shaderTags, SortingCriteria sortingCriteria, out SortingSettings sortingSettings)
    // ����� ���������� �������� ����������, ��������� ������ ����������� ���
    // ��������� �������� ShaderTagId � �������� ���������� SortingCriteria,
    // ������� ���������� ������� ��������� �������� � �����
    {
        sortingSettings = new SortingSettings(_camera)
        {
            criteria = sortingCriteria,
        };

        var drawingSettings = new DrawingSettings(shaderTags[0], sortingSettings);
        for (var i = 1; i < shaderTags.Count; i++)
        {
            drawingSettings.SetShaderPassName(i, shaderTags[i]);
        }

        return drawingSettings;
    }

    private bool Cull(out ScriptableCullingParameters parameters)
    // ����������� ������� ������ ��������, ����� ��������� ������ ��
    {
        return _camera.TryGetCullingParameters(out parameters);
    }

    private void ExecuteCommandBuffer()
    {
        _context.ExecuteCommandBuffer(_commandBuffer);
        _commandBuffer.Clear(); // ����� ������� ������
    }

    private void UIGO()
    {
        if (_camera.cameraType == CameraType.SceneView)
        {
            ScriptableRenderContext.EmitWorldGeometryForSceneView(_camera);
        }
    }
}
