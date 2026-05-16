using Godot;

namespace ChemFactory.scripts.Models;

public class ItemPath(params Vector2[] points)
{
    public Vector2[] Points { get; } = points;

    public float Length { get; } = ComputeLength(points);

    private static float ComputeLength(Vector2[] points)
    {
        float length = 0;

        for (var i = 0; i < points.Length - 1; i++)
        {
            length += points[i].DistanceTo(points[i + 1]);
        }

        return length;
    }

    public Vector2 GetPosition(float distance)
    {
        float traveled = 0;

        for (var i = 0; i < Points.Length - 1; i++)
        {
            var a = Points[i];
            var b = Points[i + 1];

            var segment = a.DistanceTo(b);

            if (traveled + segment >= distance)
            {
                var t = (distance - traveled) / segment;
                return a.LinearInterpolate(b, t);
            }

            traveled += segment;
        }

        return Points[^1];
    }
}
