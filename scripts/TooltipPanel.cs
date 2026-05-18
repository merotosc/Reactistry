using ChemFactory.scripts.Buildings;
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
    private Vector2 lastTilePosition;
    private IBuilding currentBuilding;
    private float hoverTime;

    public void Init(World world)
    {
        this.world = world;
    }

    public override void _Ready()
    {
        contentContainer = GetNode<Control>("PanelContainer/ContentContainer");
        label = contentContainer.GetNode<Label>("Name");
        inputsContainer = contentContainer.GetNode<Container>("Items/Inputs");
        outputsContainer = contentContainer.GetNode<Container>("Items/Outputs");
        itemUi = GD.Load<PackedScene>("res://scenes/item_ui.tscn");

        HideTooltip();
    }

    public override void _Process(float delta)
    {
        var tilePosition = (GetViewport().CanvasTransform.AffineInverse() * GetViewport().GetMousePosition()).ToTilePosition();
        if (tilePosition != lastTilePosition)
        {
            lastTilePosition = tilePosition;

            if (!world.TryGetBuilding(tilePosition, out currentBuilding))
            {
                hoverTime = 0;
                HideTooltip();
            }
        }

        if (currentBuilding == null)
        {
            return;
        }

        if (hoverTime >= TooltipDelay)
        {
            ShowTooltipForBuilding();
        }
        else
        {
            hoverTime += delta;
        }
    }

    private void ShowTooltipForBuilding()
    {
        var info = currentBuilding.GetInfo();

        label.Text = currentBuilding.Type.ToString();

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
            ui.GetNode<Control>("ImageFrame/Image").Modulate = item.Molecule.ToString().ColorHash();
        }
        else
        {
            ui.GetNode<Label>("Name").Text = string.Empty;
            ui.GetNode<Control>("ImageFrame/Image").Modulate = new Color(0, 0, 0, 0);
        }

        container.AddChild(ui);
    }
}
