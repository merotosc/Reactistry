using ChemFactory.scripts.Models;
using Godot;

namespace ChemFactory.scripts.Utilities;

public static class EntityExtensions
{
    public static Vector2 GetTileForEntity(this EntityType building)
    {
        return building switch
        {
            EntityType.Producer => new Vector2(0, 1),
            EntityType.Consumer => new Vector2(1, 1),
            EntityType.Merger => new Vector2(2, 1),
            _ => Vector2.Zero,
        };
    }
}
