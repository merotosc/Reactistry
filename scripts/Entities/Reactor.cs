using System.Collections.Generic;
using System.Linq;
using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts.Entities;

public class Reactor(Vector2 anchorPosition, Direction direction, int inputsCount = 2)
    : Entity(anchorPosition, direction)
{
    private const float ReactionRate = 1;
    private float elapsedTime = 0;
    private readonly Direction inputsDirection = direction.Reverse();
    private readonly int inputsCount = inputsCount;
    private readonly Item[] inputItems = new Item[inputsCount];
    private Item outputItem;
    private ItemPath itemOutputPath;
    private bool validReaction;
    private List<Molecule> outputMolecules;

    public override EntityType Type => EntityType.Reactor;

    public override Vector2 Size => Type.GetSizeForEntity(inputsCount - 2);

    public override void Update(World world, float delta)
    {
        // TODO: create helper to make timer
        if (elapsedTime < ReactionRate && inputItems.All(x => x != null))
        {
            elapsedTime += delta;
        }

        if (outputItem?.PathEndReached ?? false)
        {
            if (world.TryMoveItem(outputItem, AnchorPosition + Direction.ToVector(), Direction.Reverse()))
            {
                outputItem = null;
                elapsedTime = 0;
            }
        }

        else if (elapsedTime >= ReactionRate && outputItem == null)
        {
            (validReaction, outputMolecules) = ReactionRegistry.CreateReaction([.. inputItems.Select(x => x.Molecule)]);

            // TODO: support multiple output molecules
            var molecule = validReaction
                ? outputMolecules.First()
                : Molecule.InvalidMolecule;

            // TODO: output molecules count as separate molecules
            outputItem = new Item(molecule, AnchorPosition, GetItemOutputPath());
            world.AddItems([outputItem]);
            world.TryDeleteItems(inputItems);
            ClearItems();
            elapsedTime -= ReactionRate;
        }
    }

    public override bool TryConsumeItem(Item item, Vector2 position, Direction inputDirection)
    {
        if (inputDirection != inputsDirection)
        {
            return false;
        }

        var inputNumber = Mathf.RoundToInt(AnchorPosition.DistanceTo(position));
        if (inputItems[inputNumber] == null)
        {
            inputItems[inputNumber] = item;
            return true;
        }

        return false;
    }

    public override IEnumerable<Item> GetItems()
        => [.. inputItems.Append(outputItem).Where(x => x != null)];

    private void ClearItems()
    {
        for (var i = 0; i < inputItems.Length; i++)
        {
            inputItems[i] = null;
        }
    }

    private ItemPath GetItemOutputPath()
    {
        return itemOutputPath ??= new ItemPath(Vector2.Zero, Direction.ToVector() / 2);
    }
}
