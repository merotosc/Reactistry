using System.Collections.Generic;
using System.Linq;
using Reactistry.scripts.Models;
using Reactistry.scripts.Utilities;
using Godot;

namespace Reactistry.scripts.Buildings;

public class Reactor(Vector2 anchorPosition, Direction direction, int variant = 0) // TODO: use BuildingInputsVariant?
    : Building(anchorPosition, direction, variant)
{
    private const float ReactionRate = 1;
    private readonly CycleTimer timer = new(ReactionRate);
    private readonly Direction inputsDirection = direction.Reverse();
    private readonly int inputsCount = variant + 2;
    private readonly Item[] inputItems = new Item[variant + 2];
    private readonly Queue<Item>[] outputItems = [.. Enumerable.Range(0, variant + 2).Select(_ => new Queue<Item>())];
    private ItemPath itemOutputPath;
    private bool validReaction;
    private List<Molecule> outputMolecules;

    public override BuildingType Type => BuildingType.Reactor;

    public override void Update(World world, float delta)
    {
        if (inputItems.All(x => x != null))
        {
            timer.Advance(delta);
        }

        for (var i = 0; i < outputItems.Length; i++)
        {
            var items = outputItems[i];
            if (items.TryPeek(out var item) && item.PathEndReached)
            {
                var nextTilePosition = AnchorPosition + Direction.Previous().ToVector() * i + Direction.ToVector();
                if (world.TryMoveItem(items.Peek(), nextTilePosition, Direction.Reverse()))
                {
                    items.Dequeue();
                }
            }
        }

        if (timer.TryTrigger(() => outputItems.All(x => x.Count == 0)))
        {
            (validReaction, outputMolecules) = ReactionRegistry.CreateReaction([.. inputItems.Select(x => x.Molecule)]);

            var molecules = validReaction
                ? outputMolecules
                : [Molecule.InvalidMolecule];

            for (var differentMoleculesCount = 0; differentMoleculesCount < molecules.Count; differentMoleculesCount++)
            {
                var molecule = molecules[differentMoleculesCount];
                for (var moleculesCount = 0; moleculesCount < molecule.Count; moleculesCount++)
                {
                    var tilePosition = AnchorPosition + Direction.Previous().ToVector() * differentMoleculesCount;
                    outputItems[differentMoleculesCount].Enqueue(new Item(new(molecule.Elements), tilePosition, GetItemOutputPath()));
                }
            }

            world.AddItems(outputItems.SelectMany(x => x));
            world.DeleteItems(inputItems);
            ClearItems();
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
        => [.. inputItems
            .Concat(outputItems.SelectMany(x => x))
            .Where(x => x != null)];

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
            OutputItems = outputItems.SelectMany(x => x),
        };
}
