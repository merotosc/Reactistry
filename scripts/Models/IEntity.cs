using ChemFactory.scripts.Items;

namespace ChemFactory.scripts.Models;

public interface IEntity
{
    EntityType Type { get; }

    Direction Direction { get; }

    bool TryConsumeItem(Item item, Direction inputDirection);
}