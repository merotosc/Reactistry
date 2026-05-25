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
        var scaledPosition = position / Constants.Map.TileSize;
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
        foreach (var (position, _) in anchorPosition.EnumerateAllPositions(direction, size))
        {
            yield return position;
        }
    }

    public static IEnumerable<(Vector2 Position, Vector2 LocalPosition)> EnumerateAllPositions(this Vector2 anchorPosition, Direction direction, Vector2 size)
    {
        var width = (int)size.x;
        var height = (int)size.y;

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var localPosition = new Vector2(x, y);
                var position = anchorPosition + localPosition.RotateOffset(direction);

                yield return (position, localPosition);
            }
        }
    }

    private static Vector2 RotateOffset(this Vector2 position, Direction direction)
    {
        var x = (int)position.x;
        var y = (int)position.y;

        return direction switch
        {
            Direction.Right => new Vector2(x, -y),
            Direction.Down => new Vector2(y, x),
            Direction.Left => new Vector2(-x, y),
            Direction.Up => new Vector2(-y, -x),
            _ => Vector2.Zero,
        };
    }

    public static TileSegment GetSegment(this Vector2 position, Vector2 size)
    {
        if (size.y == 1)
        {
            return TileSegment.Single;
        }
        else if (position.y == 0)
        {
            return TileSegment.Start;
        }
        else if (position.y == size.y - 1)
        {
            return TileSegment.End;
        }

        return TileSegment.Middle;
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
