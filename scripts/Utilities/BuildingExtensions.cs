using System;
using ChemFactory.scripts.Buildings;
using ChemFactory.scripts.Models;
using Godot;

namespace ChemFactory.scripts.Utilities;

public static class BuildingExtensions
{
    public static Vector2 GetTileCoordForBuilding(this BuildingType buildingType, int variant = 0)
    {
        return buildingType switch
        {
            BuildingType.Lab => new Vector2(0, Constants.TileSet.LabYOffset),
            BuildingType.Pipe => new Vector2(variant, Constants.TileSet.PipesYOffset),
            BuildingType.Extractor => new Vector2(0, Constants.TileSet.BuildingsYOffset),
            BuildingType.Consumer => new Vector2(1, Constants.TileSet.BuildingsYOffset),
            BuildingType.Reactor => new Vector2(2, Constants.TileSet.BuildingsYOffset),
            BuildingType.Splitter => new Vector2(3, Constants.TileSet.BuildingsYOffset),
            BuildingType.Merger => new Vector2(4, Constants.TileSet.BuildingsYOffset),
            _ => Vector2.Zero,
        };
    }

    public static Vector2 GetSizeForBuilding(this BuildingType buildingType, int variant = 0)
    {
        return buildingType switch
        {
            BuildingType.Lab => new Vector2(6, 6),
            BuildingType.Pipe => Vector2.One,
            BuildingType.Extractor => Vector2.One,
            BuildingType.Consumer => Vector2.One,
            BuildingType.Reactor => new Vector2(1, 2 + variant),
            BuildingType.Splitter => new Vector2(1, 2 + variant),
            BuildingType.Merger => new Vector2(1, 2 + variant),
            _ => Vector2.One,
        };
    }

    public static int GetVariantsCountForBuilding(this BuildingType buildingType)
    {
        return buildingType switch
        {
            BuildingType.Lab => 1,
            BuildingType.Pipe => Enum.GetNames(typeof(PipeVariant)).Length,
            BuildingType.Extractor => 1,
            BuildingType.Consumer => 1,
            BuildingType.Reactor => Enum.GetNames(typeof(BuildingInputsVariant)).Length,
            BuildingType.Splitter => Enum.GetNames(typeof(BuildingInputsVariant)).Length,
            BuildingType.Merger => Enum.GetNames(typeof(BuildingInputsVariant)).Length,
            _ => 1,
        };
    }

    public static BuildingOptions ToBuildingOptions(this IBuilding building)
    {
        return new BuildingOptions
        {
            Type = building.Type,
            Position = building.AnchorPosition,
            Direction = building.Direction,
            Variant = building.Variant,
        };
    }
}
