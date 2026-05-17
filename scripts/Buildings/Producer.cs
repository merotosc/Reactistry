using System.Collections.Generic;
using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts.Buildings;

public class Producer(Vector2 anchorPosition, Direction direction, Molecule molecule)
    : Building(anchorPosition, direction)
{
    private const float ProductionRate = 2;
    private float elapsedTime = 0;
    private readonly Molecule molecule = molecule;
    private ItemPath itemOutputPath;
    private Item outputItem;

    public override BuildingType Type => BuildingType.Producer;

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
            outputItem = new Item(molecule, AnchorPosition, GetItemOutputPath());
            world.AddItems([outputItem]);
            elapsedTime -= ProductionRate;
        }
    }

    public override bool TryConsumeItem(Item item, Vector2 targetPosition, Direction fromDirection)
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
