using System.Collections.Generic;
using ChemFactory.scripts.Buildings;
using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts;

public class RendererController : Node
{
    private readonly Dictionary<Item, Sprite> itemSprites = [];
    private World world;
    private TileMap baseTileMap;
    private TileMap overlayTileMap;
    private Node2D itemLayer;

    public void Init(World world)
    {
        baseTileMap = GetNode<TileMap>("BaseTileMap");
        overlayTileMap = GetNode<TileMap>("OverlayTileMap");
        itemLayer = GetNode<Node2D>("Items");
        this.world = world;
        this.world.BuildingCreated += OnBuildingCreated;
        this.world.BuildingDeleted += OnBuildingDeleted;
        this.world.ItemCreated += OnItemCreated;
        this.world.ItemDeleted += OnItemDeleted;
    }

    public override void _Process(float delta)
    {
        DrawItems();
    }

    private void OnBuildingCreated(IBuilding building)
    {
        baseTileMap.DrawBuilding(building.ToBuildingOptions(), overlayTileMap);
    }

    private void OnBuildingDeleted(Vector2 position)
    {
        baseTileMap.SetCellv(position, tile: -1);
        overlayTileMap.SetCellv(position, tile: -1);
    }

    private void OnItemCreated(IEnumerable<Item> itemsToAdd)
    {
        foreach (var item in itemsToAdd)
        {
            CreateItemSprite(item);
        }
    }

    private void OnItemDeleted(IEnumerable<Item> itemsToRemove)
    {
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
            var localPosition = item.GetPositionOnPath();
            sprite.Position = (item.TilePosition + localPosition) * Constants.PixelsPerTile;
        }
    }
}