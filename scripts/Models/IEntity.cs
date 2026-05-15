using ChemFactory.scripts.Items;

namespace ChemFactory.scripts.Models;

public interface IEntity
{
    Direction GetDirection();

    bool TryConsumeItem(Item item, Direction inputDirection);
}