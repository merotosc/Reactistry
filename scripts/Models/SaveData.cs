using System;
using System.Collections.Generic;

namespace Reactistry.scripts.Models;

public class SaveData
{
    public DateTime? LastSaved { get; set; } = null;

    public List<LabTaskSaveData> CurrentTasks { get; set; } = [];

    public List<string> CompletedTasks { get; set; } = [];

    public List<BuildingOptions> Buildings { get; set; } = [];
}

public class LabTaskSaveData
{
    public string Id { get; set; }

    public int AmountDelivered { get; set; }
}