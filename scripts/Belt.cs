using ChemFactory.scripts;
using Godot;

public class Belt
{
    public Item Item { get; set; }

    public Direction InputDirection { get; set; } = Direction.Left;

    public Direction OutputDirection { get; set; }

    public int Speed { get; set; } = 3;

    public Vector2 GetNextPosition()
        => OutputDirection.ToVector();

    public Vector2 GetInterpolatedPosition(float t)
    {
        var start = InputDirection.ToVector() / 2;
        var end = OutputDirection.ToVector() / 2;
        return start.LinearInterpolate(end, t);
    }
}
