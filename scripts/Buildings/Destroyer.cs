using System.Collections.Generic;
using Reactistry.scripts.Models;
using Godot;
using Reactistry.scripts.Utilities;

namespace Reactistry.scripts.Buildings;

public class Destroyer(Vector2 anchorPosition, Direction direction)
    : Building(anchorPosition, direction)
{
    private Item item;
    private Dictionary<Direction, ItemPath> itemPaths;

    public override BuildingType Type => BuildingType.Destroyer;

    public override void Update(World world, float delta)
    {
        if (item?.PathEndReached ?? false)
        {
            world.DeleteItems(item);
            item = null;
        }
    }

    public override bool TryConsumeItem(Item item, Vector2 targetPosition, Direction fromDirection)
    {
        if (this.item != null)
        {
            return false;
        }

        this.item = item;
        return true;
    }

    public override IEnumerable<Item> GetItems()
        => item != null ? [item] : [];

    public override ItemPath GetItemPath(Vector2 tilePosition, Direction fromDirection)
    {
        if (itemPaths == null)
        {
            itemPaths = [];

            var right = Direction.ToVector() / 2;
            var left = -right;
            var down = Direction.Next().ToVector() / 2;
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
            InputItems = [item],
        };
}
