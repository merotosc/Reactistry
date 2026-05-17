using System.Collections.Generic;
using ChemFactory.scripts.Models;
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

    public static Direction ToDirection(this Vector2 vector)
    {
        if (vector.x > 0.5f) return Direction.Right;
        if (vector.x < -0.5f) return Direction.Left;
        if (vector.y > 0.5f) return Direction.Down;
        if (vector.y < -0.5f) return Direction.Up;
        return Direction.Right;
    }

    public static IEnumerable<Vector2> EnumeratePositions(this Vector2 anchorPosition, Direction direction, Vector2 size)
    {
        var x0 = 0;
        var y0 = 0;
        var x1 = (int)size.x;
        var y1 = (int)size.y;

        if (direction is Direction.Down or Direction.Up)
        {
            (x1, y1) = (y1, x1);
        }

        if (direction is Direction.Left or Direction.Down)
        {
            (y0, y1) = (1 - y1, 1 - y0);
        }

        if (direction is Direction.Left or Direction.Up)
        {
            (x0, x1) = (1 - x1, 1 - x0);
        }

        for (var xi = x0; xi < x1; xi++)
        {
            for (var yi = y0; yi < y1; yi++)
            {
                yield return anchorPosition + new Vector2(xi, -yi);
            }
        }
    }
}
