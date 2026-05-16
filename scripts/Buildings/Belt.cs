using System.Collections.Generic;
using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts.Buildings;

public class Belt(Vector2 anchorPosition, Direction direction, BeltVariant variant = BeltVariant.Forward)
    : Building(anchorPosition, direction)
{
    private readonly Direction inputDirection = direction.Reverse();
    private readonly Direction outputDirection = GetOutputDirectionForVariant(direction, variant);
    private ItemPath[] itemPaths;
    private Item Item;

    public override BuildingType Type => BuildingType.Belt;

    public override void Update(World world, float delta)
    {
        if (Item?.PathEndReached ?? false)
        {
            var moved = world.TryMoveItem(Item, AnchorPosition + outputDirection.ToVector(), outputDirection.Reverse());
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

        if (inputDirection != this.inputDirection)
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
