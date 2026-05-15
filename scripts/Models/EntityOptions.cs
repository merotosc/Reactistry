using Godot;

namespace ChemFactory.scripts.Models;

public class EntityOptions
{
    public Vector2 Position { get; set; }

    public Direction Direction { get; set; } = Direction.Right;

    public int Variant { get; set; } = 0;

    public int Size { get; set; } = 1;
}
