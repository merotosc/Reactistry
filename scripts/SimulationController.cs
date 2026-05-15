using Godot;

namespace ChemFactory.scripts;

public class SimulationController : Node
{
    private World world;
    private float elapsedTime;

    public void Init(World world)
    {
        this.world = world;
    }

    public override void _PhysicsProcess(float delta)
    {
        elapsedTime += delta;

        while (elapsedTime >= Constants.TickRate)
        {
            world.Tick(elapsedTime);
            elapsedTime -= Constants.TickRate;
        }
    }
}
