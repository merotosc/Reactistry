using System;
using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts;

public class BuildController : Node2D
{
    private World world;
    private EntityType currentEntity;
    private Vector2 currentEntityTile;
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
        if (currentEntity != EntityType.None)
        {
            DisplayEntityPreview();
        }
    }

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseButton mouseButton && mouseButton.Pressed)
        {
            var tilePosition = GetGlobalMousePosition().ToTilePosition();
            HandleClick(tilePosition);
        }

        if (e is InputEventKey key && key.Pressed)
        {
            if (key.Scancode is >= (uint)KeyList.Key0 and <= (uint)KeyList.Key9)
            {
                var selection = key.Scancode - (uint)KeyList.Key0;
                currentEntity = (EntityType)selection;
                currentEntityTile = currentEntity.GetTileForEntity();
            }
        }
    }

    private void HandleClick(Vector2 tilePosition)
    {
        if (currentEntity == EntityType.None)
        {
            return;
        }

        GD.PrintS("Requesting entity creation", currentEntity, "at posiiton", tilePosition);
        world.TryCreateEntity(currentEntity, tilePosition);
    }

    private void DisplayEntityPreview()
    {
        var tilePosition = GetGlobalMousePosition().ToTilePosition();
        previewTileMap.Clear();
        previewTileMap.SetCellv(tilePosition, 0, autotileCoord: currentEntityTile);
    }
}
