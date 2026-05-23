namespace Reactistry.scripts.Models;

public class ResourceSpawnRule(Molecule molecule, int min, int weight)
{
    public Molecule Molecule { get; set; } = molecule;

    public int Min { get; set; } = min;

    public int Weight { get; set; } = weight;
}
