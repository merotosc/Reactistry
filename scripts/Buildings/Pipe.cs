using System.Collections.Generic;
using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts.Buildings;

public class Pipe(Vector2 anchorPosition, Direction direction, PipeVariant variant = PipeVariant.Forward)
    : Building(anchorPosition, direction, (int)variant)
{
    private readonly Direction inputDirection = direction.Reverse();
    private readonly Direction outputDirection = GetOutputDirectionForVariant(direction, variant);
    private ItemPath[] itemPaths;
    private Item item;

    public override BuildingType Type => BuildingType.Pipe;

    public override void Update(World world, float delta)
    {
        if (item?.PathEndReached ?? false)
        {
            var moved = world.TryMoveItem(item, AnchorPosition + outputDirection.ToVector(), outputDirection.Reverse());
            if (moved)
            {
                item = null;
            }
        }
    }

    public override bool TryConsumeItem(Item item, Vector2 targetPosition, Direction fromDirection)
    {
        if (this.item != null)
        {
            return false;
        }

        if (fromDirection != inputDirection)
        {
            return false;
        }

        this.item = item;
        return true;
    }

    private static Direction GetOutputDirectionForVariant(Direction direction, PipeVariant variant)
    {
        return variant switch
        {
            PipeVariant.Forward => direction,
            PipeVariant.Right => direction.Next(),
            PipeVariant.Left => direction.Previous(),
            _ => direction,
        };
    }

    public override IEnumerable<Item> GetItems()
       => item != null ? [item] : [];

    public override ItemPath GetItemPath(Vector2 tilePosition, Direction fromDirection)
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

    public override BuildingInfo GetInfo()
        => new()
        {
            InputItems = [item],
        };
}
