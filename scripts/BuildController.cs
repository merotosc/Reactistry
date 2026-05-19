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
    private readonly Stack<BuildingOptions> draggedBuildings = new();
    private readonly HashSet<Vector2> draggedTiles = [];
    private Vector2? lastDragTile;
    private MouseButton pressedButton;

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
        if (currentBuilding.Type != BuildingType.None) // TODO: remove and use move to UpdateBuildingDrag?
        {
            var tilePosition = GetGlobalMousePosition().ToTilePosition();
            if (tilePosition != currentBuilding.Position)
            {
                currentBuilding.Position = tilePosition;
                RedrawPreview();
            }
        }
    }

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseButton mouseButton)
        {
            HandleMouseButton(mouseButton.Pressed, (MouseButton)mouseButton.ButtonIndex);
        }

        if (e is InputEventMouseMotion && pressedButton != MouseButton.None)
        {
            HandleMouseDrag();
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
                    CancelOperation();
                    break;
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
        RedrawPreview();
    }

    private void RotateBuilding(bool clockwise)
    {
        currentBuilding.Direction = clockwise
            ? currentBuilding.Direction.Next()
            : currentBuilding.Direction.Previous();
        RedrawPreview();
    }

    private void ChangeBuildingVariant(bool reverse)
    {
        var variantsCount = currentBuilding.Type.GetVariantsCountForBuilding();
        var offset = reverse ? -1 : 1;
        currentBuilding.Variant = (currentBuilding.Variant + offset + variantsCount) % currentBuilding.Type.GetVariantsCountForBuilding();
        RedrawPreview();
    }

    private void CancelOperation()
    {
        pressedButton = MouseButton.None;
        draggedBuildings.Clear();
        draggedTiles.Clear();
        lastDragTile = null;
        SelectBuilding(BuildingType.None);
    }

    private void HandleMouseButton(bool pressed, MouseButton button)
    {
        if (pressed && pressedButton == MouseButton.None)
        {
            pressedButton = button;
            HandleMouseDown();
        }
        else if (!pressed && pressedButton == button)
        {
            HandleMouseReleased();
            pressedButton = MouseButton.None;
        }
    }

    private void HandleMouseDown()
    {
        lastDragTile = null;
        HandleMouseDrag();
    }

    private void HandleMouseDrag()
    {
        var tilePosition = GetGlobalMousePosition().ToTilePosition();
        if (tilePosition == lastDragTile)
        {
            return;
        }

        lastDragTile = tilePosition;

        if (pressedButton == MouseButton.Right)
        {
            AddMissingDragTiles(tilePosition, UpdateDeletionDrag);
            UpdateDeletionDrag(tilePosition);
        }
        else if (pressedButton == MouseButton.Left)
        {
            if (currentBuilding.Type == BuildingType.Pipe)
            {
                AddMissingDragBuildings(tilePosition, UpdatePipesDrag);
                UpdatePipesDrag(tilePosition);
            }
            else if (currentBuilding.Type != BuildingType.None)
            {
                AddMissingDragBuildings(tilePosition, UpdateBuildingDrag);
                UpdateBuildingDrag(tilePosition);
            }
        }
    }

    private void HandleMouseReleased()
    {
        if (pressedButton == MouseButton.Right)
        {
            EndDeletionDrag();
        }
        else if (pressedButton == MouseButton.Left)
        {
            EndBuildingDrag();
        }
    }

    private void AddMissingDragBuildings(Vector2 tilePosition, Action<Vector2> action)
    {
        if (!draggedBuildings.TryPeek(out var previousBuilding))
        {
            return;
        }

        foreach (var position in previousBuilding.Position.EnumerateOrthogonalLinePositions(tilePosition))
        {
            action(position);
        }
    }

    private void AddMissingDragTiles(Vector2 tilePosition, Action<Vector2> action)
    {
        if (draggedTiles.Count == 0)
        {
            return;
        }

        foreach (var position in draggedTiles.Last().EnumerateOrthogonalLinePositions(tilePosition))
        {
            action(position);
        }
    }

    private void UpdatePipesDrag(Vector2 tilePosition)
    {
        var pipeExists = draggedBuildings.TryPeek(out var previousPipe);
        var direction = currentBuilding.Direction;
        var variant = currentBuilding.Variant;

        if (pipeExists)
        {
            if (previousPipe.Position == tilePosition)
            {
                return;
            }

            variant = (int)PipeVariant.Forward;

            var delta = tilePosition - previousPipe.Position;
            direction = delta.ToDirection();

            if (draggedBuildings.Count == 1)
            {
                previousPipe.Direction = direction;
                previousPipe.Variant = (int)PipeVariant.Forward;
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
            Variant = variant,
        };

        draggedBuildings.Push(pipe);
    }

    private void UpdateBuildingDrag(Vector2 tilePosition)
    {
        var building = new BuildingOptions
        {
            Type = currentBuilding.Type,
            Position = tilePosition, // TODO: not necessary as currentBuilding can be used directly as position is set in _Process
            Direction = currentBuilding.Direction,
            Variant = currentBuilding.Variant,
        };

        draggedBuildings.Push(building);
    }

    private void EndBuildingDrag()
    {
        if (draggedBuildings.Count == 0)
        {
            return;
        }

        var alreadyBuilded = new HashSet<Vector2>();

        foreach (var building in draggedBuildings)
        {
            if (!alreadyBuilded.Add(building.Position))
            {
                continue;
            }

            world.TryCreateBuilding(building);
        }

        draggedBuildings.Clear();
        RedrawPreview();
    }

    private void UpdateDeletionDrag(Vector2 tilePosition)
    {
        draggedTiles.Add(tilePosition);
        RedrawPreview();
    }

    private void EndDeletionDrag()
    {
        foreach (var tilePosition in draggedTiles)
        {
            world.TryDeleteBuilding(tilePosition);
        }

        draggedTiles.Clear();
        RedrawPreview();
    }

    private void RedrawPreview()
    {
        previewBaseTileMap.Clear();
        previewOverlayTileMap.Clear();

        if (pressedButton == MouseButton.Right && draggedTiles.Count > 0)
        {
            foreach (var tilePosition in draggedTiles)
            {
                previewOverlayTileMap.SetCellv(tilePosition, tile: Constants.TileSet.IconsId, autotileCoord: new Vector2(0, Constants.TileSet.IconsYOffset));
            }

            return;
        }

        if (currentBuilding.Type == BuildingType.None)
        {
            return;
        }

        previewBaseTileMap.DrawBuilding(currentBuilding, previewOverlayTileMap);

        if (draggedBuildings.Count > 1)
        {
            foreach (var building in draggedBuildings.Reverse())
            {
                previewBaseTileMap.DrawBuilding(building, previewOverlayTileMap);
            }
        }
    }
}
