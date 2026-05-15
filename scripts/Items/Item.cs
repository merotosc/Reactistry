using Godot;

namespace ChemFactory.scripts.Items;

public class Item
{
    public ItemType Type { get; set; }

    public Vector2 TilePosition { get; set; }

    public float Progress { get; set; }
}
