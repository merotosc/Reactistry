using Godot;

namespace ChemFactory.scripts.Models;

public class Item
{
    public Molecule Molecule { get; set; }

    public Vector2 TilePosition { get; set; }

    public ItemPath Path { get; set; }

    public float DistanceOnPath { get; set; }

    public bool PathEndReached => DistanceOnPath >= Path?.Length;

    public float DistanceOvershoot => DistanceOnPath - Path?.Length ?? 0;

    public void TravelDistance(float distance)
    {
        if (PathEndReached)
        {
            return;
        }

        DistanceOnPath += distance;
    }

    public Vector2 GetPositionOnPath()
        => Path.GetPosition(DistanceOnPath);
}
