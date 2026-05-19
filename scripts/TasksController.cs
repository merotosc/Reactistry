using System.Collections.Generic;
using System.Linq;
using ChemFactory.scripts.Buildings;
using ChemFactory.scripts.Models;
using Godot;

namespace ChemFactory.scripts;

public class TasksController : Node
{
    private World world;
    private TasksUI tasksUi;
    private readonly Queue<LabTask> tasks = [];
    private LabTask currentTask;

    public void Init(World world)
    {
        this.world = world;
        tasksUi = GetNode<TasksUI>("/root/Game/Canvas/TasksUI");

        foreach (var lab in world.Buildings.OfType<Lab>())
        {
            lab.ItemDelivered += OnItemDelivered;
        }

        // TODO: make configurable JSON list
        var hardcodedTasks = ReactionRegistry.Molecules.Select(x => new LabTask(x, 10));

        foreach (var task in hardcodedTasks)
        {
            tasks.Enqueue(task);
        }

        GetNextTask();
        RefreshCurrentTask();
    }

    private void OnItemDelivered(Item item)
    {
        if (currentTask == null || currentTask.Molecule != item.Molecule)
        {
            return;
        }

        currentTask.AmountDelivered++;

        if (currentTask.Completed)
        {
            GetNextTask();
        }

        RefreshCurrentTask();
    }

    private void GetNextTask()
    {
        if (tasks.TryDequeue(out var task))
        {
            currentTask = task;
        }
    }

    private void RefreshCurrentTask()
    {
        if (currentTask == null)
        {
            return;
        }

        tasksUi.UpdateTask(currentTask);
    }
}
