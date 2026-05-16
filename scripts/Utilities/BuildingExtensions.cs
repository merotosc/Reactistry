using ChemFactory.scripts.Models;
using Godot;

namespace ChemFactory.scripts.Utilities;

public static class BuildingExtensions
{
    public static Vector2 GetTileCoordForBuilding(this BuildingType buildingType, int variant = 0)
    {
        return buildingType switch
        {
            BuildingType.Belt => new Vector2(variant, Constants.TileSet.PipesYOffset),
            BuildingType.Producer => new Vector2(0, Constants.TileSet.BuildingsYOffset),
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
            BuildingType.Belt => Vector2.One,
            BuildingType.Producer => Vector2.One,
            BuildingType.Consumer => Vector2.One,
            BuildingType.Reactor => new Vector2(1, 2 + variant),
            BuildingType.Splitter => new Vector2(1, 2 + variant),
            BuildingType.Merger => new Vector2(1, 2 + variant),
            _ => Vector2.One,
        };
    }
}
