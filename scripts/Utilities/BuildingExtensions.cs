using Reactistry.scripts.Buildings;
using Reactistry.scripts.Models;
using Godot;
using System.Collections.Generic;

namespace Reactistry.scripts.Utilities;

public static class BuildingExtensions
{
    public static BuildingOptions ToBuildingOptions(this IBuilding building)
    {
        return new BuildingOptions
        {
            Type = building.Type,
            Position = building.AnchorPosition,
            Direction = building.Direction,
            Variant = building.Variant,
        };
    }

    public static BuildingDefinition GetDefinition(this BuildingType buildingType)
        => BuildingsConstants.Definitions.GetValueOrDefault(buildingType, new());

    public static Vector2 GetTileCoord(this BuildingOptions buildingOptions, Vector2 localPosition)
    {
        var buildingDefinition = buildingOptions.Type.GetDefinition();
        var baseCoord = buildingDefinition.BaseTileCoord;
        var tileMode = buildingDefinition.TileMode;

        switch (tileMode)
        {
            case TileMode.Repeated:
                return baseCoord;
            case TileMode.Variant:
                return baseCoord + new Vector2(0, buildingOptions.Variant);
            case TileMode.Autotile1D:
                {
                    var size = buildingDefinition.GetSize(buildingOptions.Variant);
                    var segment = localPosition.GetSegment(size);

                    var localCoord = segment switch
                    {
                        TileSegment.Single or
                        TileSegment.Start => new Vector2(0, 2),
                        TileSegment.Middle => new Vector2(0, 1),
                        TileSegment.End => new Vector2(0, 0),
                        _ => new Vector2(0, 0),
                    };

                    return baseCoord + localCoord;
                }
            case TileMode.Autotile2D:
                // TODO: implement correct logic for Autotile 2D
                return baseCoord;
            default:
                return baseCoord;
        }
    }

    public static Vector2 GetDefaultTileCoord(this BuildingType buildingType)
    {
        var buildingDefinition = buildingType.GetDefinition();
        var baseCoord = buildingDefinition.BaseTileCoord;
        var tileMode = buildingDefinition.TileMode;

        return tileMode switch
        {
            TileMode.Repeated or
            TileMode.Variant => baseCoord,
            TileMode.Autotile1D or
            TileMode.Autotile2D => baseCoord + new Vector2(0, 2),
            _ => baseCoord,
        };
    }
}
