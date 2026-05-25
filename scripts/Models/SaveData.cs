using System.Collections.Generic;

namespace Reactistry.scripts.Models;

public class SaveData
{
    public int Level { get; set; } = 0;

    public List<LabTaskSaveData> CurrentTasks { get; set; } = [];

    public List<BuildingOptions> Buildings { get; set; } = [];
}

public class LabTaskSaveData
{
    public string MoleculeFormula { get; set; }

    public int AmountDelivered { get; set; }
}