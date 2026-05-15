using ChemFactory.scripts.Items;
using Godot;

namespace ChemFactory.scripts.Models;

public interface IEntity
{
    EntityType Type { get; }

    Direction Direction { get; }

    bool TryConsumeItem(Item item, Vector2 position, Direction inputDirection);
}