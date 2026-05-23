using Godot;

namespace Reactistry.scripts.Models;

public class BuildingOptions
{
    public BuildingType Type { get; set; }

    public Vector2 Position { get; set; }

    public Direction Direction { get; set; } = Direction.Right;

    public int Variant { get; set; } = 0;
}
