using System.Collections.Generic;
using System.Linq;
using Godot;

public class SimulationManager : Node
{
    private const int TicksPerSecond = 10;
    private const float TickRate = 1.0f / TicksPerSecond;
    private const int PixelsPerTile = 8;
    private float elapsedTime;
    private readonly Dictionary<Vector2, Belt> belts = [];
    private readonly Dictionary<Vector2, Producer> producers = [];
    private readonly List<Item> items = [];
    private TileMap tileMap;
    private Node2D itemLayer;

    public override void _Ready()
    {
        tileMap = GetNode<TileMap>("/root/Game/TileMap");
        itemLayer = GetNode<Node2D>("/root/Game/Items");

        items.Add(new Item { Type = ItemType.O });

        belts.Add(new Vector2(0, 0), new Belt { OutputDirection = Direction.Right, Item = items[0] });
        belts.Add(new Vector2(1, 0), new Belt { OutputDirection = Direction.Right });
        belts.Add(new Vector2(2, 0), new Belt { OutputDirection = Direction.Right });
        belts.Add(new Vector2(3, 0), new Belt { OutputDirection = Direction.Right });
        belts.Add(new Vector2(4, 0), new Belt { OutputDirection = Direction.Right });

        producers.Add(new Vector2(-1, 0), new Producer { OutputDirection = Direction.Right });

        DrawBelts();
        DrawItems();
    }

    public override void _PhysicsProcess(float delta)
    {
        elapsedTime += delta;

        while (elapsedTime >= TickRate)
        {
            Tick(delta);
            elapsedTime -= TickRate;
        }
    }

    private void Tick(float delta)
    {
        var beltsWithItems = belts
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

        foreach (var (position, producer) in producers)
        {
            producer.Update(delta);
        }
    }

    private void MoveToNextBeltIfFree(Vector2 position, Belt belt)
    {
        var nextPosition = position + belt.GetNextPosition();
        if (!belts.TryGetValue(nextPosition, out var nextBelt) || nextBelt.Item != null)
        {
            return;
        }

        belt.Item.Progress -= 1;
        nextBelt.Item = belt.Item;
        belt.Item = null;
    }

    private static void MoveOnBelt(Vector2 position, Belt belt, float delta)
    {
        belt.Item.Progress += delta * belt.Speed;
        var localPosition = belt.GetInterpolatedPosition(belt.Item.Progress);
        belt.Item.Sprite.Position = (position + localPosition) * PixelsPerTile;
    }

    private void DrawBelts()
    {
        foreach (var (position, _) in belts)
        {
            var tileId = 0;
            tileMap.SetCell((int)position.x, (int)position.y, tileId);
        }
    }

    private void DrawItems()
    {
        foreach (var item in items)
        {
            var sprite = new Sprite
            {
                Name = item.Type.ToString(),
                Texture = GetTextureForItem(item.Type),
            };

            item.Sprite = sprite;
            itemLayer.AddChild(sprite);
        }
    }

    private static Texture GetTextureForItem(ItemType item)
    {
        return item switch
        {
            ItemType.O => GD.Load<Texture>("res://assets/oxygen.png"),
            ItemType.H => GD.Load<Texture>("res://assets/hydrogen.png"),
            _ => null
        };
    }
}
