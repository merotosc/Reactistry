using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts;

public class BuildController : Node2D
{
    private World world;
    private BuildingType currentBuilding; // TODO: store in currentBuildingOptions?
    private readonly BuildingOptions currentBuildingOptions = new();
    private TileMap previewTileMap;

    public void Init(World world)
    {
        this.world = world;
    }

    public override void _Ready()
    {
        previewTileMap = GetNode<TileMap>("/root/Game/PreviewTileMap");
        previewTileMap.Modulate = new Color(1, 1, 1, 0.5f);
    }

    public override void _Process(float delta)
    {
        if (currentBuilding != BuildingType.None)
        {
            var tilePosition = GetGlobalMousePosition().ToTilePosition();
            if (tilePosition != currentBuildingOptions.Position)
            {
                RedrawBuildingPreview();
            }
        }
    }

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseButton mouseButton && mouseButton.Pressed)
        {
            HandleClick(mouseButton.ButtonIndex == (int)ButtonList.Right);
        }

        if (e is InputEventKey key && key.Pressed)
        {
            if (key.Scancode is >= (uint)KeyList.Key0 and <= (uint)KeyList.Key9)
            {
                SelectBuilding(key.Scancode - (uint)KeyList.Key0);
            }
            else if (key.Scancode == (uint)KeyList.R)
            {
                RotateBuilding(!key.Shift);
            }
            else if (key.Scancode == (uint)KeyList.T)
            {
                ChangeBuildingVariant();
            }
        }
    }

    private void SelectBuilding(uint keyNumber)
    {
        currentBuilding = (BuildingType)keyNumber;
        RedrawBuildingPreview();
    }

    private void RotateBuilding(bool clockwise)
    {
        currentBuildingOptions.Direction = clockwise
            ? currentBuildingOptions.Direction.Next()
            : currentBuildingOptions.Direction.Previous();
        RedrawBuildingPreview();
    }

    private void ChangeBuildingVariant()
    {
        // TODO: set variant based on current type
        currentBuildingOptions.Variant = (currentBuildingOptions.Variant + 1) % 3;
        RedrawBuildingPreview();
    }

    private void HandleClick(bool rightClick = false)
    {
        if (rightClick)
        {
            var tilePosition = GetGlobalMousePosition().ToTilePosition();
            GD.PrintS("Requesting building deletion at posiiton", tilePosition);
            world.TryDeleteBuilding(tilePosition);
        }
        else if (currentBuilding != BuildingType.None)
        {
            GD.PrintS("Requesting building creation", currentBuilding, "at posiiton", currentBuildingOptions.Position);
            world.TryCreateBuilding(currentBuilding, currentBuildingOptions);
        }
    }

    private void RedrawBuildingPreview()
    {
        previewTileMap.Clear();

        if (currentBuilding == BuildingType.None)
        {
            return;
        }

        currentBuildingOptions.Position = GetGlobalMousePosition().ToTilePosition();
        previewTileMap.DrawBuilding(currentBuilding, currentBuildingOptions);
    }
}
