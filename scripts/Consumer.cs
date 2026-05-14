using System.Collections.Generic;
using Godot;

namespace ChemFactory.scripts;

public class Consumer : IBuilding
{
    private readonly List<Item> items = [];

    public Direction InputDirection { get; set; }

    public void Update(Vector2 position, World world, float delta)
    {
    }

    public bool TryConsumeItem(Item item, Direction inputDirection)
    {
        if (inputDirection != InputDirection)
        {
            return false;
        }

        item.Visible = false;
        items.Add(item);
        return true;
    }
}
