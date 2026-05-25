using Godot;

namespace Reactistry.scripts;

public class CameraController : Camera2D
{
    private const float ZoomFactor = 1.2f;
    private const float MinZoom = 0.1f;
    private const float MaxZoom = 3f;
    private const int PanSpeed = 800;
    private bool panning;

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseButton mb)
        {
            if (mb.ButtonIndex == (int)ButtonList.Middle)
            {
                panning = mb.Pressed;
            }

            if (mb.Pressed)
            {
                if (mb.ButtonIndex == (int)ButtonList.WheelUp)
                {
                    ZoomCamera(zoomIn: true);
                }
                else if (mb.ButtonIndex == (int)ButtonList.WheelDown)
                {
                    ZoomCamera(zoomIn: false);
                }
            }
        }

        if (e is InputEventMouseMotion motion && panning)
        {
            Position -= motion.Relative * Zoom;
            ClampCameraToBounds();
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        var movement = Vector2.Zero;

        if (Input.IsActionPressed("camera_up", true))
        {
            movement.y -= 1;
        }

        if (Input.IsActionPressed("camera_left", true))
        {
            movement.x -= 1;
        }

        if (Input.IsActionPressed("camera_down", true))
        {
            movement.y += 1;
        }

        if (Input.IsActionPressed("camera_right", true))
        {
            movement.x += 1;
        }

        if (movement != Vector2.Zero)
        {
            movement = movement.Normalized();
            Position += movement * Zoom * PanSpeed * delta;
            ClampCameraToBounds();
        }
    }

    private void ZoomCamera(bool zoomIn)
    {
        var before = GetGlobalMousePosition();

        var factor = zoomIn
            ? 1 / ZoomFactor
            : ZoomFactor;

        var newZoom = Zoom * factor;
        newZoom.x = Mathf.Clamp(newZoom.x, MinZoom, MaxZoom);
        newZoom.y = Mathf.Clamp(newZoom.y, MinZoom, MaxZoom);
        Zoom = newZoom;

        var after = GetGlobalMousePosition();
        Position += before - after;
        ClampCameraToBounds();
    }

    private void ClampCameraToBounds()
    {
        var halfTile = Constants.Map.TileSize / 2;
        var xBoundary = (Constants.Map.WorldSize.x * Constants.Map.ChunkSize.x * Constants.Map.TileSize) / 2;
        var yBoundary = (Constants.Map.WorldSize.y * Constants.Map.ChunkSize.y * Constants.Map.TileSize) / 2;
        var maxTopLeft = new Vector2(halfTile - xBoundary, halfTile - yBoundary);
        var maxBottomRight = new Vector2(halfTile + xBoundary, halfTile + yBoundary);

        var viewport = GetViewportRect().Size;
        var halfSize = viewport * Zoom * 0.5f;

        var min = maxTopLeft + halfSize;
        var max = maxBottomRight - halfSize;

        var position = Position;

        if (min.x > max.x)
        {
            position.x = (maxTopLeft.x + maxBottomRight.x) * 0.5f;
        }
        else
        {
            position.x = Mathf.Clamp(position.x, min.x, max.x);
        }

        if (min.y > max.y)
        {
            position.y = (maxTopLeft.y + maxBottomRight.y) * 0.5f;
        }
        else
        {
            position.y = Mathf.Clamp(position.y, min.y, max.y);
        }

        Position = position;
    }
}
