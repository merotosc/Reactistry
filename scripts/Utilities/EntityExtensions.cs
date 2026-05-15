using ChemFactory.scripts.Models;
using Godot;

namespace ChemFactory.scripts.Utilities;

public static class EntityExtensions
{
    public static Vector2 GetTileCoordForEntity(this EntityType entityType, int variant = 0)
    {
        return entityType switch
        {
            EntityType.Belt => new Vector2(variant, Constants.TileSet.Belts),
            EntityType.Producer => new Vector2(0, Constants.TileSet.Buildings),
            EntityType.Consumer => new Vector2(1, Constants.TileSet.Buildings),
            EntityType.Merger => new Vector2(2, Constants.TileSet.Buildings),
            EntityType.None or _ => Vector2.Zero,
        };
    }
}
