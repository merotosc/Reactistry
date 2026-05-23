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

    public void Init(World world)
    {
        this.world = world;
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
        var saveData = new SaveData
        {
            Buildings = world.Buildings
                .Where(x => x.Type != BuildingType.Lab)
                .Select(x => new BuildingOptions
                {
                    Type = x.Type,
                    Position = x.AnchorPosition,
                    Direction = x.Direction,
                    Variant = x.Variant,
                }).ToList()
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
