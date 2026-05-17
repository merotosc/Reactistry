using System;
using System.Collections.Generic;
using ChemFactory.scripts.Buildings;
using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts;

public class World
{
    private readonly Dictionary<Vector2, IBuilding> buildingTiles = [];
    private readonly List<IBuilding> entities = [];
    private readonly List<Item> items = [];

    public event Action<IBuilding> BuildingCreated;
    public event Action<Vector2> BuildingDeleted;
    public event Action<IEnumerable<Item>> ItemCreated;
    public event Action<IEnumerable<Item>> ItemDeleted;

    public void LoadDemo()
    {
        AddBuilding(new Pipe(new Vector2(-1, -1), Direction.Right));
        AddBuilding(new Pipe(new Vector2(-2, -1), Direction.Right));
        AddBuilding(new Pipe(new Vector2(-3, -1), Direction.Right));
        AddBuilding(new Pipe(new Vector2(-1, 0), Direction.Right));
        AddBuilding(new Pipe(new Vector2(-2, 0), Direction.Right));
        AddBuilding(new Pipe(new Vector2(-3, 0), Direction.Right));

        AddBuilding(new Pipe(new Vector2(1, 0), Direction.Right));
        AddBuilding(new Pipe(new Vector2(2, 0), Direction.Right));
        AddBuilding(new Pipe(new Vector2(3, 0), Direction.Right));
        AddBuilding(new Pipe(new Vector2(4, 0), Direction.Right));
        AddBuilding(new Pipe(new Vector2(5, 0), Direction.Right));

        AddBuilding(new Producer(new Vector2(-4, -1), Direction.Right, new([new(AtomElement.H, 2)])));
        AddBuilding(new Producer(new Vector2(-4, 0), Direction.Right, new([new(AtomElement.O, 2)])));
        AddBuilding(new Reactor(new Vector2(0, 0), Direction.Right));

        AddBuilding(new Pipe(new Vector2(-1, -5), Direction.Right));
        AddBuilding(new Pipe(new Vector2(-1, -4), Direction.Right));
        AddBuilding(new Pipe(new Vector2(-1, -3), Direction.Right));

        AddBuilding(new Pipe(new Vector2(1, -3), Direction.Right));
        AddBuilding(new Pipe(new Vector2(2, -3), Direction.Right));
        AddBuilding(new Pipe(new Vector2(3, -3), Direction.Right));

        AddBuilding(new Producer(new Vector2(-2, -5), Direction.Right, new([new(AtomElement.C, 1)])));
        AddBuilding(new Producer(new Vector2(-2, -4), Direction.Right, new([new(AtomElement.C, 1)])));
        AddBuilding(new Producer(new Vector2(-2, -3), Direction.Right, new([new(AtomElement.O, 2)])));
        AddBuilding(new Reactor(new Vector2(0, -3), Direction.Right, variant: 1));
    }

    public void Tick(float delta)
    {
        UpdateEntities(delta);
        UpdateItems(delta);
    }

    public void AddItems(IEnumerable<Item> itemsToAdd)
    {
        items.AddRange(itemsToAdd);
        ItemCreated?.Invoke(itemsToAdd);
    }

    public bool TryDeleteItems(IEnumerable<Item> itemsToRemove)
    {
        var allRemoved = true;

        foreach (var item in itemsToRemove)
        {
            allRemoved &= items.Remove(item);
        }

        ItemDeleted?.Invoke(itemsToRemove);

        return allRemoved;
    }

    public bool TryMoveItem(Item item, Vector2 targetPosition, Direction fromDirection)
    {
        if (buildingTiles.TryGetValue(targetPosition, out var building)
            && building.TryConsumeItem(item, targetPosition, fromDirection))
        {
            var overshoot = item.DistanceOvershoot;
            item.TilePosition = targetPosition;
            item.Path = building.GetItemPath(item.TilePosition); // TODO: set item path inside building directly or return as out param?
            item.DistanceOnPath = overshoot;
            return true;
        }

        return false;
    }

    public bool TryGetBuilding(Vector2 position, out IBuilding building)
        => buildingTiles.TryGetValue(position, out building);

    public bool TryCreateBuilding(BuildingOptions buildingOptions)
    {
        foreach (var tilePosition in buildingOptions.Position.EnumeratePositions(buildingOptions.Direction, buildingOptions.Type.GetSizeForBuilding(buildingOptions.Variant)))
        {
            if (buildingTiles.ContainsKey(tilePosition))
            {
                return false;
            }
        }

        IBuilding building = buildingOptions.Type switch
        {
            BuildingType.Pipe => new Pipe(buildingOptions.Position, buildingOptions.Direction, (PipeVariant)buildingOptions.Variant),
            BuildingType.Producer => new Producer(buildingOptions.Position, buildingOptions.Direction, Molecule.InvalidMolecule),
            BuildingType.Consumer => new Consumer(buildingOptions.Position, buildingOptions.Direction),
            BuildingType.Reactor => new Reactor(buildingOptions.Position, buildingOptions.Direction, buildingOptions.Variant),
            BuildingType.Splitter => new Splitter(buildingOptions.Position, buildingOptions.Direction, buildingOptions.Variant),
            BuildingType.Merger => new Merger(buildingOptions.Position, buildingOptions.Direction, buildingOptions.Variant),
            _ => null,
        };

        if (building == null)
        {
            GD.PrintErr("Building to create not found", buildingOptions.Type);
            return false;
        }

        AddBuilding(building);
        return true;
    }

    public bool TryDeleteBuilding(Vector2 position)
    {
        if (!buildingTiles.TryGetValue(position, out var building))
        {
            return false;
        }

        RemoveBuildings(building);
        return true;
    }

    private void AddBuilding(IBuilding building)
    {
        entities.Add(building);

        foreach (var tilePosition in building.AnchorPosition.EnumeratePositions(building.Direction, building.Size))
        {
            buildingTiles.Add(tilePosition, building);
        }

        BuildingCreated?.Invoke(building);
    }

    private void RemoveBuildings(IBuilding building)
    {
        TryDeleteItems(building.GetItems());

        entities.Remove(building);

        foreach (var tilePosition in building.AnchorPosition.EnumeratePositions(building.Direction, building.Size))
        {
            buildingTiles.Remove(tilePosition);
            BuildingDeleted?.Invoke(tilePosition); // TODO: call once with list of positions?
        }
    }

    private void UpdateItems(float delta)
    {
        foreach (var item in items)
        {
            item.TravelDistance(delta * Constants.ItemSpeed);
        }
    }

    private void UpdateEntities(float delta)
    {
        foreach (var building in entities)
        {
            building.Update(this, delta);
        }
    }
}
