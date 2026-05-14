using Godot;

namespace ChemFactory.scripts;

public class Producer : IBuilding
{
    private const float ProductionRate = 2;
    private float elapsedTime = 0;

    public ItemType ItemType { get; set; }

    public Direction OutputDirection { get; set; }

    public void Update(Vector2 position, World world, float delta)
    {
        elapsedTime += delta;

        while (elapsedTime >= ProductionRate)
        {
            var created = world.CreateItem(ItemType, position, OutputDirection);
            if (!created)
            {
                break;
            }

            elapsedTime -= ProductionRate;
        }
    }

    public bool TryConsumeItem(Item item, Direction inputDirection)
        => false;
}
