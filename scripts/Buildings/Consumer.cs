using System.Collections.Generic;
using ChemFactory.scripts.Items;
using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts.Buildings;

public class Consumer : IBuilding
{
    private readonly List<Item> items = [];

    public Direction InputDirection { get; set; }

    public EntityType Type => EntityType.Consumer;

    public Direction Direction => InputDirection.ReverseDirection();

    public Vector2 Size => Type.GetSizeForEntity();

    public void Update(Vector2 position, World world, float delta)
    {
    }

    public bool TryConsumeItem(Item item, Direction inputDirection)
    {
        if (inputDirection != InputDirection)
        {
            return false;
        }

        items.Add(item);
        return true;
    }
}
