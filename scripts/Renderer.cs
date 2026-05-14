using System.Collections.Generic;
using System.Linq;
using Godot;

namespace ChemFactory.scripts;

public class Renderer : Node
{
    private readonly Dictionary<Item, Sprite> itemSprites = [];
    private World world;
    private TileMap tileMap;
    private Node2D itemLayer;

    public void Init(World world)
    {
        this.world = world;
        tileMap = GetNode<TileMap>("/root/Game/TileMap");
        itemLayer = GetNode<Node2D>("/root/Game/Items");

        InitBelts();
        InitBuildings();
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

    private void InitBuildings()
    {
        foreach (var (position, building) in world.Buildings)
        {
            var tileCoord = GetTileForBuilding(building);
            tileMap.SetCell((int)position.x, (int)position.y, tile: 0, autotileCoord: tileCoord);
        }
    }

    private void AddCreatedItems()
    {
        var itemsToAdd = world.Items
            .Where(x => x.Visible && !itemSprites.ContainsKey(x))
            .ToList();

        foreach (var item in itemsToAdd)
        {
            CreateItemSprite(item);
        }
    }

    private void RemoveDeletedItems()
    {
        var itemsToRemove = itemSprites.Keys
            .Where(x => !x.Visible || !world.Items.Contains(x))
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
                GD.PrintErr("Belt does not exist at item tile position: " + item.TilePosition);
                continue;
            }

            var localPosition = belt.GetInterpolatedPosition(item.Progress);
            sprite.Position = (item.TilePosition + localPosition) * Constants.PixelsPerTile;
        }
    }

    private static Vector2 GetTileForBuilding(IBuilding building)
    {
        return building switch
        {
            Producer => new Vector2(0, 1),
            Consumer => new Vector2(1, 1),
            _ => Vector2.Zero,
        };
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