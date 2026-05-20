using System.Collections.Generic;
using ChemFactory.scripts.Buildings;
using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts;

public class RendererController : Node
{
    private TileMap backgroundTileMap;
    private TileMap baseTileMap;
    private TileMap overlayTileMap;
    private Node2D itemLayer;
    private World world;
    private readonly Dictionary<Item, Sprite> itemSprites = [];

    public void Init(World world)
    {
        backgroundTileMap = GetNode<TileMap>("BackgroundTileMap");
        baseTileMap = GetNode<TileMap>("BaseTileMap");
        overlayTileMap = GetNode<TileMap>("OverlayTileMap");
        itemLayer = GetNode<Node2D>("Items");
        this.world = world;
        this.world.ResourceCreated += OnResourceCreated;
        this.world.BuildingCreated += OnBuildingCreated;
        this.world.BuildingDeleted += OnBuildingDeleted;
        this.world.ItemCreated += OnItemCreated;
        this.world.ItemDeleted += OnItemDeleted;
    }

    public override void _Process(float delta)
    {
        DrawItems();
    }

    private void OnResourceCreated(Vector2 position, Molecule molecule)
    {
        backgroundTileMap.SetCellv(position, tile: Constants.TileSet.MoleculesId, autotileCoord: new Vector2(0, 0)); // TODO: correct molecule tile based on type
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