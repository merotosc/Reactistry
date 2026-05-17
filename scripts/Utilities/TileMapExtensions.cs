using ChemFactory.scripts.Models;
using Godot;

namespace ChemFactory.scripts.Utilities;

public static class TileMapExtensions
{
    public static void DrawBuilding(this TileMap baseTileMap, BuildingOptions buildingOptions, TileMap overlayTileMap)
    {
        var tileCoord = buildingOptions.Type.GetTileCoordForBuilding(buildingOptions.Variant);
        var size = buildingOptions.Type.GetSizeForBuilding(buildingOptions.Variant);
        var (flipX, flipY, transpose) = buildingOptions.Direction.GetTileTransform();

        foreach (var tilePosition in buildingOptions.Position.EnumeratePositions(buildingOptions.Direction, size))
        {
            DrawTile(baseTileMap, Constants.TileSet.BaseId);
            DrawTile(overlayTileMap, Constants.TileSet.OverlayId);

            void DrawTile(TileMap tileMap, int tileSetId)
            {
                tileMap.SetCellv(
                    tilePosition,
                    tile: tileSetId,
                    autotileCoord: tileCoord,
                    flipX: flipX,
                    flipY: flipY,
                    transpose: transpose
                );
            }
        }
    }
}
