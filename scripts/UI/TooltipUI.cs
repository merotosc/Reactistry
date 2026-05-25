using Reactistry.scripts.Buildings;
using Reactistry.scripts.Models;
using Reactistry.scripts.Utilities;
using Godot;

namespace Reactistry.scripts.UI;

public class TooltipUI : Control
{
    private const float TooltipDelay = 0.35f;
    private World world;
    private Control contentContainer;
    private Label label;
    private Control inputsContainer;
    private Control outputsContainer;
    private PackedScene itemUi;
    private Vector2 lastTilePosition;
    private Molecule currentResource;
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

            if (!world.TryGetBuilding(tilePosition, out currentBuilding) && !world.TryGetResource(tilePosition, out currentResource))
            {
                hoverTime = 0;
                HideTooltip();
            }
        }

        if (currentBuilding == null && currentResource == null)
        {
            return;
        }

        if (hoverTime >= TooltipDelay)
        {
            if (currentBuilding != null)
            {
                ShowTooltipForBuilding();
            }
            else if (currentResource != null)
            {
                ShowTooltipForResource();
            }
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

        ClearItems();

        foreach (var item in info.InputItems)
        {
            AddItemElement(inputsContainer, item?.Molecule);
        }

        foreach (var item in info.OutputItems)
        {
            AddItemElement(outputsContainer, item?.Molecule);
        }

        ShowTooltip();
    }

    private void ShowTooltipForResource()
    {
        label.Text = "Resource";

        ClearItems();

        AddItemElement(inputsContainer, currentResource);

        ShowTooltip();
    }

    private void ClearItems()
    {
        foreach (Node child in inputsContainer.GetChildren())
        {
            child.QueueFree();
        }

        foreach (Node child in outputsContainer.GetChildren())
        {
            child.QueueFree();
        }
    }

    private void AddItemElement(Control container, Molecule molecule)
    {
        var ui = itemUi.Instance();
        if (molecule != null)
        {
            ui.GetNode<Label>("Name").Text = molecule.Formula;
            ui.GetNode<Control>("ImageFrame/Image").Modulate = molecule.GetColor();
        }
        else
        {
            ui.GetNode<Label>("Name").Text = string.Empty;
            ui.GetNode<Control>("ImageFrame/Image").Modulate = new Color(0, 0, 0, 0);
        }

        container.AddChild(ui);
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
}
