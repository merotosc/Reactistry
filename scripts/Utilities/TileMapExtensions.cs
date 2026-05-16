using ChemFactory.scripts.Models;
using Godot;

namespace ChemFactory.scripts.Utilities;

public static class TileMapExtensions
{
    public static void DrawBuilding(this TileMap tileMap, BuildingType buildingType, BuildingOptions buildingOptions)
    {
        var tileCoord = buildingType.GetTileCoordForBuilding(buildingOptions.Variant);
        var size = buildingType.GetSizeForBuilding(buildingOptions.Variant);
        var (flipX, flipY, transpose) = buildingOptions.Direction.GetTileTransform();

        foreach (var tilePosition in buildingOptions.Position.EnumeratePositions(buildingOptions.Direction, size))
        {
            tileMap.SetCellv(
                tilePosition,
                tile: 0,
                autotileCoord: tileCoord,
                flipX: flipX,
                flipY: flipY,
                transpose: transpose
            );
        }
    }
}
