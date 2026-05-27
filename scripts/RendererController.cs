using System.Collections.Generic;
using Reactistry.scripts.Buildings;
using Reactistry.scripts.Models;
using Reactistry.scripts.Utilities;
using Godot;
using System;

namespace Reactistry.scripts;

public class RendererController : Node
{
    private const float AnimationDelay = 0.3f;
    private TileMap resourcesTileMap;
    private TileMap baseTileMap;
    private TileMap overlayTileMap;
    private Node2D itemLayer;
    private Texture moleculeItemTexture;
    private Texture moleculeResourceTexture;
    private World world;
    private readonly Dictionary<Molecule, int> moleculesTileId = [];
    private readonly Dictionary<Item, Sprite> itemSprites = [];

    public void Init(World world)
    {
        resourcesTileMap = GetNode<TileMap>("ResourcesTileMap");
        baseTileMap = GetNode<TileMap>("BaseTileMap");
        overlayTileMap = GetNode<TileMap>("OverlayTileMap");
        itemLayer = GetNode<Node2D>("Items");
        moleculeItemTexture = GD.Load<Texture>("res://assets/molecule_overlay.png");
        moleculeResourceTexture = GD.Load<Texture>("res://assets/molecule.png");
        CreateMoleculesTileSet();

        this.world = world;
        this.world.ResourceCreated += OnResourceCreated;
        this.world.BuildingCreated += OnBuildingCreated;
        this.world.BuildingDeleted += OnBuildingDeleted;
        this.world.ItemsCreated += OnItemsCreated;
        this.world.ItemsDeleted += OnItemsDeleted;
    }

    public void Tick(float _delta)
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
            if (!itemSprites.TryGetValue(item, out var sprite))
            {
                continue;
            }

            DeleteItemSprite(item, sprite);
        }
    }

    private void CreateItemSprite(Item item)
    {
        var name = item.Molecule.Formula;
        var sprite = new Sprite
        {
            Name = name,
            Texture = moleculeItemTexture,
            Modulate = item.Molecule.GetColor(),
        };

        itemSprites.Add(item, sprite);
        itemLayer.AddChild(sprite);

        var tween = new Tween();
        sprite.AddChild(tween);

        tween.InterpolateProperty(
            sprite,
            "scale",
            Vector2.Zero,
            Vector2.One,
            AnimationDelay,
            Tween.TransitionType.Back,
            Tween.EaseType.Out);

        tween.InterpolateProperty(
            sprite,
            "modulate:a",
            0.5f,
            1f,
            AnimationDelay);

        tween.Start();
        tween.Connect("tween_all_completed", tween, "queue_free");
    }

    private void DeleteItemSprite(Item item, Sprite sprite)
    {
        itemSprites.Remove(item);

        var tween = new Tween();
        sprite.AddChild(tween);

        tween.InterpolateProperty(
            sprite,
            "scale",
            sprite.Scale,
            Vector2.Zero,
            AnimationDelay,
            Tween.TransitionType.Back,
            Tween.EaseType.In);

        tween.InterpolateProperty(
            sprite,
            "modulate:a",
            1f,
            0.5f,
            AnimationDelay);

        tween.Start();
        tween.Connect("tween_all_completed", this, nameof(OnItemShrinkCompleted), [tween, sprite]);
    }

    private void OnItemShrinkCompleted(Tween tween, Sprite sprite)
    {
        itemLayer.RemoveChild(sprite);
        tween.QueueFree();
        sprite.QueueFree();
    }

    private void DrawItems()
    {
        foreach (var (item, sprite) in itemSprites)
        {
            var localPosition = item.GetPositionOnPath();
            sprite.Position = (item.TilePosition + localPosition) * Constants.Map.TileSize;
        }
    }

    private void CreateMoleculesTileSet()
    {
        var tileSet = resourcesTileMap.TileSet;
        var texture = moleculeResourceTexture;
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
        tileSet.TileSetRegion(tileId, new Rect2(0, 0, Constants.Map.TileSize, Constants.Map.TileSize));
        tileSet.TileSetModulate(tileId, color);
    }
}
