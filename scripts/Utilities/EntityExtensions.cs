using ChemFactory.scripts.Models;
using Godot;

namespace ChemFactory.scripts.Utilities;

public static class EntityExtensions
{
    public static Vector2 GetTileCoordForEntity(this EntityType entityType)
    {
        return entityType switch
        {
            EntityType.Belt => new Vector2(0, 0),
            EntityType.Producer => new Vector2(0, 1),
            EntityType.Consumer => new Vector2(1, 1),
            EntityType.Merger => new Vector2(2, 1),
            EntityType.None or _ => Vector2.Zero,
        };
    }
}
