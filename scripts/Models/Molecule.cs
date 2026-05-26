using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Godot;
using Reactistry.scripts.Utilities;

namespace Reactistry.scripts.Models;

public record class Molecule
{
    private static readonly Regex FormulaRegex = new(@"([A-Z][a-z]*)(\d*)", RegexOptions.Compiled);

    public string Formula { get; }

    public Dictionary<AtomElement, int> Elements { get; }

    public int Count { get; set; }

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

    private Molecule(Dictionary<AtomElement, int> elements, int count = 1, string formula = null)
    {
        Elements = elements;
        Count = count;
        Formula = formula ?? CalculateFormula();
    }

    private Molecule(AtomElement atom, int count = 1)
        : this(new Dictionary<AtomElement, int> { [atom] = count })
    {
    }

    public static Molecule Parse(string formula)
    {
        var elements = new Dictionary<AtomElement, int>();
        var matches = FormulaRegex.Matches(formula);

        foreach (Match match in matches)
        {
            var element = match.Groups[1].Value;
            var count = string.IsNullOrEmpty(match.Groups[2].Value)
                ? 1
                : int.Parse(match.Groups[2].Value);

            var elementParsed = Enum.TryParse<AtomElement>(element, ignoreCase: true, out var parsedElement);
            var elementName = elementParsed ? parsedElement : AtomElement.Invalid;

            elements.TryGetValue(elementName, out var currentCount);
            elements[elementName] = currentCount + count;
        }

        return new Molecule(elements, formula: formula);
    }

    public Color GetColor()
    {
        return Formula switch
        {
            "H2" => new Color("FFFFFF"),
            "C" => new Color("909090"),
            "N2" => new Color("3050F8"),
            "O2" => new Color("FF0D0D"),
            "Invalid" => new Color("000000"),
            _ => Formula.ColorHash(),
        };
    }

    private string CalculateFormula()
    {
        var elements = Elements
            .Select(e => e.Value > 1
                ? $"{e.Key}{e.Value}"
                : $"{e.Key}")
            .ToArray();

        var formula = string.Join("", elements);

        return Count > 1
            ? $"{Count}{formula}"
            : formula;
    }
}
