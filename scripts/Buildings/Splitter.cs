using System.Collections.Generic;
using System.Linq;
using Reactistry.scripts.Models;
using Reactistry.scripts.Utilities;
using Godot;

namespace Reactistry.scripts.Buildings;

public class Splitter(Vector2 anchorPosition, Direction direction, int variant = 0)
    : Building(anchorPosition, direction, variant)
{
    private readonly Direction inputDirection = direction.Reverse();
    private readonly int outputsCount = variant + 2;
    private readonly Item[] items = new Item[variant + 2];
    private ItemPath[] itemPaths;
    private int roundRobin;

    public override BuildingType Type => BuildingType.Splitter;

    public override void Update(World world, float delta)
    {
        for (var i = 0; i < items.Length; i++)
        {
            var item = items[i];
            if (item?.PathEndReached ?? false)
            {
                var itemPosition = AnchorPosition + Direction.Previous().ToVector() * i;
                if (world.TryMoveItem(item, itemPosition + Direction.ToVector(), Direction.Reverse()))
                {
                    items[i] = null;
                }
            }
        }
    }

    public override bool TryConsumeItem(Item item, Vector2 targetPosition, Direction fromDirection)
    {
        if (fromDirection != inputDirection || targetPosition != AnchorPosition)
        {
            return false;
        }

        if (items.All(x => x != null))
        {
            return false;
        }

        while (items[roundRobin] != null)
        {
            roundRobin++;
            roundRobin %= outputsCount;
        }

        items[roundRobin] = item;
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
            var up = Direction.Previous().ToVector();

            for (var i = 0; i < items.Length; i++)
            {
                var topCenter = up * i;
                var topRight = topCenter + right;
                itemPaths[i] = new ItemPath(left, center, topCenter, topRight);
            }
        }

        var path = itemPaths[roundRobin];
        roundRobin++;
        roundRobin %= outputsCount;
        return path;
    }

    public override BuildingInfo GetInfo()
        => new()
        {
            InputItems = items,
        };
}
