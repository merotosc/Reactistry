using ChemFactory.scripts.Items;
using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts.Transports;

public class Belt : IEntity
{
    public Item Item { get; set; }

    public Direction InputDirection { get; set; }

    public Direction OutputDirection { get; set; }

    public int Speed { get; set; } = 3;

    public EntityType Type => EntityType.Belt;

    public Direction Direction => InputDirection.ReverseDirection();

    public Vector2 OutputPosition()
        => OutputDirection.ToVector();

    public Vector2 GetInterpolatedPosition(float t)
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

    public bool TryConsumeItem(Item item, Direction inputDirection)
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

    public static Direction GetOutputDirectionForVariant(Direction direction, int variant)
    {
        return variant switch
        {
            0 => direction,
            1 => direction.NextDirection(),
            2 => direction.PreviousDirection(),
            _ => direction,
        };
    }
}
