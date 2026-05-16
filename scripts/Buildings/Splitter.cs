using System.Collections.Generic;
using System.Linq;
using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts.Buildings;

public class Splitter(Vector2 anchorPosition, Direction direction, int outputsCount = 2)
    : Building(anchorPosition, direction)
{
    private readonly Direction inputDirection = direction.Reverse();
    private readonly int outputsCount = outputsCount;
    private readonly Item[] items = new Item[outputsCount];
    private ItemPath[] itemPaths;
    private int roundRobin;

    public override BuildingType Type => BuildingType.Splitter;

    public override Vector2 Size => Type.GetSizeForBuilding(outputsCount - 2);

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

    public override bool TryConsumeItem(Item item, Vector2 position, Direction inputDirection)
    {
        if (inputDirection != this.inputDirection || position != AnchorPosition)
        {
            return false;
        }

        if (items[roundRobin] != null)
        {
            return false;
        }

        items[roundRobin] = item;

        return true;
    }

    public override IEnumerable<Item> GetItems()
        => [.. items.Where(x => x != null)];

    public override ItemPath GetItemPath(Vector2 tilePosition)
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
}
