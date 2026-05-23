using System.Collections.Generic;

namespace Reactistry.scripts.Models;

public class BuildingInfo
{
    public BuildingState BuildingState { get; set; }

    public IEnumerable<Item> InputItems { get; set; } = [];

    public IEnumerable<Item> OutputItems { get; set; } = [];
}
