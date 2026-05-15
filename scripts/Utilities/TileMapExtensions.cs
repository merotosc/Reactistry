using ChemFactory.scripts.Models;
using Godot;

namespace ChemFactory.scripts.Utilities;

public static class TileMapExtensions
{
    public static void DrawEntity(this TileMap tileMap, EntityType entityType, EntityOptions entityOptions)
    {
        var tileCoord = entityType.GetTileCoordForEntity(entityOptions.Variant);
        var size = entityType.GetSizeForEntity(entityOptions.Variant);
        var (flipX, flipY, transpose) = entityOptions.Direction.GetTileTransform();

        foreach (var tilePosition in entityOptions.Position.EnumeratePositions(entityOptions.Direction, size))
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
