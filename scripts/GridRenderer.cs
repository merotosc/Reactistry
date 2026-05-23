using Godot;

namespace Reactistry.scripts;

public class GridRenderer : Node2D
{
    private const int TileSize = Constants.Map.TileSize;
    private static readonly int ChunkPixels = (int)Constants.Map.ChunkSize.x * TileSize;
    private const float MaxZoomGrid = 2f;
    private Camera2D camera;
    private static readonly Color tilesGridColor = Color.FromHsv(0, 1, 0, 0.05f);
    private static readonly Color chunksGridColor = Color.FromHsv(0, 1, 0, 0.1f);
    private static readonly Color backgroundColor = Color.FromHsv(0, 0, 1, 1);

    public override void _Ready()
    {
        VisualServer.SetDefaultClearColor(backgroundColor);
        camera = GetNode<Camera2D>("/root/Game/Camera2D");
    }

    public override void _Process(float delta)
    {
        Update();
    }

    public override void _Draw()
    {
        var viewport = GetViewportRect();

        var topLeft = camera.GlobalPosition - viewport.Size * camera.Zoom / 2f;
        var bottomRight = topLeft + viewport.Size * camera.Zoom;

        if (camera.Zoom.x <= MaxZoomGrid)
        {
            DrawTilesGrid(topLeft, bottomRight);
        }

        DrawChunksGrid(topLeft, bottomRight);
    }

    private void DrawTilesGrid(Vector2 topLeft, Vector2 bottomRight)
    {
        var startX = Mathf.FloorToInt(topLeft.x / TileSize);
        var endX = Mathf.CeilToInt(bottomRight.x / TileSize);

        var startY = Mathf.FloorToInt(topLeft.y / TileSize);
        var endY = Mathf.CeilToInt(bottomRight.y / TileSize);

        DrawLines(startX, endX, topLeft.y, bottomRight.y, TileSize, 0, vertical: true, tilesGridColor);
        DrawLines(startY, endY, topLeft.x, bottomRight.x, TileSize, 0, vertical: false, tilesGridColor);
    }

    private void DrawChunksGrid(Vector2 topLeft, Vector2 bottomRight)
    {
        var halfChunk = (ChunkPixels - TileSize) / 2;

        var chunkStartX = Mathf.FloorToInt((topLeft.x + halfChunk) / ChunkPixels);
        var chunkEndX = Mathf.CeilToInt((bottomRight.x + halfChunk) / ChunkPixels);

        var chunkStartY = Mathf.FloorToInt((topLeft.y + halfChunk) / ChunkPixels);
        var chunkEndY = Mathf.CeilToInt((bottomRight.y + halfChunk) / ChunkPixels);

        DrawLines(chunkStartX, chunkEndX, topLeft.y, bottomRight.y, ChunkPixels, -halfChunk, vertical: true, chunksGridColor, 2f * camera.Zoom.x);
        DrawLines(chunkStartY, chunkEndY, topLeft.x, bottomRight.x, ChunkPixels, -halfChunk, vertical: false, chunksGridColor, 2f * camera.Zoom.x);
    }

    private void DrawLines(int from, int to, float min, float max, int size, int offset, bool vertical, Color color, float width = 1f)
    {
        for (var i = from; i <= to; i++)
        {
            var p = i * size + offset;
            var start = vertical ? new Vector2(p, min) : new Vector2(min, p);
            var end = vertical ? new Vector2(p, max) : new Vector2(max, p);
            DrawLine(start, end, color, width);
        }
    }
}
