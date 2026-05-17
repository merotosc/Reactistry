using System.Collections.Generic;
using ChemFactory.scripts.Models;
using Godot;

namespace ChemFactory.scripts.Buildings;

public interface IBuilding
{
    BuildingType Type { get; }

    Vector2 AnchorPosition { get; }

    Direction Direction { get; }

    Vector2 Size { get; }

    int Variant { get; }

    void Update(World world, float delta);

    bool TryConsumeItem(Item item, Vector2 targetPosition, Direction fromDirection);

    IEnumerable<Item> GetItems();

    ItemPath GetItemPath(Vector2 tilePosition);

    string GetInfo();
}