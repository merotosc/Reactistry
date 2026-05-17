using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts;

public class TooltipPanel : Control
{
    private const float TooltipDelay = 0.35f;
    private World world;
    private Control contentContainer;
    private Label label;
    private Control inputsContainer;
    private Control outputsContainer;
    private PackedScene itemUi;
    private Vector2 hoveredTile;
    private float hoverTime;
    private bool tooltipShown;

    public void Init(World world)
    {
        this.world = world;
    }

    public override void _Ready()
    {
        contentContainer = GetNode<Control>("Panel/MarginContainer/ContentContainer");
        label = contentContainer.GetNode<Label>("Name");
        inputsContainer = contentContainer.GetNode<Container>("Items/Inputs");
        outputsContainer = contentContainer.GetNode<Container>("Items/Outputs");
        itemUi = GD.Load<PackedScene>("res://scenes/item_ui.tscn");

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
            tooltipShown = false;
            return;
        }

        //if (tooltipShown) return;

        hoverTime += delta;
        if (hoverTime >= TooltipDelay) // TODO: ignore delay if tooltip is already displayed (when switching to near building)
        {
            ShowTooltipForTile(tilePosition);
            tooltipShown = true;
        }
    }

    private void ShowTooltipForTile(Vector2 tilePosition)
    {
        if (!world.TryGetBuilding(tilePosition, out var building))
        {
            return;
        }

        var info = building.GetInfo();

        label.Text = building.Type.ToString();

        foreach (Node child in inputsContainer.GetChildren())
        {
            child.QueueFree();
        }

        foreach (Node child in outputsContainer.GetChildren())
        {
            child.QueueFree();
        }

        foreach (var item in info.InputItems)
        {
            AddItemElement(inputsContainer, item);
        }

        foreach (var item in info.OutputItems)
        {
            AddItemElement(outputsContainer, item);
        }

        ShowTooltip();
    }

    private void ShowTooltip()
    {
        RectSize = GetMinimumSize();
        var offset = new Vector2(20, 0);
        var screenPosition = GetViewport().GetMousePosition();

        RectPosition = new Vector2(
            screenPosition.x + offset.x,
            screenPosition.y - RectSize.y * 0.5f
        );

        Show();
    }

    private void HideTooltip()
    {
        Hide();
    }

    private void AddItemElement(Control container, Item item)
    {
        var ui = itemUi.Instance();
        if (item != null)
        {
            ui.GetNode<Label>("Name").Text = item.Molecule.ToString();
            ui.GetNode<Control>("Image").Modulate = item.Molecule.ToString().ColorHash();
        }
        else
        {
            ui.GetNode<Label>("Name").Text = string.Empty;
            ui.GetNode<Control>("Image").Modulate = new Color(0, 0, 0, 0);
        }

        container.AddChild(ui);
    }
}
