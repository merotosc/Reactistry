using System;
using System.Collections.Generic;
using ChemFactory.scripts.Entities;
using ChemFactory.scripts.Items;
using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts;

public class World
{
    // TODO: make private?
    public Dictionary<Vector2, IEntity> EntityTiles { get; set; } = [];

    public List<IEntity> Entities { get; set; } = [];

    public List<Item> Items { get; set; } = [];

    public event Action<IEntity, EntityOptions> EntityCreated;
    public event Action<Vector2> EntityDeleted;

    public void LoadDemo()
    {
        AddEntity(new Belt(new Vector2(-1, -1), Direction.Right));
        AddEntity(new Belt(new Vector2(-2, -1), Direction.Right));
        AddEntity(new Belt(new Vector2(-3, -1), Direction.Right));
        AddEntity(new Belt(new Vector2(-1, 0), Direction.Right));
        AddEntity(new Belt(new Vector2(-2, 0), Direction.Right));
        AddEntity(new Belt(new Vector2(-3, 0), Direction.Right));

        AddEntity(new Belt(new Vector2(1, 0), Direction.Right));
        AddEntity(new Belt(new Vector2(2, 0), Direction.Right));
        AddEntity(new Belt(new Vector2(3, 0), Direction.Right));
        AddEntity(new Belt(new Vector2(4, 0), Direction.Right));
        AddEntity(new Belt(new Vector2(5, 0), Direction.Right));

        AddEntity(new Producer(new Vector2(-4, -1), Direction.Right) { Molecule = new([new(AtomElement.H, 2)]) });
        AddEntity(new Producer(new Vector2(-4, 0), Direction.Right) { Molecule = new([new(AtomElement.O, 2)]) });
        AddEntity(new Reactor(new Vector2(0, 0), Direction.Right));

        AddEntity(new Belt(new Vector2(-1, -5), Direction.Right));
        AddEntity(new Belt(new Vector2(-1, -4), Direction.Right));
        AddEntity(new Belt(new Vector2(-1, -3), Direction.Right));

        AddEntity(new Belt(new Vector2(1, -3), Direction.Right));
        AddEntity(new Belt(new Vector2(2, -3), Direction.Right));
        AddEntity(new Belt(new Vector2(3, -3), Direction.Right));

        AddEntity(new Producer(new Vector2(-2, -5), Direction.Right) { Molecule = new([new(AtomElement.C, 1)]) });
        AddEntity(new Producer(new Vector2(-2, -4), Direction.Right) { Molecule = new([new(AtomElement.C, 1)]) });
        AddEntity(new Producer(new Vector2(-2, -3), Direction.Right) { Molecule = new([new(AtomElement.O, 2)]) });
        AddEntity(new Reactor(new Vector2(0, -3), Direction.Right, inputsCount: 3));

        foreach (var (position, belt) in EntityTiles)
        {
            EntityCreated?.Invoke(belt, new() { Position = position, Direction = belt.Direction });
        }

        foreach (var building in Entities)
        {
            EntityCreated?.Invoke(building, new() { Position = building.AnchorPosition, Direction = building.Direction });
        }
    }

    public void Tick(float delta)
    {
        UpdateEntities(delta);
        UpdateItems();
    }

    public bool TryCreateItem(Molecule molecule, Vector2 outputPosition, Direction outputDirection)
    {
        GD.PrintS("Requested to create molecule", molecule);

        var tilePosition = outputPosition + outputDirection.ToVector();

        var item = new Item
        {
            Molecule = molecule,
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

    public bool TryDeleteItems(IEnumerable<Item> items)
    {
        var allRemoved = true;

        foreach (var item in items)
        {
            allRemoved &= Items.Remove(item);
        }

        return allRemoved;
    }

    public bool TryCreateEntity(EntityType entityType, EntityOptions entityOptions)
    {
        foreach (var tilePosition in entityOptions.Position.EnumeratePositions(entityOptions.Direction, entityType.GetSizeForEntity(entityOptions.Variant)))
        {
            if (EntityTiles.ContainsKey(tilePosition))
            {
                return false;
            }
        }

        IEntity entity = entityType switch
        {
            EntityType.Belt => new Belt(entityOptions.Position, entityOptions.Direction, (BeltVariant)entityOptions.Variant),
            EntityType.Producer => new Producer(entityOptions.Position, entityOptions.Direction),
            EntityType.Consumer => new Consumer(entityOptions.Position, entityOptions.Direction),
            EntityType.Reactor => new Reactor(entityOptions.Position, entityOptions.Direction, entityOptions.Variant + 2),
            _ => null,
        };

        AddEntity(entity);
        EntityCreated?.Invoke(entity, entityOptions);

        return true;
    }

    public bool TryDeleteEntity(Vector2 position)
    {
        if (!EntityTiles.TryGetValue(position, out var entity))
        {
            return false;
        }

        TryDeleteItems(entity.GetItems());

        // TODO: create separate method RemoveBuilding as for AddBuilding
        Entities.Remove(entity);

        foreach (var tilePosition in entity.AnchorPosition.EnumeratePositions(entity.Direction, entity.Size))
        {
            EntityTiles.Remove(tilePosition);
            EntityDeleted?.Invoke(tilePosition); // TODO: call once with list of positions?
        }

        return true;
    }

    private void AddEntity(IEntity entity)
    {
        Entities.Add(entity);

        foreach (var tilePosition in entity.AnchorPosition.EnumeratePositions(entity.Direction, entity.Size))
        {
            EntityTiles.Add(tilePosition, entity);
        }
    }

    private void UpdateItems()
    {
        foreach (var (position, entity) in EntityTiles)
        {
            // TODO: move logic in belt update?
            if (entity is Belt belt && belt.Item?.Progress >= 1)
            {
                var moved = TryMoveToNextTile(position, belt);
                if (!moved)
                {
                    belt.Item.Progress = 1;
                }
            }
        }
    }

    private void UpdateEntities(float delta)
    {
        foreach (var entity in Entities)
        {
            entity.Update(this, delta);
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
        if (TryMoveItemToEntity(item, position, outputDirection, EntityTiles))
        {
            item.Progress %= 1f;
            item.TilePosition = position;
            return true;
        }

        return false;
    }

    private static bool TryMoveItemToEntity<T>(Item item, Vector2 position, Direction outputDirection, Dictionary<Vector2, T> entities)
        where T : IEntity
    {
        return entities.TryGetValue(position, out var entity)
            && entity.TryConsumeItem(item, position, outputDirection.ReverseDirection());
    }
}
