using System.Collections.Generic;
using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts.Entities;

public class Producer(Vector2 anchorPosition, Direction direction)
    : Entity(anchorPosition, direction)
{
    private const float ProductionRate = 2;
    private float elapsedTime = 0;
    private Item outputItem;
    private ItemPath itemOutputPath;

    public override EntityType Type => EntityType.Producer;

    public Molecule Molecule { get; set; } = Molecule.InvalidMolecule;

    public override void Update(World world, float delta)
    {
        if (elapsedTime < ProductionRate)
        {
            elapsedTime += delta;
        }

        if (outputItem?.PathEndReached ?? false)
        {
            if (world.TryMoveItem(outputItem, AnchorPosition + Direction.ToVector(), Direction.Reverse()))
            {
                outputItem = null;
            }
        }

        if (elapsedTime >= ProductionRate && outputItem == null)
        {
            outputItem = new Item(Molecule, AnchorPosition, GetItemOutputPath());
            world.AddItems([outputItem]);
            elapsedTime -= ProductionRate;
        }
    }

    public override bool TryConsumeItem(Item item, Vector2 position, Direction inputDirection)
        => false;

    public override IEnumerable<Item> GetItems()
        => outputItem != null
            ? [outputItem]
            : [];

    private ItemPath GetItemOutputPath()
    {
        return itemOutputPath ??= new ItemPath(Vector2.Zero, Direction.ToVector() / 2);
    }
}
