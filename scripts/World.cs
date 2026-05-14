using System.Collections.Generic;
using System.Linq;
using Godot;

namespace ChemFactory.scripts;

public class World
{
    public Dictionary<Vector2, Belt> Belts { get; set; } = [];

    public Dictionary<Vector2, IBuilding> Buildings { get; set; } = [];

    public List<Item> Items { get; set; } = [];

    public World()
    {
        Belts.Add(new Vector2(0, 0), new Belt { InputDirection = Direction.Left, OutputDirection = Direction.Right });
        Belts.Add(new Vector2(1, 0), new Belt { InputDirection = Direction.Left, OutputDirection = Direction.Right });
        Belts.Add(new Vector2(2, 0), new Belt { InputDirection = Direction.Left, OutputDirection = Direction.Right });
        Belts.Add(new Vector2(3, 0), new Belt { InputDirection = Direction.Left, OutputDirection = Direction.Right });
        Belts.Add(new Vector2(4, 0), new Belt { InputDirection = Direction.Left, OutputDirection = Direction.Right });

        Belts.Add(new Vector2(5, -3), new Belt { InputDirection = Direction.Up, OutputDirection = Direction.Down });
        Belts.Add(new Vector2(5, -2), new Belt { InputDirection = Direction.Up, OutputDirection = Direction.Down });
        Belts.Add(new Vector2(5, -1), new Belt { InputDirection = Direction.Up, OutputDirection = Direction.Down });

        Buildings.Add(new Vector2(-1, 0), new Producer { ItemType = ItemType.O, OutputDirection = Direction.Right });
        Buildings.Add(new Vector2(5, -4), new Producer { ItemType = ItemType.H, OutputDirection = Direction.Down });
        Buildings.Add(new Vector2(5, 0), new Consumer { InputDirection = Direction.Left });
    }

    public void Tick(float delta)
    {
        UpdateBeltItems(delta);
        UpdateBuildings(delta);
    }

    public bool CreateItem(ItemType itemType, Vector2 outputPosition, Direction outputDirection)
    {
        var tilePosition = outputPosition + outputDirection.ToVector();

        var item = new Item
        {
            Type = itemType,
            TilePosition = tilePosition,
            Visible = true,
        };

        var created = TryMoveItemToEntity(item, tilePosition, outputDirection);
        if (created)
        {
            Items.Add(item);
        }

        return created;
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
                TryMoveToNextTile(position, belt);
            }
            else
            {
                MoveOnBelt(position, belt, delta);
            }
        }
    }

    private void UpdateBuildings(float delta)
    {
        foreach (var (position, building) in Buildings)
        {
            building.Update(position, this, delta);
        }
    }

    private void TryMoveToNextTile(Vector2 position, Belt belt)
    {
        var tilePosition = position + belt.OutputPosition();
        var moved = TryMoveItemToEntity(belt.Item, tilePosition, belt.OutputDirection);
        if (moved)
        {
            belt.Item = null;
        }
    }

    private bool TryMoveItemToEntity(Item item, Vector2 position, Direction outputDirection)
    {
        var moved = TryMoveItemToEntity(item, position, outputDirection, Belts)
            || TryMoveItemToEntity(item, position, outputDirection, Buildings);

        if (moved)
        {
            item.Progress = item.Progress > 1 ? item.Progress - 1 : 0;
            item.TilePosition = position;
        }

        return moved;
    }

    private static bool TryMoveItemToEntity<T>(Item item, Vector2 position, Direction outputDirection, Dictionary<Vector2, T> entities)
        where T : IEntity
    {
        return entities.TryGetValue(position, out var entity) && entity.TryConsumeItem(item, outputDirection.ReverseDirection());
    }

    private static void MoveOnBelt(Vector2 position, Belt belt, float delta)
    {
        belt.Item.Progress += delta * belt.Speed;
    }
}
