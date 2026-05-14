using System.Collections.Generic;
using System.Linq;
using Godot;

public class SimulationManager : Node
{
    private const int TicksPerSecond = 2;
    private const float TickRate = 1.0f / TicksPerSecond;
    private float elapsedTime;
    private readonly Dictionary<Vector2, Belt> belts = [];
    private TileMap tileMap;
    private Node2D itemLayer;

    public override void _Ready()
    {
        tileMap = GetNode<TileMap>("/root/Game/TileMap");
        itemLayer = GetNode<Node2D>("/root/Game/Items");

        belts.Add(new Vector2(0, 0), new Belt { Direction = Direction.Right, Item = Item.O });
        belts.Add(new Vector2(1, 0), new Belt { Direction = Direction.Right });
        belts.Add(new Vector2(2, 0), new Belt { Direction = Direction.Right });
        belts.Add(new Vector2(3, 0), new Belt { Direction = Direction.Right });
        belts.Add(new Vector2(4, 0), new Belt { Direction = Direction.Right });

        DrawBelts();
    }

    public override void _PhysicsProcess(float delta)
    {
        elapsedTime += delta;

        while (elapsedTime >= TickRate)
        {
            Tick();
            elapsedTime -= TickRate;
        }
    }

    private void Tick()
    {
        DrawItems();

        foreach (var (position, belt) in belts.Reverse())
        {
            if (belt.Item == Item.Empty)
            {
                continue;
            }

            var nextPosition = position + belt.GetNextPosition();
            if (!belts.TryGetValue(nextPosition, out var nextBelt))
            {
                continue;
            }

            if (nextBelt.Item != Item.Empty)
            {
                continue;
            }

            nextBelt.Item = belt.Item;
            belt.Item = Item.Empty;
        }
    }

    private void DrawBelts()
    {
        foreach (var (position, belt) in belts)
        {
            var tileId = 0;
            tileMap.SetCell((int)position.x, (int)position.y, tileId);
        }
    }

    private void DrawItems()
    {
        foreach (var (position, belt) in belts)
        {
            if (belt.Item == Item.Empty)
            {
                continue;
            }

            Sprite sprite;
            if (!itemLayer.HasNode(position.ToString()))
            {
                sprite = new Sprite { Name = position.ToString() };
                itemLayer.AddChild(sprite);
            }
            else
            {
                sprite = itemLayer.GetNode<Sprite>(position.ToString());
            }

            sprite.Position = position * 8;
            sprite.Texture = GetTextureForItem(belt.Item);
            GD.Print("Texture position: " + sprite.Position);
        }
    }

    private static Texture GetTextureForItem(Item item)
    {
        return item switch
        {
            Item.O => GD.Load<Texture>("res://assets/oxygen.png"),
            Item.H => GD.Load<Texture>("res://assets/hydrogen.png"),
            _ => null
        };
    }
}
