using ChemFactory.scripts.Models;
using Godot;

namespace ChemFactory.scripts.Utilities;

public static class TileMapExtensions
{
    public static void DrawEntity(this TileMap tileMap, EntityType entityType, EntityOptions entityOptions)
    {
        var tileCoord = entityType.GetTileCoordForEntity(entityOptions.Variant);
        var (flipX, flipY, transpose) = entityOptions.Direction.GetTileTransform();

        tileMap.SetCellv(
            entityOptions.Position,
            tile: 0,
            autotileCoord: tileCoord,
            flipX: flipX,
            flipY: flipY,
            transpose: transpose
        );
    }
}
