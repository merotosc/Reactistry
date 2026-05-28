using Reactistry.scripts.UI;
using Godot;
using System;

namespace Reactistry.scripts;

public class GameController : Node
{
    private readonly World world = new();
    private RendererController rendererController;
    private float elapsedTime;
    private bool isPaused;

    public override void _Ready()
    {
        ReactionRegistry.Load();
        rendererController = GetNode<RendererController>("RendererController");
        var saveController = GetNode<SaveController>("SaveController");
        var buildController = GetNode<BuildController>("BuildController");
        var tasksController = GetNode<TasksController>("TasksController");
        var saveData = saveController.LoadGame();

        GetNode<TasksUI>("Canvas/TasksUI").Init();
        GetNode<ToolsUI>("Canvas/ToolsUI").Init();
        GetNode<TooltipUI>("Canvas/TooltipUI").Init(world);
        saveController.Init(world, tasksController);
        buildController.Init(world);
        rendererController.Init(world); // Renderer controller must init before as it subscribes to the buildings events
        world.LoadWorld(saveData);
        tasksController.Init(world, saveData); // Tasks controller must init after because it requires an existing Lab building to exist in the World
    }

    public override void _PhysicsProcess(float delta)
    {
        try
        {
            if (isPaused)
            {
                return;
            }

            elapsedTime += delta;

            if (elapsedTime >= Constants.TickRate)
            {
                world.Tick(elapsedTime);
                rendererController.Tick(elapsedTime);
                elapsedTime -= Constants.TickRate;
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr("An exception occurred during game controller physics process\n", ex);
        }
    }

    public override void _Input(InputEvent e)
    {
        if (Input.IsActionJustPressed("toggle_pause", true))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        isPaused = !isPaused;
    }
}
