using System.Collections.Generic;
using System.Linq;
using Reactistry.scripts.Models;
using Reactistry.scripts.Utilities;
using Godot;

namespace Reactistry.scripts.Buildings;

public class Merger(Vector2 anchorPosition, Direction direction, int variant = 0)
    : Building(anchorPosition, direction, variant)
{
    private readonly Direction inputsDirection = direction.Reverse();
    private readonly int inputsCount = variant + 2;
    private readonly Item[] items = new Item[variant + 2];
    private ItemPath[] itemPaths;

    public override BuildingType Type => BuildingType.Merger;

    public override void Update(World world, float delta)
    {
        for (var i = 0; i < items.Length; i++)
        {
            var item = items[i];
            if (item?.PathEndReached ?? false)
            {
                if (world.TryMoveItem(item, AnchorPosition + Direction.ToVector(), Direction.Reverse()))
                {
                    items[i] = null;
                }
            }
        }
    }

    public override bool TryConsumeItem(Item item, Vector2 targetPosition, Direction fromDirection)
    {
        if (fromDirection != inputsDirection)
        {
            return false;
        }

        var inputNumber = DistanceFromAnchor(targetPosition);
        if (items[inputNumber] != null)
        {
            return false;
        }

        items[inputNumber] = item;
        return true;
    }

    public override IEnumerable<Item> GetItems()
        => [.. items.Where(x => x != null)];

    public override ItemPath GetItemPath(Vector2 tilePosition, Direction fromDirection)
    {
        if (itemPaths == null)
        {
            itemPaths = new ItemPath[items.Length];

            var right = Direction.ToVector() / 2;
            var left = -right;
            var center = Vector2.Zero;
            var down = Direction.Next().ToVector();

            for (var i = 0; i < items.Length; i++)
            {
                var bottomCenter = down * i;
                var bottomRight = bottomCenter + right;
                itemPaths[i] = new ItemPath(left, center, bottomCenter, bottomRight);
            }
        }

        var inputNumber = DistanceFromAnchor(tilePosition);
        return itemPaths[inputNumber];
    }

    public override BuildingInfo GetInfo()
        => new()
        {
            InputItems = items,
        };
}
