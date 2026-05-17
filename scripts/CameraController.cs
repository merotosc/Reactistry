using Godot;

namespace ChemFactory.scripts;

public class CameraController : Camera2D
{
    private const float ZoomFactor = 1.2f;
    private const float MinZoom = 0.1f;
    private const float MaxZoom = 3f;
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
    }
}
