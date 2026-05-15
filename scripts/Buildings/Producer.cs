using ChemFactory.scripts.Items;
using ChemFactory.scripts.Models;
using Godot;

namespace ChemFactory.scripts.Buildings;

public class Producer(Vector2 anchorPosition, Direction direction)
    : Building(anchorPosition, direction)
{
    private const float ProductionRate = 2;
    private float elapsedTime = 0;

    public Molecule Molecule { get; set; }

    public override EntityType Type => EntityType.Producer;

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
}
