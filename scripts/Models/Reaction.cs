using System.Collections.Generic;

namespace ChemFactory.scripts.Models;

public class Reaction
{
    public string Name { get; set; }

    public List<Molecule> InputMolecules { get; set; } = [];

    public List<Molecule> OutputMolecules { get; set; } = [];

    public override string ToString()
        => ReactionRegistry.GetFormula(InputMolecules);
}
