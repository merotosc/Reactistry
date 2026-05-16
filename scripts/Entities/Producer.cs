using System.Collections.Generic;
using ChemFactory.scripts.Items;
using ChemFactory.scripts.Models;
using Godot;

namespace ChemFactory.scripts.Entities;

public class Producer(Vector2 anchorPosition, Direction direction)
    : Entity(anchorPosition, direction)
{
    private const float ProductionRate = 2;
    private float elapsedTime = 0;

    public override EntityType Type => EntityType.Producer;

    public Molecule Molecule { get; set; } = Molecule.InvalidMolecule;

    public override void Update(World world, float delta)
    {
        elapsedTime += delta;

        if (elapsedTime >= ProductionRate)
        {
            var created = world.TryCreateItem(Molecule, AnchorPosition, Direction);
            if (created)
            {
                elapsedTime -= ProductionRate;
            }
        }
    }

    public override bool TryConsumeItem(Item item, Vector2 position, Direction inputDirection)
        => false;

    public override Vector2 GetInterpolatedPosition(float progress)
    {
        return Vector2.Zero;
    }

    public override IEnumerable<Item> GetItems()
        => [];
}
