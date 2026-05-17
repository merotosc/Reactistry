using System;
using System.Collections.Generic;
using System.Linq;
using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts;

public class BuildController : Node2D
{
    private TileMap previewBaseTileMap;
    private TileMap previewOverlayTileMap;
    private World world;
    private readonly BuildingOptions currentBuilding = new();
    private readonly Stack<BuildingOptions> pipes = new();
    private bool isDragging;
    private Vector2? lastDragTile;

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
                currentBuilding.Position = tilePosition;
                RedrawBuildingPreview();
            }
        }
    }

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseButton mouseButton)
        {
            HandleMouseButton(mouseButton.Pressed, (ButtonList)mouseButton.ButtonIndex);
        }

        if (e is InputEventMouseMotion && isDragging)
        {
            UpdateDrag();
        }

        if (e is InputEventKey key && key.Pressed)
        {
            switch (key.Scancode)
            {
                case >= (uint)KeyList.Key0 and <= (uint)KeyList.Key9:
                    {
                        var keyNumber = key.Scancode - (uint)KeyList.Key0;
                        SelectBuilding((BuildingType)keyNumber);
                        break;
                    }
                case (uint)KeyList.R:
                    RotateBuilding(!key.Shift);
                    break;
                case (uint)KeyList.T:
                    ChangeBuildingVariant(key.Shift);
                    break;
                case (uint)KeyList.Escape:
                    SelectBuilding(BuildingType.None);
                    break;
            }
        }
    }

    private void HandleMouseButton(bool pressed, ButtonList button)
    {
        if (!pressed)
        {
            HandleMouseReleased(button == ButtonList.Right);
        }

        if (button == ButtonList.Left)
        {
            if (pressed)
            {
                StartDrag();
            }
            else
            {
                EndDrag();
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

    private void HandleMouseReleased(bool rightClick = false)
    {
        if (rightClick)
        {
            var tilePosition = GetGlobalMousePosition().ToTilePosition();
            GD.PrintS("Requesting building deletion at posiiton", tilePosition);
            world.TryDeleteBuilding(tilePosition);
        }
        else if (currentBuilding.Type == BuildingType.Pipe && pipes.Count > 1)
        {
            var buildedPipes = new HashSet<Vector2>();

            foreach (var pipe in pipes)
            {
                if (!buildedPipes.Add(pipe.Position))
                {
                    continue;
                }

                world.TryCreateBuilding(pipe);
            }
        }
        else if (currentBuilding.Type != BuildingType.None)
        {
            GD.PrintS("Requesting building creation", currentBuilding.Type, "at posiiton", currentBuilding.Position);
            world.TryCreateBuilding(currentBuilding);
        }
    }

    private void StartDrag()
    {
        if (currentBuilding.Type != BuildingType.Pipe)
        {
            return;
        }

        isDragging = true;
        pipes.Clear();
        lastDragTile = null;

        UpdateDrag();
    }

    private void UpdateDrag()
    {
        if (currentBuilding.Type != BuildingType.Pipe)
        {
            return;
        }

        var tilePosition = GetGlobalMousePosition().ToTilePosition();
        if (tilePosition == lastDragTile)
        {
            return;
        }

        lastDragTile = tilePosition;

        var pipeExists = pipes.TryPeek(out var previousPipe);
        var direction = Direction.Right;

        if (pipeExists)
        {
            var delta = tilePosition - previousPipe.Position;
            direction = delta.ToDirection();

            if (pipes.Count == 1)
            {
                previousPipe.Direction = direction;
            }
            else if (direction != previousPipe.Direction)
            {
                previousPipe.Variant = previousPipe.Direction.Next() == direction
                    ? (int)PipeVariant.Right
                    : (int)PipeVariant.Left;
            }
        }

        var pipe = new BuildingOptions
        {
            Type = BuildingType.Pipe,
            Position = tilePosition,
            Direction = direction,
            Variant = (int)PipeVariant.Forward,
        };

        pipes.Push(pipe);
        RedrawBuildingPreview();
    }

    private void EndDrag()
    {
        if (currentBuilding.Type != BuildingType.Pipe)
        {
            return;
        }

        isDragging = false;
        pipes.Clear();
        RedrawBuildingPreview();
    }

    private void RedrawBuildingPreview()
    {
        previewBaseTileMap.Clear();
        previewOverlayTileMap.Clear();

        if (currentBuilding.Type == BuildingType.None)
        {
            return;
        }

        previewBaseTileMap.DrawBuilding(currentBuilding, previewOverlayTileMap);

        if (pipes.Count > 1)
        {
            foreach (var pipe in pipes.Reverse())
            {
                previewBaseTileMap.DrawBuilding(pipe, previewOverlayTileMap);
            }
        }
    }
}
