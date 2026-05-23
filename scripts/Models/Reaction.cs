using System.Collections.Generic;

namespace Reactistry.scripts.Models;

public class Reaction
{
    public string Name { get; set; }

    public List<Molecule> InputMolecules { get; set; } = [];

    public List<Molecule> OutputMolecules { get; set; } = [];
}
