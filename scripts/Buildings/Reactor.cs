using System.Collections.Generic;
using System.Linq;
using ChemFactory.scripts.Items;
using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts.Buildings;

public class Reactor(Vector2 anchorPosition, Direction direction, int inputsCount = 2)
    : Building(anchorPosition, direction)
{
    private const float ReactionRate = 1;
    private float elapsedTime = 0;
    private readonly Direction inputsDirection = direction.ReverseDirection();
    private readonly Item[] items = new Item[inputsCount];
    private readonly int inputsCount = inputsCount;
    private bool validReaction;
    private bool outputReady;
    private List<Molecule> outputMolecules;

    public override EntityType Type => EntityType.Reactor;

    public override Vector2 Size => Type.GetSizeForEntity(inputsCount - 2);

    public override void Update(World world, float delta)
    {
        // TODO: create helper to make timer
        if (items.All(x => x != null))
        {
            elapsedTime += delta;
        }

        if (outputReady)
        {
            // TODO: support multiple output molecules
            var molecule = validReaction
                ? outputMolecules.First()
                : Molecule.InvalidMolecule;

            // TODO: output molecules count as separate molecules
            var created = world.TryCreateItem(molecule, AnchorPosition, Direction);
            if (created)
            {
                elapsedTime = 0;
                ClearItems();
                outputReady = false;
            }
        }
        else if (elapsedTime >= ReactionRate)
        {
            (validReaction, outputMolecules) = ReactionRegistry.CreateReaction([.. items.Select(x => x.Molecule)]);
            outputReady = true;
        }
    }

    public override bool TryConsumeItem(Item item, Vector2 position, Direction inputDirection)
    {
        if (inputDirection != inputsDirection)
        {
            return false;
        }

        var inputNumber = Mathf.RoundToInt(AnchorPosition.DistanceTo(position));
        if (items[inputNumber] == null)
        {
            items[inputNumber] = item;
            return true;
        }

        return false;
    }

    private void ClearItems()
    {
        for (var i = 0; i < items.Length; i++)
        {
            items[i] = null;
        }
    }
}
