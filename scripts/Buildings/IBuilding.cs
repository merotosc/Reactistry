using ChemFactory.scripts.Models;
using Godot;

namespace ChemFactory.scripts.Buildings;

public interface IBuilding : IEntity
{
    void Update(Vector2 position, World world, float delta);
}