using ChemFactory.scripts;
using Godot;

namespace ChemFactory.scripts.Utilities;

public static class Vector2Extensions
{
    public static Vector2 ToTilePosition(this Vector2 position)
    {
        var scaledPosition = position / Constants.PixelsPerTile;
        return new Vector2(
            Mathf.Floor(scaledPosition.x),
            Mathf.Floor(scaledPosition.y)
        );
    }
}
