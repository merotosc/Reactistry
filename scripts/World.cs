using System;
using System.Collections.Generic;
using Reactistry.scripts.Buildings;
using Reactistry.scripts.Models;
using Reactistry.scripts.Utilities;
using Godot;

namespace Reactistry.scripts;

public class World
{
    private readonly Dictionary<Vector2, Molecule> resourceTiles = [];
    private readonly Dictionary<Vector2, IBuilding> buildingTiles = [];
    private readonly List<IBuilding> buildings = [];
    private readonly List<Item> items = [];

    public IReadOnlyList<IBuilding> Buildings => buildings;
    public event Action<Vector2, Molecule> ResourceCreated;
    public event Action<IBuilding> BuildingCreated;
    public event Action<Vector2> BuildingDeleted;
    public event Action<IEnumerable<Item>> ItemsCreated;
    public event Action<IEnumerable<Item>> ItemsDeleted;

    public void LoadWorld(SaveData saveData)
    {
        GenerateWorld();

        foreach (var building in saveData.Buildings)
        {
            if (!TryCreateBuilding(building))
            {
                GD.PrintErr("Could not create building from save data: ", building.Type);
            }
        }
    }

    public void GenerateWorld()
    {
        AddBuilding(new Lab(new Vector2(-2, 2), Direction.Right));
        ResourcesRandomizer.SpawnResources((position, molecule) =>
        {
            resourceTiles.Add(position, molecule);
            ResourceCreated?.Invoke(position, molecule);
        });
    }

    public void Tick(float delta)
    {
        UpdateEntities(delta);
        UpdateItems(delta);
    }

    public void AddItems(params IEnumerable<Item> itemsToAdd)
    {
        items.AddRange(itemsToAdd);
        ItemsCreated?.Invoke(itemsToAdd);
    }

    public void DeleteItems(params IEnumerable<Item> itemsToRemove)
    {
        foreach (var item in itemsToRemove)
        {
            items.Remove(item);
        }

        ItemsDeleted?.Invoke(itemsToRemove);
    }

    public bool TryMoveItem(Item item, Vector2 targetPosition, Direction fromDirection)
    {
        if (buildingTiles.TryGetValue(targetPosition, out var building)
            && building.TryConsumeItem(item, targetPosition, fromDirection))
        {
            var overshoot = item.DistanceOvershoot;
            item.TilePosition = targetPosition;
            item.Path = building.GetItemPath(item.TilePosition, fromDirection); // TODO: set item path inside building directly or return as out param?
            item.DistanceOnPath = overshoot;
            return true;
        }

        return false;
    }

    public bool TryGetResource(Vector2 position, out Molecule resource)
        => resourceTiles.TryGetValue(position, out resource);

    public bool TryGetBuilding(Vector2 position, out IBuilding building)
        => buildingTiles.TryGetValue(position, out building);

    public bool TryCreateBuilding(BuildingOptions buildingOptions)
    {
        if (!ValidBuildingPosition(buildingOptions))
        {
            return false;
        }

        IBuilding building = buildingOptions.Type switch
        {
            BuildingType.Pipe => new Pipe(buildingOptions.Position, buildingOptions.Direction, (PipeVariant)buildingOptions.Variant),
            BuildingType.Extractor => new Extractor(buildingOptions.Position, buildingOptions.Direction, resourceTiles.GetValueOrDefault(buildingOptions.Position, Molecule.InvalidMolecule)),
            BuildingType.Destroyer => new Destroyer(buildingOptions.Position, buildingOptions.Direction),
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
        if (!buildingTiles.TryGetValue(position, out var building) || building is Lab)
        {
            return false;
        }

        RemoveBuildings(building);
        return true;
    }

    public bool ValidBuildingPosition(BuildingOptions buildingOptions)
    {
        var buildingSize = buildingOptions.Type.GetDefinition().GetSize(buildingOptions.Variant);
        foreach (var tilePosition in buildingOptions.Position.EnumeratePositions(buildingOptions.Direction, buildingSize))
        {
            if (buildingTiles.ContainsKey(tilePosition))
            {
                return false;
            }

            var resourceExists = resourceTiles.ContainsKey(tilePosition);

            switch (buildingOptions.Type)
            {
                case BuildingType.Extractor:
                    if (!resourceExists)
                        return false;
                    break;
                default:
                    if (resourceExists)
                        return false;
                    break;
            }

        }

        return true;
    }

    private void AddBuilding(IBuilding building)
    {
        // TODO: add check to avoid duplicate buildings and key
        buildings.Add(building);

        foreach (var tilePosition in building.AnchorPosition.EnumeratePositions(building.Direction, building.Size))
        {
            buildingTiles.Add(tilePosition, building);
        }

        BuildingCreated?.Invoke(building);
    }

    private void RemoveBuildings(IBuilding building)
    {
        DeleteItems(building.GetItems());

        // TODO: add check to avoid missing building and key
        buildings.Remove(building);

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
        foreach (var building in buildings)
        {
            building.Update(this, delta);
        }
    }
}
