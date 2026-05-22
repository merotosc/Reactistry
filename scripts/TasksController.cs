using System.Collections.Generic;
using System.Linq;
using ChemFactory.scripts.Buildings;
using ChemFactory.scripts.Models;
using ChemFactory.scripts.UI;
using Godot;

namespace ChemFactory.scripts;

public class TasksController : Node
{
    private const string TasksFile = "res://assets/tasks.csv";
    private World world;
    private TasksUI tasksUi;
    private readonly Queue<LevelTasks> tasks = [];
    private LevelTasks currentTask;

    public void Init(World world)
    {
        this.world = world;
        tasksUi = GetNode<TasksUI>("/root/Game/Canvas/TasksUI");

        foreach (var lab in world.Buildings.OfType<Lab>())
        {
            lab.ItemDelivered += OnItemDelivered;
        }

        LoadTasksFromCsv();
        GetNextTask();
    }

    private void LoadTasksFromCsv()
    {
        var file = new File();
        if (!file.FileExists(TasksFile))
        {
            GD.PrintErr("Tasks CSV file not found", TasksFile);
            return;
        }

        file.Open(TasksFile, File.ModeFlags.Read);

        List<Molecule> molecules = [];
        var i = 0;

        while (!file.EofReached())
        {
            var row = file.GetCsvLine();

            if (i == 0)
            {
                foreach (var column in row.Skip(1))
                {
                    var molecule = Molecule.Parse(column);
                    molecules.Add(molecule);
                }
            }
            else
            {
                var levelTasks = new LevelTasks();

                for (var column = 0; column < row.Length; column++)
                {
                    var text = row[column];

                    if (column == 0)
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(text))
                    {
                        continue;
                    }

                    if (!int.TryParse(text, out var value) || value == 0)
                    {
                        continue;
                    }

                    var molecule = molecules[column - 1];
                    levelTasks.Add(new(molecule, value));
                }

                tasks.Enqueue(levelTasks);
            }

            i++;
        }

        file.Close();
    }

    private void OnItemDelivered(Item item)
    {
        if (currentTask == null || !currentTask.TryGetLabTask(item.Molecule, out var labTask))
        {
            return;
        }

        labTask.IncrementDeliveryCount();

        if (currentTask.AllLabTasksCompleted())
        {
            GetNextTask();
        }
        else
        {
            RefreshCurrentTask();
        }
    }

    private void GetNextTask()
    {
        if (tasks.TryDequeue(out var task))
        {
            currentTask = task;
            tasksUi.CreateNewTasks(currentTask);
        }
    }

    private void RefreshCurrentTask()
    {
        if (currentTask != null)
        {
            tasksUi.UpdateTasks(currentTask);
        }
    }
}
