using System.Collections.Generic;
using System.Linq;
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
        this.world.EntityDeleted += OnEntityDeleted;
    }

    public override void _Process(float delta)
    {
        // TODO: also pass added/removed items using event callback?
        AddCreatedItems();
        RemoveDeletedItems();
        DrawItems();
    }

    private void OnEntityCreated(IEntity entity, EntityOptions entityOptions)
    {
        tileMap.DrawEntity(entity.Type, entityOptions);
    }

    private void OnEntityDeleted(Vector2 position)
    {
        tileMap.SetCellv(position, tile: -1);
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
        var name = item.Molecule.ToString();
        var sprite = new Sprite
        {
            Name = name,
            Texture = GD.Load<Texture>("res://assets/molecule.png"),
            ZIndex = 5,
            Modulate = name.ColorHash(),
        };

        itemSprites.Add(item, sprite);
        itemLayer.AddChild(sprite);
    }

    private void DrawItems()
    {
        foreach (var (item, sprite) in itemSprites)
        {
            var entityExists = world.EntityTiles.TryGetValue(item.TilePosition, out var _);
            if (!entityExists)
            {
                GD.PrintErr("Entity does not exist at item tile position: " + item.TilePosition);
                continue;
            }

            var localPosition = item.GetPositionOnPath();
            sprite.Position = (item.TilePosition + localPosition) * Constants.PixelsPerTile;
        }
    }
}