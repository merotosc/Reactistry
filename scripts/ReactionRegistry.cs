using System;
using System.Collections.Generic;
using System.Linq;
using Reactistry.scripts.Models;
using Godot;

namespace Reactistry.scripts;

public static class ReactionRegistry
{
    private const string ReactionsFile = "res://assets/reactions.csv";
    private static readonly List<Molecule> molecules = [];
    private static readonly Dictionary<string, Reaction> reactions = [];

    public static IReadOnlyList<Molecule> Molecules => molecules;

    public static void Load()
    {
        try
        {
            LoadReactionsFromCsv();
        }
        catch (Exception ex)
        {
            GD.PrintErr("An exception occurred loading the reactions csv file\n", ex);
        }
    }

    public static void LoadReactionsFromCsv()
    {
        var file = new File();
        if (!file.FileExists(ReactionsFile))
        {
            GD.PrintErr("Reactions CSV file not found", ReactionsFile);
            return;
        }

        file.Open(ReactionsFile, File.ModeFlags.Read);

        var i = 0;

        while (!file.EofReached())
        {
            var row = file.GetCsvLine();

            if (i == 0)
            {
                foreach (var column in row.Skip(1))
                {
                    var molecule = Molecule.Parse(column);
                    molecules.Add(molecule);
                }
            }
            else
            {
                var reaction = new Reaction();

                for (var column = 0; column < row.Length; column++)
                {
                    var text = row[column];

                    if (column == 0)
                    {
                        reaction.Name = text;
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(text))
                    {
                        continue;
                    }

                    if (!int.TryParse(text, out var value) || value == 0)
                    {
                        continue;
                    }

                    var molecule = molecules[column - 1];
                    var newMolecule = molecule with { Count = Math.Abs(value) };

                    if (value < 0)
                    {
                        reaction.InputMolecules.Add(newMolecule);
                    }
                    else
                    {
                        reaction.OutputMolecules.Add(newMolecule);
                    }
                }

                if (reaction.InputMolecules.Any() && reaction.OutputMolecules.Any())
                {
                    var formula = GetStableFormula(reaction.InputMolecules);
                    var formulaOutput = GetStableFormula(reaction.OutputMolecules);
                    reactions.Add(formula, reaction);
                    GD.PrintT("Registered reaction", reaction.Name, formula, formulaOutput);
                }
            }

            i++;
        }

        file.Close();
    }

    public static (bool Valid, List<Molecule> OutputMolecules) CreateReaction(List<Molecule> molecules)
    {
        var formula = GetStableFormula(molecules);
        //GD.PrintS("Requested reaction for", formula);

        if (!reactions.TryGetValue(formula, out var reaction))
        {
            return (false, null);
        }

        //GD.PrintS("Found matching reaction", reaction.Name);
        return (true, reaction.OutputMolecules);
    }

    public static string GetStableFormula(List<Molecule> molecules)
    {
        var elements = new Dictionary<AtomElement, int>();

        foreach (var molecule in molecules)
        {
            foreach (var element in molecule.Elements)
            {
                elements.TryGetValue(element.Key, out var current);
                elements[element.Key] = current + element.Value * molecule.Count;
            }
        }

        return string.Join(":", elements
            .OrderBy(x => x.Key)
            .Select(x => $"{x.Key}{x.Value}"));
    }
}
