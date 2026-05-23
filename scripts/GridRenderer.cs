using Godot;

namespace Reactistry.scripts;

public class GridRenderer : Node2D
{
    private const int CellSize = Constants.PixelsPerTile;
    private const float MaxZoomGrid = 2f;
    private Camera2D camera;
    private static readonly Color gridColor = Color.FromHsv(0, 1, 0, 0.05f);
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
        if (camera.Zoom.x > MaxZoomGrid)
        {
            return;
        }

        var viewport = GetViewportRect();

        var topLeft = camera.GlobalPosition - viewport.Size * camera.Zoom / 2f;
        var bottomRight = topLeft + viewport.Size * camera.Zoom;

        var startX = Mathf.FloorToInt(topLeft.x / CellSize);
        var endX = Mathf.CeilToInt(bottomRight.x / CellSize);

        var startY = Mathf.FloorToInt(topLeft.y / CellSize);
        var endY = Mathf.CeilToInt(bottomRight.y / CellSize);

        for (var x = startX; x <= endX; x++)
        {
            float px = x * CellSize;

            DrawLine(
                new Vector2(px, topLeft.y),
                new Vector2(px, bottomRight.y),
                gridColor);
        }

        for (var y = startY; y <= endY; y++)
        {
            float py = y * CellSize;

            DrawLine(
                new Vector2(topLeft.x, py),
                new Vector2(bottomRight.x, py),
                gridColor);
        }
    }
}
