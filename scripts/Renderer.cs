using System.Collections.Generic;
using System.Linq;
using Godot;

namespace ChemFactory.scripts;

public class Renderer : Node
{
    private World world;
    private Dictionary<Item, Sprite> itemSprites = [];
    private TileMap tileMap;
    private Node2D itemLayer;

    public void Init(World world)
    {
        this.world = world;
        tileMap = GetNode<TileMap>("/root/Game/TileMap");
        itemLayer = GetNode<Node2D>("/root/Game/Items");

        InitBelts();
        InitProducers();
    }

    public override void _Process(float delta)
    {
        AddCreatedItems();
        RemoveDeletedItems();
        DrawItems();
    }

    private void InitBelts()
    {
        foreach (var (position, _) in world.Belts)
        {
            tileMap.SetCell((int)position.x, (int)position.y, tile: 0, autotileCoord: new Vector2(0, 0));
        }
    }

    private void InitProducers()
    {
        foreach (var (position, _) in world.Producers)
        {
            tileMap.SetCell((int)position.x, (int)position.y, tile: 0, autotileCoord: new Vector2(0, 1));
        }
    }

    private void AddCreatedItems()
    {
        var itemsToAdd = world.Items
            .Where(i => !itemSprites.ContainsKey(i))
            .ToList();

        foreach (var item in itemsToAdd)
        {
            CreateItemSprite(item);
        }
    }

    private void RemoveDeletedItems()
    {
        var itemsToRemove = itemSprites.Keys
            .Where(i => !world.Items.Contains(i))
            .ToList();

        foreach (var item in itemsToRemove)
        {
            var sprite = itemSprites[item];
            itemLayer.RemoveChild(sprite);
            itemSprites.Remove(item);
        }
    }

    private void CreateItemSprite(Item item)
    {
        var sprite = new Sprite
        {
            Name = item.Type.ToString(),
            Texture = GetTextureForItem(item.Type),
        };

        itemSprites.Add(item, sprite);
        itemLayer.AddChild(sprite);
    }

    private void DrawItems()
    {
        foreach (var (item, sprite) in itemSprites)
        {
            var beltExists = world.Belts.TryGetValue(item.TilePosition, out var belt);
            if (!beltExists)
            {
                GD.PrintErr("Belt does not exist at item tile position");
                continue;
            }

            var localPosition = belt.GetInterpolatedPosition(item.Progress);
            sprite.Position = (item.TilePosition + localPosition) * Constants.PixelsPerTile;
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