using System;
using System.Collections.Generic;
using Reactistry.scripts.Models;
using Reactistry.scripts.Utilities;
using Godot;

namespace Reactistry.scripts.Buildings;

public class Lab(Vector2 anchorPosition, Direction direction)
    : Building(anchorPosition, direction)
{
    private readonly Queue<Item> items = [];
    private Dictionary<Direction, ItemPath> itemPaths;

    public override BuildingType Type => BuildingType.Lab;

    public event Action<Item> ItemDelivered;

    public override void Update(World world, float delta)
    {
        if (items.TryPeek(out var item) && item.PathEndReached)
        {
            world.DeleteItems(item);
            ItemDelivered?.Invoke(item);
            items.Dequeue();
        }
    }

    public override bool TryConsumeItem(Item item, Vector2 targetPosition, Direction fromDirection)
    {
        items.Enqueue(item);
        return true;
    }

    public override IEnumerable<Item> GetItems()
        => [];

    public override ItemPath GetItemPath(Vector2 tilePosition, Direction fromDirection)
    {
        if (itemPaths == null)
        {
            itemPaths = [];

            var right = Vector2.Right / 2;
            var left = -right;
            var down = Vector2.Down / 2;
            var up = -down;

            itemPaths.Add(Direction.Right, new(right, Vector2.Zero));
            itemPaths.Add(Direction.Left, new(left, Vector2.Zero));
            itemPaths.Add(Direction.Down, new(down, Vector2.Zero));
            itemPaths.Add(Direction.Up, new(up, Vector2.Zero));
        }

        return itemPaths[fromDirection];
    }

    public override BuildingInfo GetInfo()
        => new()
        {
            InputItems = items,
        };
}
