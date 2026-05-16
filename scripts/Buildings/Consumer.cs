using System.Collections.Generic;
using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts.Buildings;

public class Consumer(Vector2 anchorPosition, Direction direction)
    : Building(anchorPosition, direction)
{
    private readonly List<Item> items = [];
    private readonly Direction inputDirection = direction.Reverse();

    public override BuildingType Type => BuildingType.Consumer;

    public override bool TryConsumeItem(Item item, Vector2 position, Direction inputDirection)
    {
        if (inputDirection != this.inputDirection)
        {
            return false;
        }

        items.Add(item);
        return true;
    }

    public override IEnumerable<Item> GetItems()
        => [];
}
