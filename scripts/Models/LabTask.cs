using System;
using System.Collections.Generic;
using System.Linq;

namespace Reactistry.scripts.Models;

public class LevelTasks : List<LabTask>
{
    public bool TryGetLabTask(Molecule molecule, out LabTask labTask)
    {
        labTask = this.FirstOrDefault(x => x.Molecule == molecule);
        return labTask != null;
    }

    public bool AllLabTasksCompleted()
        => this.All(x => x.Completed);
}

public class LabTask(Molecule molecule, int amountRequired)
{
    public Molecule Molecule { get; } = molecule;

    public int AmountRequired { get; } = amountRequired;

    public int AmountDelivered { get; private set; } = 0;

    public bool Completed => AmountDelivered >= AmountRequired;

    public void AddDeliveredAmount(int count)
    {
        AmountDelivered += count;
        AmountDelivered = Math.Min(AmountDelivered, AmountRequired);
    }
}
