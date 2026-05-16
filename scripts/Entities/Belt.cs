using System.Collections.Generic;
using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts.Entities;

public class Belt(Vector2 anchorPosition, Direction direction, BeltVariant variant = BeltVariant.Forward)
    : Entity(anchorPosition, direction)
{
    private ItemPath[] itemPaths;

    public override EntityType Type => EntityType.Belt;

    public Item Item { get; set; }

    public Direction InputDirection { get; } = direction.Reverse();

    public Direction OutputDirection { get; } = GetOutputDirectionForVariant(direction, variant);

    public override void Update(World world, float delta)
    {
        if (Item?.PathEndReached ?? false)
        {
            var moved = world.TryMoveItem(Item, AnchorPosition + OutputDirection.ToVector(), OutputDirection.Reverse());
            if (moved)
            {
                Item = null;
            }
        }
    }

    public override bool TryConsumeItem(Item item, Vector2 position, Direction inputDirection)
    {
        if (Item != null)
        {
            return false;
        }

        if (inputDirection != InputDirection)
        {
            return false;
        }

        Item = item;
        return true;
    }

    private static Direction GetOutputDirectionForVariant(Direction direction, BeltVariant variant)
    {
        return variant switch
        {
            BeltVariant.Forward => direction,
            BeltVariant.Right => direction.Next(),
            BeltVariant.Left => direction.Previous(),
            _ => direction,
        };
    }

    public override IEnumerable<Item> GetItems()
       => [Item];

    public override ItemPath GetItemPath(Vector2 tilePosition)
    {
        if (itemPaths == null)
        {
            itemPaths = new ItemPath[3];

            var right = Direction.ToVector() / 2;
            var left = -right;
            var down = Direction.Next().ToVector() / 2;
            var up = -down;

            itemPaths[0] = new ItemPath(left, right);
            itemPaths[1] = new ItemPath(left, Vector2.Zero, down);
            itemPaths[2] = new ItemPath(left, Vector2.Zero, up);
        }

        return itemPaths[(int)variant];
    }
}
