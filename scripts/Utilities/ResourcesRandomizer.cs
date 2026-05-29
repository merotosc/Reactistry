using System;
using System.Collections.Generic;
using System.Linq;
using Reactistry.scripts.Models;
using Godot;

namespace Reactistry.scripts.Utilities;

public static class ResourcesRandomizer
{
    public static void SpawnResources(Action<Vector2, Molecule> action)
    {
        var moleculesPerRing = GetRandomMoleculesPerRing();
        var worldEnd = (Constants.Map.WorldSize - Vector2.One) / 2;
        var worldStart = -worldEnd;

        for (var y = worldStart.y; y <= worldEnd.y; y++)
        {
            for (var x = worldStart.x; x <= worldEnd.x; x++)
            {
                var ring = (int)Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                if (ring == 0)
                {
                    continue;
                }

                if (!moleculesPerRing.TryGetValue(ring, out var molecules) ||
                    !molecules.TryDequeue(out var molecule))
                {
                    continue;
                }

                SpawnResourcesCluster(new Vector2(x, y), molecule, action);
            }
        }
    }

    private static void SpawnResourcesCluster(Vector2 chunk, Molecule molecule, Action<Vector2, Molecule> action)
    {
        var chunkSize = Constants.Map.ChunkSize;
        var minClusterSize = Constants.Map.MinClusterSize;
        var maxClusterSize = Constants.Map.MaxClusterSize;

        var chunkOrigin = (chunk * chunkSize - chunkSize / 2).Ceil();
        var rng = new Vector3(chunk.x, chunk.y, Constants.Map.Seed).GetRandom();
        var offsetX = rng.Next(0, (int)(chunkSize.x - maxClusterSize.x));
        var offsetY = rng.Next(0, (int)(chunkSize.y - maxClusterSize.y));
        var localOffset = new Vector2(offsetX, offsetY);
        var globalOffset = chunkOrigin + localOffset;

        var clusterWidth = rng.Next((int)minClusterSize.x, (int)maxClusterSize.x + 1);
        var clusterHeight = rng.Next((int)minClusterSize.y, (int)maxClusterSize.y + 1);

        for (var y = 0; y < clusterHeight; y++)
        {
            for (var x = 0; x < clusterWidth; x++)
            {
                var position = globalOffset + new Vector2(x, y);
                action(position, molecule);
            }
        }
    }

    private static Dictionary<int, Queue<Molecule>> GetRandomMoleculesPerRing()
    {
        var moleculesPerRing = new Dictionary<int, Queue<Molecule>>();

        foreach (var (ring, rules) in Constants.Map.ResourceSpawnRules)
        {
            var molecules = new List<Molecule>();
            var chunksCount = ring * 2 * 4; // Height/width doubled and 4 sides
            var rng = new Vector2(ring, Constants.Map.Seed).GetRandom();

            foreach (var rule in rules)
            {
                for (var i = 0; i < rule.Min; i++)
                {
                    molecules.Add(rule.Molecule);
                }
            }

            while (molecules.Count < chunksCount)
            {
                molecules.Add(GetWeightedRandomMolecule(rules, rng));
            }

            var moleculesQueue = new Queue<Molecule>(molecules.OrderBy(_ => rng.Next()));
            moleculesPerRing.Add(ring, moleculesQueue);
        }

        return moleculesPerRing;
    }

    private static Molecule GetWeightedRandomMolecule(List<ResourceSpawnRule> rules, Random rng)
    {
        var totalWeight = rules.Sum(r => r.Weight);
        var roll = (float)(rng.NextDouble() * totalWeight);
        var current = 0f;

        foreach (var rule in rules)
        {
            current += rule.Weight;
            if (roll <= current)
            {
                return rule.Molecule;
            }
        }

        return rules.Last().Molecule;
    }
}
