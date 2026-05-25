using Reactistry.scripts.Models;
using Godot;
using Reactistry.scripts.Buildings;

namespace Reactistry.scripts.Utilities;

public static class TileMapExtensions
{
    public static void DrawBuilding(this TileMap baseTileMap, BuildingOptions buildingOptions, TileMap overlayTileMap)
    {
        var size = buildingOptions.Type.GetDefinition().GetSize(buildingOptions.Variant);
        var (flipX, flipY, transpose) = buildingOptions.Direction.GetTileTransform();

        foreach (var (tilePosition, localPosition) in buildingOptions.Position.EnumerateAllPositions(buildingOptions.Direction, size))
        {
            var tileCoord = buildingOptions.GetTileCoord(localPosition);
            DrawTile(baseTileMap, Constants.TileSet.BuildingsBaseId);
            DrawTile(overlayTileMap, Constants.TileSet.BuildingsOverlayId);

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

    public static void DrawInvalidPositionWarning(this TileMap baseTileMap, BuildingOptions buildingOptions, TileMap overlayTileMap)
    {
        var tileCoord = new Vector2(1, Constants.TileSet.IconsYOffset);
        var size = buildingOptions.Type.GetDefinition().GetSize(buildingOptions.Variant);

        foreach (var tilePosition in buildingOptions.Position.EnumeratePositions(buildingOptions.Direction, size))
        {
            DrawTile(baseTileMap, Constants.TileSet.IconsId);
            DrawTile(overlayTileMap, Constants.TileSet.IconsId);

            void DrawTile(TileMap tileMap, int tileSetId)
            {
                tileMap.SetCellv(
                    tilePosition,
                    tile: tileSetId,
                    autotileCoord: tileCoord
                );
            }
        }
    }
}
