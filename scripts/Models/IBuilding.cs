using System.Collections.Generic;
using Godot;

namespace ChemFactory.scripts.Models;

public interface IBuilding
{
    BuildingType Type { get; }

    Vector2 AnchorPosition { get; }

    Direction Direction { get; }

    Vector2 Size { get; }

    void Update(World world, float delta);

    bool TryConsumeItem(Item item, Vector2 position, Direction inputDirection);

    IEnumerable<Item> GetItems();

    ItemPath GetItemPath(Vector2 tilePosition);
}