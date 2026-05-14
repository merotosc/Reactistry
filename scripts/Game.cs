using Godot;

namespace ChemFactory.scripts;

public class Game : Node
{
    private World world = new();

    public override void _Ready()
    {
        GetNode<SimulationManager>("SimulationManager").Init(world);
        GetNode<Renderer>("Renderer").Init(world);
    }
}
