using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChemFactory.scripts.Models;

public class Molecule(List<Atom> atoms, int count = 1)
{
    private static readonly Regex FormulaRegex = new(@"([A-Z][a-z]*)(\d*)", RegexOptions.Compiled);

    public List<Atom> Atoms { get; } = atoms;

    public int Count { get; } = count;

    public static Molecule InvalidMolecule
        => new([new(AtomElement.Invalid)]);

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
}
