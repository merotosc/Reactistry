using Godot;

namespace ChemFactory.scripts;

public class GameController : Node
{
    private readonly World world = new();
    private float elapsedTime;

    public override void _Ready()
    {
        ReactionRegistry.Load();
        GetNode<RendererController>("RendererController").Init(world); // Renderer must init before as it subscribes to the buildings events
        world.LoadDemo();
        GetNode<BuildController>("BuildController").Init(world);
        GetNode<TasksUI>("Canvas/TasksUI").Init();
        GetNode<TasksController>("TasksController").Init(world);
        GetNode<TooltipPanel>("Canvas/TooltipPanel").Init(world);
    }

    public override void _PhysicsProcess(float delta)
    {
        elapsedTime += delta;

        if (elapsedTime >= Constants.TickRate)
        {
            world.Tick(elapsedTime);
            elapsedTime -= Constants.TickRate;
        }
    }
}
