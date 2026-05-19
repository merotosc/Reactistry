namespace ChemFactory.scripts.Models;

public class LabTask(Molecule molecule, int amountRequired)
{
    public Molecule Molecule { get; } = molecule;

    public int AmountRequired { get; } = amountRequired;

    public int AmountDelivered { get; set; } = 0;

    public bool Completed => AmountDelivered >= AmountRequired;
}