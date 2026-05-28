using System;
using System.Linq;
using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Reactistry.scripts.Models;
using Reactistry.scripts.Utilities;

namespace Reactistry.scripts;

public class SaveController : Node
{
    private Label notificationMessage;
    private World world;
    private TasksController tasksController;
    private float autosaveTimer = 0;

    public void Init(World world, TasksController tasksController)
    {
        notificationMessage = GetNode<Label>("/root/Game/Canvas/NotificationMessage");
        notificationMessage.Visible = false;

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

        if (Input.IsActionJustPressed("clear_game", true))
        {
            ClearGame();
        }
    }

    public void SaveGame()
    {
        try
        {
            var buildings = world.Buildings
                .Where(x => x.Type != BuildingType.Lab)
                .Select(x => x.ToBuildingOptions())
                .ToList();

            var currentTasks = tasksController.CurrentTasks
                .Select(x => new LabTaskSaveData
                {
                    Id = x.Id,
                    AmountDelivered = x.AmountDelivered,
                }).ToList();

            var saveData = new SaveData
            {
                CurrentTasks = currentTasks,
                CompletedTasks = tasksController.CompletedTasks,
                Buildings = buildings,
            };

            var json = JsonConvert.SerializeObject(saveData, new StringEnumConverter());

            var file = new File();
            file.Open(Constants.SaveData.Path, File.ModeFlags.Write);
            file.StoreString(json);
            file.Close();

            ShowMessage("Game Saved");
        }
        catch (Exception ex)
        {
            GD.PrintErr("An exception occurred saving the game\n", ex);
            ShowMessage("Error Saving Game");
        }
    }

    public SaveData LoadGame()
    {
        try
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
        catch (Exception ex)
        {
            GD.PrintErr("An exception occurred loading the game\n", ex);
            return new();
        }
    }

    public void ClearGame()
    {
        try
        {
            var file = new File();
            file.Open(Constants.SaveData.Path, File.ModeFlags.Write);
            file.StoreString(string.Empty);
            file.Close();

            ShowMessage("Game Cleared");
        }
        catch (Exception ex)
        {
            GD.PrintErr("An exception occurred saving the game\n", ex);
            ShowMessage("Error Clearing Game");
        }
    }

    public async void ShowMessage(string message)
    {
        notificationMessage.Visible = true;
        notificationMessage.Text = message;

        await ToSignal(GetTree().CreateTimer(3), "timeout");

        var tween = new Tween();
        notificationMessage.AddChild(tween);

        tween.InterpolateProperty(
            notificationMessage,
            "modulate:a",
            1f,
            0f,
            1f);

        tween.Start();

        await ToSignal(tween, "tween_all_completed");

        notificationMessage.Visible = false;
        notificationMessage.Modulate = new Color(0, 0, 0, 1);
        tween.QueueFree();
    }
}
