using System.Collections.Generic;
using Reactistry.scripts.Models;
using Reactistry.scripts.Utilities;
using Godot;

namespace Reactistry.scripts.Buildings;

public class Extractor(Vector2 anchorPosition, Direction direction, Molecule molecule)
    : Building(anchorPosition, direction)
{
    private const float ProductionRate = 2;
    private readonly CycleTimer timer = new(ProductionRate);
    private readonly Molecule molecule = molecule;
    private ItemPath itemOutputPath;
    private Item outputItem;

    public override BuildingType Type => BuildingType.Extractor;

    public override void Update(World world, float delta)
    {
        timer.Advance(delta);

        if (timer.TryTrigger(() => outputItem == null))
        {
            outputItem = new Item(molecule, AnchorPosition, GetItemOutputPath());
            world.AddItems(outputItem);
        }

        if (outputItem?.PathEndReached ?? false)
        {
            if (world.TryMoveItem(outputItem, AnchorPosition + Direction.ToVector(), Direction.Reverse()))
            {
                outputItem = null;
            }
        }
    }

    public override IEnumerable<Item> GetItems()
        => outputItem != null
            ? [outputItem]
            : [];

    private ItemPath GetItemOutputPath()
    {
        return itemOutputPath ??= new ItemPath(Vector2.Zero, Direction.ToVector() / 2);
    }

    public override BuildingInfo GetInfo()
        => new()
        {
            InputItems = [outputItem],
        };
}
