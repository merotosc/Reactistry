using System.Collections.Generic;
using Reactistry.scripts.Models;
using Godot;

namespace Reactistry.scripts;

public class Constants
{
    public const int TicksPerSecond = 20;
    public const float TickRate = 1.0f / TicksPerSecond;
    public const int PixelsPerTile = 16;
    public const float ItemSpeed = 20;

    public static class TileSet
    {
        public const int BuildingsId = 3;
        public const int BuildingsBaseId = 0;
        public const int BuildingsOverlayId = 1;
        public const int IconsId = 2;
        public const int MoleculesId = 4;
        public const int PipesYOffset = 0;
        public const int BuildingsYOffset = 1;
        public const int LabYOffset = 2;
        public const int IconsYOffset = 0;
    }

    public static class Map
    {
        public static readonly Vector2 WorldSize = new(7, 7);
        public static readonly Vector2 ChunkSize = new(33, 33);
        public static readonly Vector2 MinClusterSize = new(2, 2);
        public static readonly Vector2 MaxClusterSize = new(4, 2);

        public static readonly Dictionary<int, List<ResourceSpawnRule>> ResourceSpawnRules = new()
        {
            [1] = // Ring 1
            [
                new(Molecule.O2, min: 2, weight: 5),
                new(Molecule.H2, min: 2, weight: 5),
            ],
            [2] = // Ring 2
            [
                new(Molecule.O2, min: 2, weight: 5),
                new(Molecule.H2, min: 2, weight: 5),
                new(Molecule.C, min: 2, weight: 2),
            ],
            [3] = // Ring 3
            [
                new(Molecule.O2, min: 2, weight: 5),
                new(Molecule.H2, min: 2, weight: 5),
                new(Molecule.C, min: 2, weight: 2),
                new(Molecule.N2, min: 2, weight: 1),
            ],
        };
    }
}
