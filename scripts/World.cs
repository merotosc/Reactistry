using System;
using System.Collections.Generic;
using ChemFactory.scripts.Entities;
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

        foreach (var entity in Entities)
        {
            EntityCreated?.Invoke(entity, new() { Position = entity.AnchorPosition, Direction = entity.Direction });
        }
    }

    public void Tick(float delta)
    {
        UpdateEntities(delta);
        UpdateItems(delta);
    }

    public bool TryCreateItem(Molecule molecule, Vector2 sourcePosition, Direction outputDirection)
    {
        GD.PrintS("Requested to create molecule", molecule);

        var targetPosition = sourcePosition + outputDirection.ToVector();

        // TODO: avoid allocation every request
        var item = new Item
        {
            Molecule = molecule,
            TilePosition = targetPosition,
        };

        var created = TryMoveItem(item, targetPosition, outputDirection.Reverse());
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

    public bool TryMoveItem(Item item, Vector2 targetPosition, Direction fromDirection)
    {
        if (EntityTiles.TryGetValue(targetPosition, out var entity)
            && entity.TryConsumeItem(item, targetPosition, fromDirection))
        {
            var overshoot = item.DistanceOvershoot;
            item.TilePosition = targetPosition;
            item.Path = entity.GetItemPath(item.TilePosition);
            item.DistanceOnPath = overshoot;
            return true;
        }

        return false;
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
            EntityType.Merger => new Merger(entityOptions.Position, entityOptions.Direction, entityOptions.Variant + 2),
            _ => null,
        };

        if (entity == null)
        {
            GD.PrintErr("Entity to create not found", entityType);
            return false;
        }

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

    private void UpdateItems(float delta)
    {
        foreach (var entity in Entities)
        {
            foreach (var item in entity.GetItems())
            {
                item?.TravelDistance(delta * Constants.ItemSpeed);
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
}
