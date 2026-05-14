using Godot;

namespace ChemFactory.scripts;

public interface IBuilding : IEntity
{
    Direction GetDirection();

    void Update(Vector2 position, World world, float delta);
}