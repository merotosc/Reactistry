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

    public Direction GetDirection()
        => InputDirection.ReverseDirection();

    public Vector2 OutputPosition()
        => OutputDirection.ToVector();

    public Vector2 GetInterpolatedPosition(float t)
    {
        var start = InputDirection.ToVector() / 2;
        var end = OutputDirection.ToVector() / 2;
        return start.LinearInterpolate(end, t);
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
}
