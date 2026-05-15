using ChemFactory.scripts.Items;
using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts.Buildings;

public class Producer : IBuilding
{
    private const float ProductionRate = 2;
    private float elapsedTime = 0;

    public ItemType ItemType { get; set; }

    public Direction OutputDirection { get; set; }

    public EntityType Type => EntityType.Producer;

    public Direction Direction => OutputDirection;

    public Vector2 Size => Type.GetSizeForEntity();

    public void Update(Vector2 position, World world, float delta)
    {
        elapsedTime += delta;

        if (elapsedTime >= ProductionRate)
        {
            var created = world.CreateItem(ItemType, position, OutputDirection);
            if (created)
            {
                elapsedTime -= ProductionRate;
            }
        }
    }

    public bool TryConsumeItem(Item item, Direction inputDirection)
        => false;
}
