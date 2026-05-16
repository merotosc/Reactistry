using System.Collections.Generic;
using ChemFactory.scripts.Items;
using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts.Entities;

public class Belt(Vector2 anchorPosition, Direction direction, BeltVariant variant = BeltVariant.Forward)
    : Entity(anchorPosition, direction)
{
    public override EntityType Type => EntityType.Belt;

    public Item Item { get; set; }

    public Direction InputDirection { get; } = direction.ReverseDirection();

    public Direction OutputDirection { get; } = GetOutputDirectionForVariant(direction, variant);

    public int Speed { get; } = 3;

    public override void Update(World world, float delta)
    {
        Item?.Progress += delta * Speed;
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

        this.Item = item;
        return true;
    }

    public override Vector2 GetInterpolatedPosition(float t)
    {
        var start = InputDirection.ToVector() / 2;
        var end = OutputDirection.ToVector() / 2;

        if (InputDirection == OutputDirection.ReverseDirection())
        {
            return start.LinearInterpolate(end, t);
        }

        return t < 0.5f
            ? start.LinearInterpolate(Vector2.Zero, t * 2)
            : Vector2.Zero.LinearInterpolate(end, (t - 0.5f) * 2);
    }

    private static Direction GetOutputDirectionForVariant(Direction direction, BeltVariant variant)
    {
        return variant switch
        {
            BeltVariant.Forward => direction,
            BeltVariant.Right => direction.NextDirection(),
            BeltVariant.Left => direction.PreviousDirection(),
            _ => direction,
        };
    }

    public override IEnumerable<Item> GetItems()
       => [Item];
}
