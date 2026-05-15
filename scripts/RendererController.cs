using System.Collections.Generic;
using System.Linq;
using ChemFactory.scripts.Items;
using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts;

public class RendererController : Node
{
    private readonly Dictionary<Item, Sprite> itemSprites = [];
    private World world;
    private TileMap tileMap;
    private Node2D itemLayer;

    public void Init(World world)
    {
        tileMap = GetNode<TileMap>("/root/Game/TileMap");
        itemLayer = GetNode<Node2D>("/root/Game/Items");
        this.world = world;
        this.world.EntityCreated += OnEntityCreated;
    }

    public override void _Process(float delta)
    {
        AddCreatedItems();
        RemoveDeletedItems();
        DrawItems();
    }

    private void OnEntityCreated(Vector2 position, IEntity entity)
    {
        GD.PrintS("entity created");
        var tileCoord = entity.Type.GetTileCoordForEntity();
        SetTile(position, entity.Direction, tileCoord);
    }

    private void AddCreatedItems()
    {
        var itemsToAdd = world.Items
            .Where(x => !itemSprites.ContainsKey(x))
            .ToList();

        foreach (var item in itemsToAdd)
        {
            CreateItemSprite(item);
        }
    }

    private void RemoveDeletedItems()
    {
        var itemsToRemove = itemSprites.Keys
            .Where(x => !world.Items.Contains(x))
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
            ZIndex = 5,
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

    private void SetTile(Vector2 position, Direction direction, Vector2 tileCoord)
    {
        var (mirror, rotate) = direction.GetTileOptionsForDirection();
        tileMap.SetCellv(position, tile: 0, autotileCoord: tileCoord, flipX: mirror && !rotate, flipY: mirror && rotate, transpose: rotate);
    }

    private static Texture GetTextureForItem(ItemType item)
    {
        return item switch
        {
            ItemType.O => GD.Load<Texture>("res://assets/oxygen.png"),
            ItemType.H => GD.Load<Texture>("res://assets/hydrogen.png"),
            ItemType.HO => GD.Load<Texture>("res://assets/ho.png"),
            _ => null
        };
    }
}