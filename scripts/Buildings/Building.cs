using System.Collections.Generic;
using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts.Buildings;

public abstract class Building(Vector2 anchorPosition, Direction direction, int variant = 0) : IBuilding
{
    private ItemPath itemPath;

    public abstract BuildingType Type { get; }

    public Vector2 AnchorPosition { get; } = anchorPosition;

    public Direction Direction { get; } = direction;

    public Vector2 Size { get => Type.GetSizeForBuilding(Variant); }

    public int Variant { get; } = variant;

    public virtual void Update(World world, float delta)
    {
    }

    public virtual bool TryConsumeItem(Item item, Vector2 targetPosition, Direction fromDirection)
        => false;

    public abstract IEnumerable<Item> GetItems();

    public virtual ItemPath GetItemPath(Vector2 tilePosition)
    {
        return itemPath ??= new ItemPath(Direction.Reverse().ToVector() / 2, Vector2.Zero);
    }

    public abstract BuildingInfo GetInfo();

    protected int DistanceFromAnchor(Vector2 position)
        => Mathf.RoundToInt(AnchorPosition.DistanceTo(position));

    public override string ToString()
        => string.Join('\n', Type, AnchorPosition, Direction, Size, Variant);
}
