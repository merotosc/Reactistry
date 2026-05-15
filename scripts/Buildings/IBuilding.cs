using ChemFactory.scripts.Models;
using Godot;

namespace ChemFactory.scripts.Buildings;

public interface IBuilding : IEntity
{
    public Vector2 AnchorPosition { get; }

    public Vector2 Size { get; }

    void Update(World world, float delta);
}