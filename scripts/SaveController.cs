using System.Linq;
using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Reactistry.scripts.Models;
using Reactistry.scripts.Utilities;

namespace Reactistry.scripts;

public class SaveController : Node
{
    private Label saveNotification;
    private World world;
    private TasksController tasksController;
    private float autosaveTimer = 0;

    public void Init(World world, TasksController tasksController)
    {
        saveNotification = GetNode<Label>("/root/Game/Canvas/SaveNotification");
        saveNotification.Visible = false;

        this.world = world;
        this.tasksController = tasksController;
    }

    public override void _Process(float delta)
    {
        autosaveTimer += delta;

        if (autosaveTimer >= Constants.SaveData.AutosaveIntervalSeconds)
        {
            autosaveTimer = 0f;
            SaveGame();
        }
    }

    public override void _Input(InputEvent e)
    {
        if (Input.IsActionJustPressed("save_game", true))
        {
            SaveGame();
        }
    }

    public void SaveGame()
    {
        var buildings = world.Buildings
            .Where(x => x.Type != BuildingType.Lab)
            .Select(x => x.ToBuildingOptions())
            .ToList();

        var currentTasks = tasksController.CurrentTasks
            .Select(x => new LabTaskSaveData
            {
                MoleculeFormula = x.Molecule.Formula,
                AmountDelivered = x.AmountDelivered,
            }).ToList();

        var saveData = new SaveData
        {
            Level = tasksController.CurrentLevel,
            CurrentTasks = currentTasks,
            Buildings = buildings,
        };

        var json = JsonConvert.SerializeObject(saveData, new StringEnumConverter());

        var file = new File();
        file.Open(Constants.SaveData.Path, File.ModeFlags.Write);
        file.StoreString(json);
        file.Close();

        ShowSavedMessage();
    }

    public SaveData LoadGame()
    {
        var file = new File();
        if (!file.FileExists(Constants.SaveData.Path))
        {
            return new();
        }

        file.Open(Constants.SaveData.Path, File.ModeFlags.Read);

        var json = file.GetAsText();
        if (string.IsNullOrWhiteSpace(json))
        {
            return new();
        }

        return JsonConvert.DeserializeObject<SaveData>(json, new StringEnumConverter());
    }

    public async void ShowSavedMessage()
    {
        saveNotification.Visible = true;

        await ToSignal(GetTree().CreateTimer(3), "timeout");

        var tween = new Tween();
        saveNotification.AddChild(tween);

        tween.InterpolateProperty(
            saveNotification,
            "modulate:a",
            1f,
            0f,
            1f);

        tween.Start();

        await ToSignal(tween, "tween_all_completed");

        saveNotification.Visible = false;
        saveNotification.Modulate = new Color(0, 0, 0, 1);
        tween.QueueFree();
    }
}
