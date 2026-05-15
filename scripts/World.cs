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

    public Dictionary<Vector2, IBuilding> Buildings { get; set; } = [];

    public List<Item> Items { get; set; } = [];

    public event Action<IEntity, Vector2> EntityCreated;

    public World()
    {
        //Belts.Add(new Vector2(0, 0), new Belt { InputDirection = Direction.Left, OutputDirection = Direction.Right });
        //Belts.Add(new Vector2(1, 0), new Belt { InputDirection = Direction.Left, OutputDirection = Direction.Right });
        //Belts.Add(new Vector2(2, 0), new Belt { InputDirection = Direction.Left, OutputDirection = Direction.Right });
        //Belts.Add(new Vector2(3, 0), new Belt { InputDirection = Direction.Left, OutputDirection = Direction.Right });
        //Belts.Add(new Vector2(4, 0), new Belt { InputDirection = Direction.Left, OutputDirection = Direction.Right });

        //Belts.Add(new Vector2(5, -3), new Belt { InputDirection = Direction.Up, OutputDirection = Direction.Down });
        //Belts.Add(new Vector2(5, -2), new Belt { InputDirection = Direction.Up, OutputDirection = Direction.Down });
        //Belts.Add(new Vector2(5, -1), new Belt { InputDirection = Direction.Up, OutputDirection = Direction.Down });

        //Buildings.Add(new Vector2(-1, 0), new Producer { ItemType = ItemType.O, OutputDirection = Direction.Right });
        //Buildings.Add(new Vector2(5, -4), new Producer { ItemType = ItemType.H, OutputDirection = Direction.Down });
        //Buildings.Add(new Vector2(5, 0), new Consumer { InputDirection = Direction.Left });

        Belts.Add(new Vector2(0, -3), new Belt { InputDirection = Direction.Up, OutputDirection = Direction.Down });
        Belts.Add(new Vector2(0, -2), new Belt { InputDirection = Direction.Up, OutputDirection = Direction.Down });
        Belts.Add(new Vector2(0, -1), new Belt { InputDirection = Direction.Up, OutputDirection = Direction.Down });

        Belts.Add(new Vector2(0, 3), new Belt { InputDirection = Direction.Down, OutputDirection = Direction.Up });
        Belts.Add(new Vector2(0, 2), new Belt { InputDirection = Direction.Down, OutputDirection = Direction.Up });
        Belts.Add(new Vector2(0, 1), new Belt { InputDirection = Direction.Down, OutputDirection = Direction.Up });

        Belts.Add(new Vector2(1, 0), new Belt { InputDirection = Direction.Left, OutputDirection = Direction.Right });
        Belts.Add(new Vector2(2, 0), new Belt { InputDirection = Direction.Left, OutputDirection = Direction.Right });
        Belts.Add(new Vector2(3, 0), new Belt { InputDirection = Direction.Left, OutputDirection = Direction.Right });
        Belts.Add(new Vector2(4, 0), new Belt { InputDirection = Direction.Left, OutputDirection = Direction.Right });
        Belts.Add(new Vector2(5, 0), new Belt { InputDirection = Direction.Left, OutputDirection = Direction.Right });

        Buildings.Add(new Vector2(0, 4), new Producer { ItemType = ItemType.O, OutputDirection = Direction.Up });
        Buildings.Add(new Vector2(0, -4), new Producer { ItemType = ItemType.H, OutputDirection = Direction.Down });
        Buildings.Add(new Vector2(0, 0), new Merger { OutputDirection = Direction.Right });
    }

    public void Tick(float delta)
    {
        UpdateBeltItems(delta);
        UpdateBuildings(delta);
    }

    public bool CreateItem(ItemType itemType, Vector2 outputPosition, Direction outputDirection)
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

    public bool TryCreateEntity(EntityType entityType, Vector2 position)
    {
        if (Belts.ContainsKey(position) || Buildings.ContainsKey(position))
        {
            return false;
        }

        if (entityType == EntityType.Belt)
        {
            var belt = new Belt { InputDirection = Direction.Left, OutputDirection = Direction.Right };
            Belts.Add(position, belt);
            EntityCreated?.Invoke(belt, position);
        }
        else
        {
            // TODO: create based on entity type
            var producer = new Producer { OutputDirection = Direction.Right };
            Buildings.Add(position, producer);
            EntityCreated?.Invoke(producer, position);
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
            if (belt.Item.Progress >= 1)
            {
                TryMoveToNextTile(position, belt);
            }
            else
            {
                MoveOnBelt(position, belt, delta);
            }
        }
    }

    private void UpdateBuildings(float delta)
    {
        foreach (var (position, building) in Buildings)
        {
            building.Update(position, this, delta);
        }
    }

    private void TryMoveToNextTile(Vector2 position, Belt belt)
    {
        var tilePosition = position + belt.OutputPosition();
        var moved = TryMoveItemToEntity(belt.Item, tilePosition, belt.OutputDirection);
        if (moved)
        {
            belt.Item = null;
        }
    }

    private bool TryMoveItemToEntity(Item item, Vector2 position, Direction outputDirection)
    {
        if (TryMoveItemToEntity(item, position, outputDirection, Belts))
        {
            item.Progress %= 1f;
            item.TilePosition = position;
            return true;
        }

        if (TryMoveItemToEntity(item, position, outputDirection, Buildings))
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
        return entities.TryGetValue(position, out var entity) && entity.TryConsumeItem(item, outputDirection.ReverseDirection());
    }

    private static void MoveOnBelt(Vector2 position, Belt belt, float delta)
    {
        belt.Item.Progress += delta * belt.Speed;
    }
}
