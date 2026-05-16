using System.Collections.Generic;
using System.Linq;
using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts.Entities;

public class Merger(Vector2 anchorPosition, Direction direction, int inputsCount = 2)
    : Entity(anchorPosition, direction)
{
    private readonly Direction inputsDirection = direction.Reverse();
    private readonly int inputsCount = inputsCount;
    private readonly Item[] items = new Item[inputsCount];
    private ItemPath[] itemPaths;

    public override EntityType Type => EntityType.Merger;

    public override Vector2 Size => Type.GetSizeForEntity(inputsCount - 2);

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

    public override bool TryConsumeItem(Item item, Vector2 position, Direction inputDirection)
    {
        if (inputDirection != inputsDirection)
        {
            return false;
        }

        var inputNumber = DistanceFromAnchor(position);
        if (items[inputNumber] == null)
        {
            items[inputNumber] = item;
            return true;
        }

        return false;
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
}
