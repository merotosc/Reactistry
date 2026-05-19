using ChemFactory.scripts.Models;
using ChemFactory.scripts.Utilities;
using Godot;

namespace ChemFactory.scripts;

public class TasksUI : Control
{
    private Control contentContainer;
    private Label amount;
    private Label itemName;
    private Control itemTexture;

    public void Init()
    {
        contentContainer = GetNode<Control>("PanelContainer/Content");
        amount = contentContainer.GetNode<Label>("Amount");
        itemName = contentContainer.GetNode<Label>("Item/Name");
        itemTexture = contentContainer.GetNode<Control>("Item/ImageFrame/Image");
    }

    public void UpdateTask(LabTask task)
    {
        amount.Text = $"{task.AmountDelivered}\n/{task.AmountRequired}";
        itemName.Text = task.Molecule.ToString();
        itemTexture.Modulate = task.Molecule.ToString().ColorHash();
    }
}
