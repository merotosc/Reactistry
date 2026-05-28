using System;
using System.Collections.Generic;
using System.Linq;

namespace Reactistry.scripts.Models;

public class LabTasks : List<LabTask>
{
    public bool TryGetTask(Molecule molecule, out LabTask task)
    {
        task = this.FirstOrDefault(x => x.Molecule.Formula == molecule.Formula);
        return task != null;
    }

    public bool RemoveTaskById(string id)
    {
        return RemoveAll(x => x.Id == id) == 1;
    }
}

public class LabTask(string id, Molecule molecule, int amountRequired)
{
    public string Id { get; } = id;

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
