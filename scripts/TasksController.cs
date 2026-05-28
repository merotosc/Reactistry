using System.Collections.Generic;
using System.Linq;
using Reactistry.scripts.Buildings;
using Reactistry.scripts.Models;
using Reactistry.scripts.UI;
using Godot;

namespace Reactistry.scripts;

public class TasksController : Node
{
    private const string TasksFile = "res://assets/tasks.csv";
    private const int MaxTasks = 3;
    private World world;
    private TasksUI tasksUi;
    private readonly Queue<LabTask> tasks = [];

    public LabTasks CurrentTasks { get; private set; } = [];

    public List<string> CompletedTasks { get; private set; } = [];

    public void Init(World world, SaveData saveData)
    {
        this.world = world;
        tasksUi = GetNode<TasksUI>("/root/Game/Canvas/TasksUI");

        foreach (var lab in world.Buildings.OfType<Lab>())
        {
            lab.ItemDelivered += OnItemDelivered;
        }

        LoadTasksFromCsv();
        RestoreCurrentTasks(saveData.CompletedTasks, saveData.CurrentTasks);
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
        var rowIndex = 0;

        while (!file.EofReached())
        {
            var row = file.GetCsvLine();

            if (rowIndex == 0)
            {
                foreach (var column in row.Skip(1))
                {
                    var molecule = Molecule.Parse(column);
                    molecules.Add(molecule);
                }
            }
            else
            {
                var taskId = row.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(taskId))
                {
                    GD.PrintErr("No ID found for task at row ", rowIndex);
                    return;
                }

                for (var column = 1; column < row.Length; column++)
                {
                    var moleculeCount = row[column];

                    if (string.IsNullOrWhiteSpace(moleculeCount))
                    {
                        continue;
                    }

                    if (!int.TryParse(moleculeCount, out var value) || value == 0)
                    {
                        GD.PrintErr("Molecule count is invalid for task with ID ", taskId);
                        continue;
                    }

                    var molecule = molecules[column - 1];
                    tasks.Enqueue(new(taskId, molecule, value));
                    break;
                }
            }

            rowIndex++;
        }

        file.Close();
    }

    private void OnItemDelivered(Item item)
    {
        if (!CurrentTasks.TryGetTask(item.Molecule, out var labTask))
        {
            return;
        }

        labTask.AddDeliveredAmount(item.Molecule.Count);

        if (labTask.Completed)
        {
            CompletedTasks.Add(labTask.Id);
            CurrentTasks.RemoveTaskById(labTask.Id);

            if (CurrentTasks.Count < MaxTasks && tasks.TryDequeue(out var task))
            {
                CurrentTasks.Add(task);
            }

            RecreateCurrentTasks();
        }
        else
        {
            RefreshCurrentTask();
        }
    }

    private void RestoreCurrentTasks(List<string> completedTasks, List<LabTaskSaveData> currentTasks)
    {
        while (CurrentTasks.Count < MaxTasks && tasks.TryDequeue(out var task))
        {
            if (completedTasks.Contains(task.Id))
            {
                CompletedTasks.Add(task.Id);
                continue;
            }

            var currentTask = currentTasks.FirstOrDefault(x => x.Id == task.Id);
            if (currentTask != null)
            {
                task.AddDeliveredAmount(currentTask.AmountDelivered);
                CurrentTasks.Add(task);
                continue;
            }

            CurrentTasks.Add(task);
        }

        RecreateCurrentTasks();
    }

    private void RecreateCurrentTasks()
    {
        tasksUi.RecreateTasks(CurrentTasks);
    }

    private void RefreshCurrentTask()
    {
        if (CurrentTasks.Any())
        {
            tasksUi.UpdateTasks(CurrentTasks);
        }
    }
}
