using System;
using Godot;

public class Belt
{
    public Item Item { get; set; }

    public Direction Direction { get; set; }

    public Vector2 GetNextPosition()
    {
        return Direction switch
        {
            Direction.Up => Vector2.Up,
            Direction.Down => Vector2.Down,
            Direction.Left => Vector2.Left,
            Direction.Right => Vector2.Right,
            _ => throw new NotImplementedException($"Direction not recognized"),
        };
    }
}
