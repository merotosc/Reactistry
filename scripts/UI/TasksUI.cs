using Reactistry.scripts.Models;
using Reactistry.scripts.Utilities;
using Godot;

namespace Reactistry.scripts.UI;

public class TasksUI : Control
{
    private Control levelTasks;
    private PackedScene labTaskUi;

    public void Init()
    {
        levelTasks = GetNode<Control>("LevelTasks");
        labTaskUi = GD.Load<PackedScene>("res://scenes/lab_task_ui.tscn");
    }

    public void CreateNewTasks(LevelTasks tasks)
    {
        foreach (Node child in levelTasks.GetChildren())
        {
            levelTasks.RemoveChild(child);
            child.QueueFree();
        }

        foreach (var task in tasks)
        {
            var ui = labTaskUi.Instance() as Control;
            SetTask(ui, task);
            levelTasks.AddChild(ui);
        }
    }

    public void UpdateTasks(LevelTasks tasks)
    {
        var i = 0;
        foreach (var task in tasks)
        {
            var ui = levelTasks.GetChild<Control>(i++);
            SetTask(ui, task);
        }
    }

    private void SetTask(Control ui, LabTask task)
    {
        ui.GetNode<Label>("Content/Amount").Text = $"{task.AmountDelivered}\n/{task.AmountRequired}";
        ui.GetNode<Label>("Content/Item/Name").Text = task.Molecule.Formula;
        ui.GetNode<TextureRect>("Content/Item/ImageFrame/Image").Modulate = task.Molecule.GetColor();

        if (task.Completed)
        {
            ui.Modulate = new Color(0.8f, 1f, 0.75f);
        }
    }
}
