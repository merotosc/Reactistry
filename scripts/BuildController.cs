using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts;

public class BuildController : Node2D
{
    private World world;
    private EntityType currentEntity; // TODO: store in currentEntityOptions?
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
            HandleClick(mouseButton.ButtonIndex == (int)ButtonList.Right);
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
            else if (key.Scancode == (uint)KeyList.T)
            {
                ChangeEntityVariant();
            }
        }
    }

    private void SelectEntity(uint keyNumber)
    {
        currentEntity = (EntityType)keyNumber;
        RedrawEntityPreview();
    }

    private void RotateEntity(bool clockwise)
    {
        currentEntityOptions.Direction = clockwise
            ? currentEntityOptions.Direction.Next()
            : currentEntityOptions.Direction.Previous();
        RedrawEntityPreview();
    }

    private void ChangeEntityVariant()
    {
        // TODO: set variant based on current type
        currentEntityOptions.Variant = (currentEntityOptions.Variant + 1) % 3;
        RedrawEntityPreview();
    }

    private void HandleClick(bool rightClick = false)
    {
        if (rightClick)
        {
            var tilePosition = GetGlobalMousePosition().ToTilePosition();
            GD.PrintS("Requesting entity deletion at posiiton", tilePosition);
            world.TryDeleteEntity(tilePosition);
        }
        else if (currentEntity != EntityType.None)
        {
            GD.PrintS("Requesting entity creation", currentEntity, "at posiiton", currentEntityOptions.Position);
            world.TryCreateEntity(currentEntity, currentEntityOptions);
        }
    }

    private void RedrawEntityPreview()
    {
        previewTileMap.Clear();

        if (currentEntity == EntityType.None)
        {
            return;
        }

        currentEntityOptions.Position = GetGlobalMousePosition().ToTilePosition();
        previewTileMap.DrawEntity(currentEntity, currentEntityOptions);
    }
}
