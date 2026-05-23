using System.Linq;
using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Reactistry.scripts.Models;

namespace Reactistry.scripts;

public class SaveController : Node
{
    private float autosaveTimer = 0;
    private World world;
    private TasksController tasksController;

    public void Init(World world, TasksController tasksController)
    {
        this.world = world;
        this.tasksController = tasksController;
    }

    public override void _Process(float delta)
    {
        autosaveTimer += delta;

        if (autosaveTimer >= Constants.SaveData.AutosaveInterval)
        {
            autosaveTimer = 0f;
            SaveGame();
        }
    }

    public override void _Input(InputEvent e)
    {
        if (e is InputEventKey key && key.Pressed)
        {
            if (key.Control && key.Scancode == (uint)KeyList.S)
            {
                SaveGame();
            }
        }
    }

    public void SaveGame()
    {
        var buildings = world.Buildings
            .Where(x => x.Type != BuildingType.Lab)
            .Select(x => new BuildingOptions
            {
                Type = x.Type,
                Position = x.AnchorPosition,
                Direction = x.Direction,
                Variant = x.Variant,
            }).ToList();

        var currentTasks = tasksController.CurrentTasks
            .Select(x => new LabTaskSaveData
            {
                MoleculeName = x.Molecule.ToString(),
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
}
