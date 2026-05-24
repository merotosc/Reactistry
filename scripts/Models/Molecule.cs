using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Reactistry.scripts.Models;

public class Molecule(List<Atom> atoms, int count = 1)
{
    private static readonly Regex FormulaRegex = new(@"([A-Z][a-z]*)(\d*)", RegexOptions.Compiled);

    public List<Atom> Atoms { get; } = atoms;

    public int Count { get; } = count;

    public static Molecule InvalidMolecule
        = new(AtomElement.Invalid);

    public static Molecule H2
        = new(AtomElement.H, 2);

    public static Molecule C
        = new(AtomElement.C, 1);

    public static Molecule N2
        = new(AtomElement.N, 2);

    public static Molecule O2
        = new(AtomElement.O, 2);

    public Molecule(AtomElement atom, int count = 1)
        : this(new(atom, count))
    {
    }

    public Molecule(Atom atom)
        : this([atom])
    {
    }

    public static Molecule Parse(string formula)
    {
        var atoms = new List<Atom>();
        var matches = FormulaRegex.Matches(formula);

        foreach (Match match in matches)
        {
            var element = match.Groups[1].Value;
            var count = string.IsNullOrEmpty(match.Groups[2].Value)
                ? 1
                : int.Parse(match.Groups[2].Value);

            var elementParsed = Enum.TryParse<AtomElement>(element, ignoreCase: true, out var parsedElement);
            atoms.Add(new Atom(elementParsed ? parsedElement : AtomElement.Invalid, count));
        }

        return new Molecule(atoms);
    }

    // TODO: make cached property and molecule immutable?
    public override string ToString()
    {
        var atoms = Atoms
            .OrderBy(x => x.Element)
            .Select(x => x.ToString())
            .ToArray();

        var formula = string.Join("", atoms);

        return Count > 1
            ? $"{Count}{formula}"
            : formula;
    }

    public override bool Equals(object obj)
    {
        return obj is Molecule molecule &&
               EqualityComparer<List<Atom>>.Default.Equals(Atoms, molecule.Atoms) &&
               Count == molecule.Count;
    }

    public override int GetHashCode()
        => HashCode.Combine(Atoms, Count);

    public static bool operator ==(Molecule left, Molecule right)
        => left?.ToString() == right?.ToString();

    public static bool operator !=(Molecule left, Molecule right)
        => !(left == right);
}
