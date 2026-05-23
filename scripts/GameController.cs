using Reactistry.scripts.UI;
using Godot;
using System.Linq;

namespace Reactistry.scripts;

public class GameController : Node
{
    private readonly World world = new();
    private float elapsedTime;

    public override void _Ready()
    {
        ReactionRegistry.Load();
        var saveController = GetNode<SaveController>("SaveController");
        var tasksController = GetNode<TasksController>("TasksController");
        var saveData = saveController.LoadGame();

        saveController.Init(world, tasksController);
        GetNode<TasksUI>("Canvas/TasksUI").Init();
        GetNode<ToolsUI>("Canvas/ToolsUI").Init();
        GetNode<TooltipUI>("Canvas/TooltipUI").Init(world);
        GetNode<BuildController>("BuildController").Init(world);
        GetNode<RendererController>("RendererController").Init(world); // Renderer controller must init before as it subscribes to the buildings events
        world.LoadWorld(saveData);
        tasksController.Init(world, saveData); // Tasks controller must init after because it requires an existing Lab building to exist in the World
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
