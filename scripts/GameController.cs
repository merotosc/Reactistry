using Godot;

namespace ChemFactory.scripts;

public class GameController : Node
{
    private readonly World world = new();

    public override void _Ready()
    {
        GetNode<RendererController>("RendererController").Init(world);
        GetNode<SimulationController>("SimulationController").Init(world);
        GetNode<BuildController>("BuildController").Init(world);
    }
}
