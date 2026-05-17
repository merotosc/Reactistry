using System;
using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts;

public class BuildController : Node2D
{
    private World world;
    private readonly BuildingOptions currentBuilding = new();
    private TileMap previewBaseTileMap;
    private TileMap previewOverlayTileMap;

    public void Init(World world)
    {
        this.world = world;
    }

    public override void _Ready()
    {
        previewBaseTileMap = GetNode<TileMap>("PreviewBaseTileMap");
        previewOverlayTileMap = GetNode<TileMap>("PreviewOverlayTileMap");
        previewBaseTileMap.Modulate = new Color(1, 1, 1, 0.5f);
        previewOverlayTileMap.Modulate = new Color(1, 1, 1, 0.5f);
    }

    public override void _Process(float delta)
    {
        if (currentBuilding.Type != BuildingType.None)
        {
            var tilePosition = GetGlobalMousePosition().ToTilePosition();
            if (tilePosition != currentBuilding.Position)
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
                var keyNumber = key.Scancode - (uint)KeyList.Key0;
                SelectBuilding((BuildingType)keyNumber);
            }
            else if (key.Scancode == (uint)KeyList.R)
            {
                RotateBuilding(!key.Shift);
            }
            else if (key.Scancode == (uint)KeyList.T)
            {
                ChangeBuildingVariant(key.Shift);
            }
            else if (key.Scancode == (uint)KeyList.Escape)
            {
                SelectBuilding((uint)BuildingType.None);
            }
        }
    }

    private void SelectBuilding(BuildingType buildingType)
    {
        if (!Enum.IsDefined(typeof(BuildingType), buildingType))
        {
            return;
        }

        currentBuilding.Type = buildingType;
        currentBuilding.Variant = 0;
        RedrawBuildingPreview();
    }

    private void RotateBuilding(bool clockwise)
    {
        currentBuilding.Direction = clockwise
            ? currentBuilding.Direction.Next()
            : currentBuilding.Direction.Previous();
        RedrawBuildingPreview();
    }

    private void ChangeBuildingVariant(bool reverse)
    {
        var variantsCount = currentBuilding.Type.GetVariantsCountForBuilding();
        var offset = reverse ? -1 : 1;
        currentBuilding.Variant = (currentBuilding.Variant + offset + variantsCount) % currentBuilding.Type.GetVariantsCountForBuilding();
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
        else if (currentBuilding.Type != BuildingType.None)
        {
            GD.PrintS("Requesting building creation", currentBuilding.Type, "at posiiton", currentBuilding.Position);
            world.TryCreateBuilding(currentBuilding);
        }
    }

    private void RedrawBuildingPreview()
    {
        previewBaseTileMap.Clear();
        previewOverlayTileMap.Clear();

        if (currentBuilding.Type == BuildingType.None)
        {
            return;
        }

        currentBuilding.Position = GetGlobalMousePosition().ToTilePosition();
        previewBaseTileMap.DrawBuilding(currentBuilding, previewOverlayTileMap);
    }
}
