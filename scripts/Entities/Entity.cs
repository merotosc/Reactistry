using System.Collections.Generic;
using ChemFactory.scripts.Items;
using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts.Entities;

public abstract class Entity(Vector2 anchorPosition, Direction direction) : IEntity
{
    public abstract EntityType Type { get; }

    public Vector2 AnchorPosition { get; } = anchorPosition;

    public Direction Direction { get; } = direction;

    public virtual Vector2 Size { get => Type.GetSizeForEntity(); }

    public virtual void Update(World world, float delta)
    {
    }

    public abstract bool TryConsumeItem(Item item, Vector2 position, Direction inputDirection);

    public abstract Vector2 GetInterpolatedPosition(float progress);

    public abstract IEnumerable<Item> GetItems();
}
