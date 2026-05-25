using System;
using Godot;

namespace Reactistry.scripts.Models;

public class BuildingDefinition
{
    public Vector2 BaseTileCoord { get; set; }

    public TileMode TileMode { get; set; } = TileMode.Repeated;

    public Vector2 DefaultSize { get; set; } = Vector2.One;

    public Func<int, Vector2> GetSize { get; set; }

    public int VariantsCount { get; set; } = 1;

    public BuildingDefinition()
    {
        GetSize = _ => DefaultSize;
    }
}
