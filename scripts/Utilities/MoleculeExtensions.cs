using ChemFactory.scripts.Models;
using Godot;

namespace ChemFactory.scripts.Utilities;

public static class MoleculeExtensions
{
    public static Vector2 GetTileCoord(this Molecule molecule)
    {
        return molecule.ToString() switch
        {
            "H2" => new Vector2(0, 0),
            "C" => new Vector2(1, 0),
            "N2" => new Vector2(2, 0),
            "O2" => new Vector2(3, 0),
            _ => Vector2.Zero,
        };
    }
}
