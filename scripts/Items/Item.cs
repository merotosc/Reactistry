using ChemFactory.scripts.Models;
using Godot;

namespace ChemFactory.scripts.Items;

public class Item
{
    public Molecule Molecule { get; set; }

    public Vector2 TilePosition { get; set; }

    public float Progress { get; set; }
}
