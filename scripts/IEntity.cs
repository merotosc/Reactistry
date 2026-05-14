namespace ChemFactory.scripts;

public interface IEntity
{
    bool TryConsumeItem(Item item, Direction inputDirection);
}