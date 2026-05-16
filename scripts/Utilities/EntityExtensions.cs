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
            EntityType.Reactor => new Vector2(2, Constants.TileSet.Buildings),
            EntityType.Merger => new Vector2(3, Constants.TileSet.Buildings),
            _ => Vector2.Zero,
        };
    }

    public static Vector2 GetSizeForEntity(this EntityType entityType, int variant = 0)
    {
        return entityType switch
        {
            EntityType.Belt => Vector2.One,
            EntityType.Producer => Vector2.One,
            EntityType.Consumer => Vector2.One,
            EntityType.Reactor => new Vector2(1, 2 + variant),
            EntityType.Merger => new Vector2(1, 2 + variant),
            _ => Vector2.One,
        };
    }
}
