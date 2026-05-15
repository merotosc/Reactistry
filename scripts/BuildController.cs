using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts;

public class BuildController : Node2D
{
    private World world;
    private EntityType currentEntity;
    private Vector2 currentEntityTileCoord;
    private readonly EntityOptions currentEntityOptions = new();
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
            var tilePosition = GetGlobalMousePosition().ToTilePosition();
            if (tilePosition != currentEntityOptions.Position)
            {
                RedrawEntityPreview();
            }
        }
    }

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseButton mouseButton && mouseButton.Pressed)
        {
            HandleClick();
        }

        if (e is InputEventKey key && key.Pressed)
        {
            if (key.Scancode is >= (uint)KeyList.Key0 and <= (uint)KeyList.Key9)
            {
                SelectEntity(key.Scancode - (uint)KeyList.Key0);
            }
            else if (key.Scancode == (uint)KeyList.R)
            {
                RotateEntity(!key.Shift);
            }
        }
    }

    private void SelectEntity(uint keyNumber)
    {
        currentEntity = (EntityType)keyNumber;
        currentEntityTileCoord = currentEntity.GetTileCoordForEntity();
        RedrawEntityPreview();
    }

    private void RotateEntity(bool clockwise)
    {
        currentEntityOptions.Direction = clockwise
            ? currentEntityOptions.Direction.NextDirection()
            : currentEntityOptions.Direction.PreviousDirection();
        RedrawEntityPreview();
    }

    private void HandleClick()
    {
        if (currentEntity == EntityType.None)
        {
            return;
        }

        GD.PrintS("Requesting entity creation", currentEntity, "at posiiton", currentEntityOptions.Position);
        world.TryCreateEntity(currentEntity, currentEntityOptions);
    }

    private void RedrawEntityPreview()
    {
        previewTileMap.Clear();

        if (currentEntity == EntityType.None)
        {
            return;
        }

        var tilePosition = GetGlobalMousePosition().ToTilePosition();
        currentEntityOptions.Position = tilePosition;
        // TODO: tile map utility method
        var (mirror, rotate) = currentEntityOptions.Direction.GetTileOptionsForDirection();
        previewTileMap.SetCellv(tilePosition, tile: 0, autotileCoord: currentEntityTileCoord, flipX: mirror && !rotate, flipY: mirror && rotate, transpose: rotate);
    }
}
