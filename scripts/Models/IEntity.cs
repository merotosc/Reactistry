using System.Collections.Generic;
using ChemFactory.scripts.Items;
using Godot;

namespace ChemFactory.scripts.Models;

public interface IEntity
{
    EntityType Type { get; }

    Vector2 AnchorPosition { get; }

    Direction Direction { get; }

    Vector2 Size { get; }

    void Update(World world, float delta);

    bool TryConsumeItem(Item item, Vector2 position, Direction inputDirection);

    Vector2 GetInterpolatedPosition(float progress);

    IEnumerable<Item> GetItems();
}