using System;
using System.Collections.Generic;
using System.Linq;
using Reactistry.scripts.Models;
using Godot;

namespace Reactistry.scripts.Utilities;

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

    public static Random GetRandom(this Vector2 chunk)
    {
        var seed = (int)chunk.x * 73856093 ^ (int)chunk.y * 19349663;
        return new Random(seed);
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

    public static IEnumerable<Vector2> EnumerateOrthogonalLinePositions(this Vector2 start, Vector2 end)
    {
        var corner = new Vector2(start.x, end.y);
        var line1 = start.EnumerateLinePositions(corner);
        var line2 = corner.EnumerateLinePositions(end);
        return line1.Concat(line2).Distinct().Where(x => x != start && x != end);
    }

    public static IEnumerable<Vector2> EnumerateLinePositions(this Vector2 start, Vector2 end)
    {
        if (start == end)
        {
            yield break;
        }

        if (start.x == end.x)
        {
            var step = Math.Sign(end.y - start.y);
            for (var y = (int)start.y; y != (int)end.y; y += step)
            {
                yield return new Vector2(start.x, y);
            }
        }
        else
        {
            var step = Math.Sign(end.x - start.x);
            for (var x = (int)start.x; x != (int)end.x; x += step)
            {
                yield return new Vector2(x, start.y);
            }
        }

        yield return end;
    }
}
