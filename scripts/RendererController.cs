using System.Collections.Generic;
using Reactistry.scripts.Buildings;
using Reactistry.scripts.Models;
using Reactistry.scripts.Utilities;
using Godot;

namespace Reactistry.scripts;

public class RendererController : Node
{
    private TileMap resourcesTileMap;
    private TileMap baseTileMap;
    private TileMap overlayTileMap;
    private Node2D itemLayer;
    private Texture moleculeTexture;
    private World world;
    private readonly Dictionary<Molecule, int> moleculesTileId = [];
    private readonly Dictionary<Item, Sprite> itemSprites = [];

    public void Init(World world)
    {
        resourcesTileMap = GetNode<TileMap>("ResourcesTileMap");
        baseTileMap = GetNode<TileMap>("BaseTileMap");
        overlayTileMap = GetNode<TileMap>("OverlayTileMap");
        itemLayer = GetNode<Node2D>("Items");
        moleculeTexture = GD.Load<Texture>("res://assets/molecule.png");
        CreateMoleculesTileSet();

        this.world = world;
        this.world.ResourceCreated += OnResourceCreated;
        this.world.BuildingCreated += OnBuildingCreated;
        this.world.BuildingDeleted += OnBuildingDeleted;
        this.world.ItemsCreated += OnItemsCreated;
        this.world.ItemsDeleted += OnItemsDeleted;
    }

    public override void _Process(float delta)
    {
        DrawItems();
    }

    private void OnResourceCreated(Vector2 position, Molecule molecule)
    {
        resourcesTileMap.SetCellv(position, moleculesTileId.GetValueOrDefault(molecule, 0));
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

    private void OnItemsCreated(IEnumerable<Item> itemsToAdd)
    {
        foreach (var item in itemsToAdd)
        {
            CreateItemSprite(item);
        }
    }

    private void OnItemsDeleted(IEnumerable<Item> itemsToRemove)
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
            Texture = moleculeTexture,
            Modulate = item.Molecule.GetColor(),
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

    private void CreateMoleculesTileSet()
    {
        var tileSet = resourcesTileMap.TileSet;
        var texture = moleculeTexture;
        var molecules = new List<Molecule> { Molecule.InvalidMolecule, Molecule.H2, Molecule.C, Molecule.N2, Molecule.O2 };

        for (var i = 0; i < molecules.Count; i++)
        {
            var molecule = molecules[i];
            CreateTile(tileSet, i, texture, molecule.GetColor());
            moleculesTileId.Add(molecule, i);
        }
    }

    private void CreateTile(TileSet tileSet, int tileId, Texture texture, Color color)
    {
        tileSet.CreateTile(tileId);
        tileSet.TileSetTexture(tileId, texture);
        tileSet.TileSetRegion(tileId, new Rect2(0, 0, Constants.PixelsPerTile, Constants.PixelsPerTile));
        tileSet.TileSetModulate(tileId, color);
    }
}
