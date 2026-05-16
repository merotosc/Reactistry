using Godot;

namespace ChemFactory.scripts.Models;

public class Item(Molecule molecule, Vector2 tilePosition, ItemPath path)
{
    public Molecule Molecule { get; } = molecule;

    public Vector2 TilePosition { get; set; } = tilePosition;

    public ItemPath Path { get; set; } = path;

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
