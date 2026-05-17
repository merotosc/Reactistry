using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts;

public class TooltipPanel : Control
{
    private const float TooltipDelay = 0.5f;
    private Camera2D camera;
    private World world;
    private Label label;
    private Vector2 hoveredTile;
    private float hoverTime;

    public void Init(World world)
    {
        this.world = world;
    }

    public override void _Ready()
    {
        camera = GetNode<Camera2D>("/root/Game/Camera2D");
        label = GetNode<Label>("Panel/MarginContainer/VBoxContainer/Label");
        HideTooltip();
    }

    public override void _Process(float delta)
    {
        var tilePosition = (GetViewport().CanvasTransform.AffineInverse() * GetViewport().GetMousePosition()).ToTilePosition();
        if (tilePosition != hoveredTile)
        {
            hoveredTile = tilePosition;
            hoverTime = 0;
            HideTooltip();
            return;
        }

        hoverTime += delta;
        if (hoverTime >= TooltipDelay)
        {
            ShowTooltipForTile(tilePosition);
        }
    }

    private void ShowTooltipForTile(Vector2 tilePosition)
    {
        if (!world.TryGetBuilding(tilePosition, out var building))
        {
            return;
        }

        var text = building.GetInfo();
        ShowTooltip(text);
    }

    public void ShowTooltip(string text)
    {
        label.Text = text;

        RectSize = GetMinimumSize();
        var offset = new Vector2(20, 0);
        var screenPosition = GetViewport().GetMousePosition();

        RectPosition = new Vector2(
            screenPosition.x + offset.x,
            screenPosition.y - RectSize.y * 0.5f
        );

        Show();
    }

    public void HideTooltip()
    {
        Hide();
    }

}
