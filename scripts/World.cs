using System;
using System.Collections.Generic;
using System.Linq;
using ChemFactory.scripts.Buildings;
using ChemFactory.scripts.Items;
using ChemFactory.scripts.Models;
using ChemFactory.scripts.Transports;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts;

public class World
{
    public Dictionary<Vector2, Belt> Belts { get; set; } = [];

    public Dictionary<Vector2, IBuilding> BuildingTiles { get; set; } = [];

    public List<IBuilding> Buildings { get; set; } = [];

    public List<Item> Items { get; set; } = [];

    public event Action<IEntity, EntityOptions> EntityCreated;
    public event Action<Vector2> EntityDeleted;

    public void LoadDemo()
    {
        Belts.Add(new Vector2(-1, -1), new Belt(Direction.Right));
        Belts.Add(new Vector2(-2, -1), new Belt(Direction.Right));
        Belts.Add(new Vector2(-3, -1), new Belt(Direction.Right));
        Belts.Add(new Vector2(-1, 0), new Belt(Direction.Right));
        Belts.Add(new Vector2(-2, 0), new Belt(Direction.Right));
        Belts.Add(new Vector2(-3, 0), new Belt(Direction.Right));

        Belts.Add(new Vector2(1, 0), new Belt(Direction.Right));
        Belts.Add(new Vector2(2, 0), new Belt(Direction.Right));
        Belts.Add(new Vector2(3, 0), new Belt(Direction.Right));
        Belts.Add(new Vector2(4, 0), new Belt(Direction.Right));
        Belts.Add(new Vector2(5, 0), new Belt(Direction.Right));

        AddBuilding(new Producer(new Vector2(-4, -1), Direction.Right) { ItemType = ItemType.H });
        AddBuilding(new Producer(new Vector2(-4, 0), Direction.Right) { ItemType = ItemType.O });
        AddBuilding(new Reactor(new Vector2(0, 0), Direction.Right));

        foreach (var (position, belt) in Belts)
        {
            EntityCreated?.Invoke(belt, new() { Position = position, Direction = belt.Direction });
        }

        foreach (var building in Buildings)
        {
            EntityCreated?.Invoke(building, new() { Position = building.AnchorPosition, Direction = building.Direction });
        }
    }

    public void Tick(float delta)
    {
        UpdateBeltItems(delta);
        UpdateBuildings(delta);
    }

    public bool TryCreateItem(ItemType itemType, Vector2 outputPosition, Direction outputDirection)
    {
        var tilePosition = outputPosition + outputDirection.ToVector();

        var item = new Item
        {
            Type = itemType,
            TilePosition = tilePosition,
            Progress = 0,
        };

        var created = TryMoveItemToEntity(item, tilePosition, outputDirection);
        if (created)
        {
            Items.Add(item);
        }

        return created;
    }

    public bool TryCreateEntity(EntityType entityType, EntityOptions entityOptions)
    {
        foreach (var tilePosition in entityOptions.Position.EnumeratePositions(entityOptions.Direction, entityType.GetSizeForEntity(entityOptions.Variant)))
        {
            if (Belts.ContainsKey(tilePosition) || BuildingTiles.ContainsKey(tilePosition))
            {
                return false;
            }
        }

        IEntity entity = entityType switch
        {
            EntityType.Belt => new Belt(entityOptions.Direction, (BeltVariant)entityOptions.Variant),
            EntityType.Producer => new Producer(entityOptions.Position, entityOptions.Direction),
            EntityType.Consumer => new Consumer(entityOptions.Position, entityOptions.Direction),
            EntityType.Reactor => new Reactor(entityOptions.Position, entityOptions.Direction, entityOptions.Variant + 2),
            _ => null,
        };

        if (entity is Belt belt)
        {
            Belts.Add(entityOptions.Position, belt);
        }
        else if (entity is IBuilding building)
        {
            AddBuilding(building);
        }

        EntityCreated?.Invoke(entity, entityOptions);

        return true;
    }

    public bool TryDeleteEntity(Vector2 position)
    {
        IBuilding building = null;

        if (!Belts.TryGetValue(position, out var belt) && !BuildingTiles.TryGetValue(position, out building))
        {
            return false;
        }

        if (belt != null)
        {
            if (belt.Item != null)
            {
                Items.Remove(belt.Item);
            }

            Belts.Remove(position);
            EntityDeleted?.Invoke(position);
        }
        else if (building != null)
        {
            // TODO: create separate method RemoveBuilding as for AddBuilding
            Buildings.Remove(building);

            foreach (var tilePosition in building.AnchorPosition.EnumeratePositions(building.Direction, building.Size))
            {
                BuildingTiles.Remove(tilePosition);
                EntityDeleted?.Invoke(tilePosition); // TODO: call once with list of positions?
            }
        }

        return true;
    }

    private void UpdateBeltItems(float delta)
    {
        var beltsWithItems = Belts
            .Where(x => x.Value.Item != null)
            .ToList();

        foreach (var (position, belt) in beltsWithItems)
        {
            MoveOnBelt(position, belt, delta);

            if (belt.Item.Progress >= 1)
            {
                var moved = TryMoveToNextTile(position, belt);
                if (!moved)
                {
                    belt.Item.Progress = 1;
                }
            }
        }
    }

    private void AddBuilding(IBuilding building)
    {
        Buildings.Add(building);

        foreach (var tilePosition in building.AnchorPosition.EnumeratePositions(building.Direction, building.Size))
        {
            BuildingTiles.Add(tilePosition, building);
        }
    }

    private void UpdateBuildings(float delta)
    {
        foreach (var building in Buildings)
        {
            building.Update(this, delta);
        }
    }

    private bool TryMoveToNextTile(Vector2 position, Belt belt)
    {
        var tilePosition = position + belt.OutputDirection.ToVector();
        var moved = TryMoveItemToEntity(belt.Item, tilePosition, belt.OutputDirection);
        if (moved)
        {
            belt.Item = null;
        }

        return moved;
    }

    private bool TryMoveItemToEntity(Item item, Vector2 position, Direction outputDirection)
    {
        if (TryMoveItemToEntity(item, position, outputDirection, Belts))
        {
            item.Progress %= 1f;
            item.TilePosition = position;
            return true;
        }

        if (TryMoveItemToEntity(item, position, outputDirection, BuildingTiles))
        {
            item.Progress = 0;
            item.TilePosition = position;
            Items.Remove(item);
            return true;
        }

        return false;
    }

    private static bool TryMoveItemToEntity<T>(Item item, Vector2 position, Direction outputDirection, Dictionary<Vector2, T> entities)
        where T : IEntity
    {
        return entities.TryGetValue(position, out var entity) && entity.TryConsumeItem(item, position, outputDirection.ReverseDirection());
    }

    private static void MoveOnBelt(Vector2 position, Belt belt, float delta)
    {
        belt.Item.Progress += delta * belt.Speed;
    }
}
