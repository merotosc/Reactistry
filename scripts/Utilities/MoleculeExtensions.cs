using Reactistry.scripts.Models;
using Godot;

namespace Reactistry.scripts.Utilities;

public static class MoleculeExtensions
{
    public static Color GetColor(this Molecule molecule)
    {
        var name = molecule.ToString();
        return name switch
        {
            "H2" => new Color("FFFFFF"),
            "C" => new Color("909090"),
            "N2" => new Color("3050F8"),
            "O2" => new Color("FF0D0D"),
            "Invalid" => new Color("000000"),
            _ => name.ColorHash(),
        };
    }
}
