using System.Collections.Generic;
using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts.Buildings;

public class Consumer(Vector2 anchorPosition, Direction direction)
    : Building(anchorPosition, direction)
{
    private readonly Direction inputDirection = direction.Reverse();
    private Item item;

    public override BuildingType Type => BuildingType.Consumer;

    public override void Update(World world, float delta)
    {
        if (item?.PathEndReached ?? false)
        {
            world.DeleteItems([item]);
            item = null;
        }
    }

    public override bool TryConsumeItem(Item item, Vector2 targetPosition, Direction fromDirection)
    {
        if (fromDirection != inputDirection)
        {
            return false;
        }

        this.item = item;
        return true;
    }

    public override IEnumerable<Item> GetItems()
        => [];

    public override BuildingInfo GetInfo()
        => new()
        {
            InputItems = [item],
        };
}
