using System;
using System.Collections.Generic;
using System.Linq;
using ChemFactory.scripts.Models;
using Godot;

namespace ChemFactory.scripts;

public static class ReactionRegistry
{
    private const string ReactionsFile = "res://assets/reactions.csv";
    private static readonly List<Reaction> reactions = [];

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

        var molecules = new List<Molecule>();
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
                        if (value < 0)
                        {
                            // TODO: add count to molecule instead of separate ones?
                            for (i = 0; i < Math.Abs(value); i++)
                                reaction.InputMolecules.Add(molecule);
                        }
                        else
                        {
                            for (i = 0; i < value; i++)
                                reaction.OutputMolecules.Add(molecule);
                        }
                    }
                }

                if (reaction.InputMolecules.Any() && reaction.OutputMolecules.Any())
                {
                    reactions.Add(reaction);
                    GD.PrintS("Registered reaction", reaction.ToString());
                }
            }

            i++;
        }

        file.Close();
    }

    public static (bool Valid, List<Molecule> OutputMolecules) CreateReaction(List<Molecule> molecules)
    {
        var moleculesName = GetFormula(molecules);
        GD.PrintS("Requested reaction with following molecules", moleculesName);

        var reaction = reactions.FirstOrDefault(x => x.ToString() == moleculesName);
        if (reaction == null)
        {
            return (false, null);
        }

        return (true, reaction.OutputMolecules);
    }

    public static string GetFormula(List<Molecule> molecules)
        => string.Join("", molecules
            .Select(x => x.ToString())
            .OrderBy(x => x));
}
