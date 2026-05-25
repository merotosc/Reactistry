using System;
using System.Collections.Generic;
using Godot;
using Reactistry.scripts.Models;

namespace Reactistry.scripts.Buildings;

public static class BuildingsConstants
{
    public static readonly Dictionary<BuildingType, BuildingDefinition> Definitions = new()
    {
        [BuildingType.Lab] = new()
        {
            BaseTileCoord = new Vector2(6, 0),
            TileMode = TileMode.Autotile2D,
            DefaultSize = new Vector2(5, 5),
        },
        [BuildingType.Pipe] = new()
        {
            BaseTileCoord = new Vector2(0, 0),
            TileMode = TileMode.Variant,
            VariantsCount = 3,
        },
        [BuildingType.Extractor] = new()
        {
            BaseTileCoord = new Vector2(1, 0),
        },
        [BuildingType.Destroyer] = new()
        {
            BaseTileCoord = new Vector2(2, 0),
        },
        [BuildingType.Reactor] = new()
        {
            BaseTileCoord = new Vector2(3, 0),
            TileMode = TileMode.Autotile1D,
            DefaultSize = new Vector2(1, 2),
            GetSize = variant => new Vector2(1, 2 + variant),
            VariantsCount = Enum.GetNames(typeof(BuildingInputsVariant)).Length,
        },
        [BuildingType.Splitter] = new()
        {
            BaseTileCoord = new Vector2(4, 0),
            TileMode = TileMode.Autotile1D,
            DefaultSize = new Vector2(1, 2),
            GetSize = variant => new Vector2(1, 2 + variant),
            VariantsCount = Enum.GetNames(typeof(BuildingInputsVariant)).Length,
        },
        [BuildingType.Merger] = new()
        {
            BaseTileCoord = new Vector2(5, 0),
            TileMode = TileMode.Autotile1D,
            DefaultSize = new Vector2(1, 2),
            GetSize = variant => new Vector2(1, 2 + variant),
            VariantsCount = Enum.GetNames(typeof(BuildingInputsVariant)).Length,
        },
    };
}
