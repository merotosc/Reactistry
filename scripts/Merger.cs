using Godot;

namespace ChemFactory.scripts;

public class Merger : IBuilding
{
    private const float MergeRate = 1;
    private float elapsedTime = 0;

    private Item item1;
    private Item item2;

    public Direction OutputDirection { get; set; }

    public Direction GetDirection()
        => OutputDirection;

    public void Update(Vector2 position, World world, float delta)
    {
        // TODO: create helper to make timer
        if (item1 != null && item2 != null)
        {
            elapsedTime += delta;
        }

        if (elapsedTime >= MergeRate)
        {
            var created = world.CreateItem(ItemType.HO, position, OutputDirection);
            if (created)
            {
                elapsedTime -= MergeRate;
                item1 = null;
                item2 = null;
                // TODO: request romoval of items
            }
        }
    }

    public bool TryConsumeItem(Item item, Direction inputDirection)
    {
        var inputDirection1 = OutputDirection.PreviousDirection();
        var inputDirection2 = OutputDirection.NextDirection();

        if (inputDirection == inputDirection1 && item1 == null)
        {
            item1 = item;
            item.Visible = false; // TODO: move logic outside building
            return true;
        }
        else if (inputDirection == inputDirection2 && item2 == null)
        {
            item2 = item;
            item.Visible = false;
            return true;
        }

        return false;
    }
}
