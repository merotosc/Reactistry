using Godot;

namespace ChemFactory.scripts;

public static class DirectionExtensions
{
    public static Vector2 ToVector(this Direction direction)
    {
        return direction switch
        {
            Direction.Up => Vector2.Up,
            Direction.Down => Vector2.Down,
            Direction.Left => Vector2.Left,
            Direction.Right => Vector2.Right,
            _ => Vector2.Zero,
        };
    }
}
