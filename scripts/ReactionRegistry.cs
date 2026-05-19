using System;
using System.Collections.Generic;
using System.Linq;
using ChemFactory.scripts.Models;
using Godot;

namespace ChemFactory.scripts;

public static class ReactionRegistry
{
    private const string ReactionsFile = "res://assets/reactions.csv";
    private static readonly List<Molecule> molecules = [];
    private static readonly Dictionary<string, Reaction> reactions = [];

    public static IReadOnlyList<Molecule> Molecules => molecules;

    public static void Load()
    {
        LoadCsv();
    }

    public static void LoadCsv()
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
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(text))
                        {
                            continue;
                        }

                        if (!int.TryParse(text, out var value) || value == 0)
                        {
                            continue;
                        }

                        var molecule = molecules[column - 1];
                        var newMolecule = new Molecule(molecule.Atoms, Math.Abs(value));

                        if (value < 0)
                        {
                            reaction.InputMolecules.Add(newMolecule);
                        }
                        else
                        {
                            reaction.OutputMolecules.Add(newMolecule);
                        }
                    }
                }

                if (reaction.InputMolecules.Any() && reaction.OutputMolecules.Any())
                {
                    var formula = GetStableFormula(reaction.InputMolecules);
                    reactions.Add(formula, reaction);
                    GD.PrintT("Registered reaction", reaction.Name, formula);
                }
            }

            i++;
        }

        file.Close();
    }

    public static (bool Valid, List<Molecule> OutputMolecules) CreateReaction(List<Molecule> molecules)
    {
        var formula = GetStableFormula(molecules);

        if (!reactions.TryGetValue(formula, out var reaction))
        {
            return (false, null);
        }

        return (true, reaction.OutputMolecules);
    }

    public static string GetFormula(List<Molecule> molecules)
        => string.Join("+", molecules
            .OrderBy(m => m.ToString())
            .Select(m => m.ToString()));

    public static string GetStableFormula(List<Molecule> molecules)
    {
        var atoms = new Dictionary<AtomElement, int>();

        foreach (var molecule in molecules)
        {
            foreach (var atom in molecule.Atoms)
            {
                if (!atoms.ContainsKey(atom.Element))
                {
                    atoms[atom.Element] = 0;
                }

                atoms[atom.Element] += atom.Count * molecule.Count;
            }
        }

        return string.Join(":", atoms
            .OrderBy(x => x.Key)
            .Select(x => $"{x.Key}{x.Value}"));
    }
}
