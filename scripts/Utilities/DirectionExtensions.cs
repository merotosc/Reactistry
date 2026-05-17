using ChemFactory.scripts.Models;
using Godot;

namespace ChemFactory.scripts.Utilities;

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

    public static Direction Reverse(this Direction direction)
    {
        return direction switch
        {
            Direction.Up => Direction.Down,
            Direction.Down => Direction.Up,
            Direction.Left => Direction.Right,
            Direction.Right => Direction.Left,
            _ => Direction.Right,
        };
    }

    public static Direction Previous(this Direction direction)
    {
        return direction switch
        {
            Direction.Up => Direction.Left,
            Direction.Down => Direction.Right,
            Direction.Left => Direction.Down,
            Direction.Right => Direction.Up,
            _ => Direction.Right,
        };
    }

    public static Direction Next(this Direction direction)
    {
        return direction switch
        {
            Direction.Up => Direction.Right,
            Direction.Down => Direction.Left,
            Direction.Left => Direction.Up,
            Direction.Right => Direction.Down,
            _ => Direction.Right,
        };
    }

    public static (bool flipX, bool flipY, bool transpose) GetTileTransform(this Direction direction)
    {
        return direction switch
        {
            Direction.Up => (false, true, true),
            Direction.Down => (true, false, true),
            Direction.Left => (true, true, false),
            Direction.Right => (false, false, false),
            _ => (false, false, false)
        };
    }
}
