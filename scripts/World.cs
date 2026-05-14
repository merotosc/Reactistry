using System.Collections.Generic;
using System.Linq;
using Godot;

namespace ChemFactory.scripts;

public class World
{
    public Dictionary<Vector2, Belt> Belts { get; set; } = [];

    public Dictionary<Vector2, Producer> Producers { get; set; } = [];

    public List<Item> Items { get; set; } = [];

    public World()
    {
        Belts.Add(new Vector2(0, 0), new Belt { OutputDirection = Direction.Right });
        Belts.Add(new Vector2(1, 0), new Belt { OutputDirection = Direction.Right });
        Belts.Add(new Vector2(2, 0), new Belt { OutputDirection = Direction.Right });
        Belts.Add(new Vector2(3, 0), new Belt { OutputDirection = Direction.Right });
        Belts.Add(new Vector2(4, 0), new Belt { OutputDirection = Direction.Right });

        Producers.Add(new Vector2(-1, 0), new Producer { ItemType = ItemType.O, OutputDirection = Direction.Right });
    }

    public void Tick(float delta)
    {
        UpdateBeltItems(delta);
        UpdateProducers(delta);
    }

    public bool CreateItem(ItemType itemType, Vector2 outputPosition, Direction outputDirection)
    {
        // TODO: use common method to check next belt
        var tilePosition = outputPosition + outputDirection.ToVector();
        if (!Belts.TryGetValue(tilePosition, out var belt) || belt.Item != null || belt.InputDirection != outputDirection.ReverseDirection())
        {
            return false;
        }

        var item = new Item
        {
            Type = itemType,
            TilePosition = tilePosition,
        };

        belt.Item = item;
        Items.Add(item);

        return true;
    }

    private void UpdateBeltItems(float delta)
    {
        var beltsWithItems = Belts
            .Where(x => x.Value.Item != null)
            .ToList();

        foreach (var (position, belt) in beltsWithItems)
        {
            if (belt.Item.Progress >= 1)
            {
                MoveToNextBeltIfFree(position, belt);
            }
            else
            {
                MoveOnBelt(position, belt, delta);
            }
        }
    }

    private void UpdateProducers(float delta)
    {
        foreach (var (position, producer) in Producers)
        {
            producer.Update(position, this, delta);
        }
    }

    private void MoveToNextBeltIfFree(Vector2 position, Belt belt)
    {
        var nextPosition = position + belt.OutputPosition();
        if (!Belts.TryGetValue(nextPosition, out var nextBelt) || nextBelt.Item != null)
        {
            return;
        }

        belt.Item.Progress -= 1;
        belt.Item.TilePosition = nextPosition;
        nextBelt.Item = belt.Item;
        belt.Item = null;
    }

    private static void MoveOnBelt(Vector2 position, Belt belt, float delta)
    {
        belt.Item.Progress += delta * belt.Speed;
    }
}
