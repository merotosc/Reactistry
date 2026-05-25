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
    private World world;
    private TasksUI tasksUi;
    private readonly List<LevelTasks> tasks = [];

    public int CurrentLevel { get; private set; }

    public LevelTasks CurrentTasks { get; private set; } = [];

    public void Init(World world, SaveData saveData)
    {
        this.world = world;
        CurrentLevel = saveData.Level;
        tasksUi = GetNode<TasksUI>("/root/Game/Canvas/TasksUI");

        foreach (var lab in world.Buildings.OfType<Lab>())
        {
            lab.ItemDelivered += OnItemDelivered;
        }

        LoadTasksFromCsv();
        RestoreCurrentTasks(saveData.CurrentTasks);
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

                tasks.Add(levelTasks);
            }

            i++;
        }

        file.Close();
    }

    private void OnItemDelivered(Item item)
    {
        if (!CurrentTasks.TryGetLabTask(item.Molecule, out var labTask))
        {
            return;
        }

        labTask.AddDeliveredAmount(item.Molecule.Count);

        if (CurrentTasks.AllLabTasksCompleted())
        {
            CurrentLevel++;
            LoadCurrentTask();
        }
        else
        {
            RefreshCurrentTask();
        }
    }

    private void RestoreCurrentTasks(List<LabTaskSaveData> tasksSaveData)
    {
        LoadCurrentTask();

        foreach (var taskSaveData in tasksSaveData)
        {
            var labTask = CurrentTasks.FirstOrDefault(x => x.Molecule.Formula == taskSaveData.MoleculeFormula);
            if (labTask == null)
            {
                continue;
            }

            labTask.AddDeliveredAmount(taskSaveData.AmountDelivered);
        }

        RefreshCurrentTask();
    }

    private void LoadCurrentTask()
    {
        if (tasks.Count > CurrentLevel)
        {
            CurrentTasks = tasks[CurrentLevel];
            tasksUi.CreateNewTasks(CurrentTasks);
        }
    }

    private void RefreshCurrentTask()
    {
        if (CurrentTasks.Any())
        {
            tasksUi.UpdateTasks(CurrentTasks);
        }
    }
}
