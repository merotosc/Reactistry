using ChemFactory.scripts.Items;
using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts.Buildings;

public abstract class Building(Vector2 anchorPosition, Direction direction) : IBuilding
{
    public abstract EntityType Type { get; }

    public Vector2 AnchorPosition { get; } = anchorPosition;

    public Direction Direction { get; } = direction;

    public virtual Vector2 Size { get => Type.GetSizeForEntity(); }

    public abstract bool TryConsumeItem(Item item, Vector2 position, Direction inputDirection);

    public virtual void Update(World world, float delta)
    {
    }
}
