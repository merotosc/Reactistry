using System.Collections.Generic;
using System.Linq;
using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts.Buildings;

public class Reactor(Vector2 anchorPosition, Direction direction, int variant = 0) // TODO: use BuildingInputsVariant?
    : Building(anchorPosition, direction, variant)
{
    private const float ReactionRate = 1;
    private float elapsedTime = 0;
    private readonly Direction inputsDirection = direction.Reverse();
    private readonly int inputsCount = variant + 2;
    private readonly Item[] inputItems = new Item[variant + 2];
    private readonly Queue<Item> outputItems = [];
    private ItemPath itemOutputPath;
    private bool validReaction;
    private List<Molecule> outputMolecules;

    public override BuildingType Type => BuildingType.Reactor;

    public override void Update(World world, float delta)
    {
        // TODO: create helper to make timer
        if (elapsedTime < ReactionRate && inputItems.All(x => x != null))
        {
            elapsedTime += delta;
        }

        if (outputItems.TryPeek(out var item) && item.PathEndReached)
        {
            if (world.TryMoveItem(outputItems.Peek(), AnchorPosition + Direction.ToVector(), Direction.Reverse()))
            {
                outputItems.Dequeue();
                elapsedTime = 0;
            }
        }

        else if (elapsedTime >= ReactionRate && outputItems.Count == 0)
        {
            (validReaction, outputMolecules) = ReactionRegistry.CreateReaction([.. inputItems.Select(x => x.Molecule)]);

            // TODO: support multiple output molecules
            var molecule = validReaction
                ? outputMolecules.First()
                : Molecule.InvalidMolecule;

            for (var i = 0; i < molecule.Count; i++)
            {
                outputItems.Enqueue(new Item(molecule, AnchorPosition, GetItemOutputPath()));
            }

            world.AddItems(outputItems);
            world.TryDeleteItems(inputItems);
            ClearItems();
            elapsedTime -= ReactionRate;
        }
    }

    public override bool TryConsumeItem(Item item, Vector2 targetPosition, Direction fromDirection)
    {
        if (fromDirection != inputsDirection)
        {
            return false;
        }

        var inputNumber = Mathf.RoundToInt(AnchorPosition.DistanceTo(targetPosition));
        if (inputItems[inputNumber] == null)
        {
            inputItems[inputNumber] = item;
            return true;
        }

        return false;
    }

    public override IEnumerable<Item> GetItems()
        => [.. inputItems.Concat(outputItems).Where(x => x != null)];

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

    public override BuildingInfo GetInfo()
        => new()
        {
            InputItems = inputItems,
            OutputItems = outputItems,
        };
}
