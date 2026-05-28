using System.Collections.Generic;
using Godot;

namespace Reactistry.scripts.UI;

public class TutorialUI : Control
{
    private class TutorialStep
    {
        public Texture Image;
        public string Text;
    }

    private readonly List<TutorialStep> steps = [];
    private int currentIndex = 0;

    private Button helpButton;
    private Button prevButton;
    private Button nextButton;
    private Button closeButton;

    private Label stepLabel;
    private Label descriptionLabel;
    private TextureRect imageRect;

    private Control panel;

    public override void _Ready()
    {
        helpButton = GetNode<Button>("HelpButton");

        panel = GetNode<Control>("PanelContainer");

        prevButton = GetNode<Button>("PanelContainer/Content/Header/PreviousButton");
        nextButton = GetNode<Button>("PanelContainer/Content/Header/NextButton");
        closeButton = GetNode<Button>("PanelContainer/Content/Header/CloseButton");

        stepLabel = GetNode<Label>("PanelContainer/Content/Header/StepLabel");
        descriptionLabel = GetNode<Label>("PanelContainer/Content/Description");
        imageRect = GetNode<TextureRect>("PanelContainer/Content/Image");

        helpButton.Connect("pressed", this, nameof(ToggleTutorial));
        prevButton.Connect("pressed", this, nameof(PrevStep));
        nextButton.Connect("pressed", this, nameof(NextStep));
        closeButton.Connect("pressed", this, nameof(ToggleTutorial));

        LoadTutorialSteps();

        panel.Visible = false;
    }

    private void LoadTutorialSteps()
    {
        var index = 0;

        while (true)
        {
            var pngPath = $"res://assets/tutorial/{index}.png";
            var txtPath = $"res://assets/tutorial/{index}.txt";

            var image = GD.Load<Texture>(pngPath);
            var text = LoadTextFile(txtPath);

            if (image == null && text == null)
                break;

            steps.Add(new TutorialStep
            {
                Image = image,
                Text = text ?? ""
            });

            index++;
        }

        if (steps.Count == 0)
        {
            GD.PrintErr("No tutorial steps found!");
        }
    }

    private string LoadTextFile(string path)
    {
        var file = new File();

        if (!file.FileExists(path))
            return null;

        file.Open(path, File.ModeFlags.Read);
        var content = file.GetAsText();
        file.Close();

        return content;
    }

    private void UpdateUI()
    {
        if (steps.Count == 0)
        {
            return;
        }

        var step = steps[currentIndex];

        imageRect.Texture = step.Image;
        descriptionLabel.Text = step.Text;

        stepLabel.Text = $"{currentIndex + 1} / {steps.Count}";

        prevButton.Visible = currentIndex != 0;
        nextButton.Visible = currentIndex != steps.Count - 1;
    }

    private void ToggleTutorial()
    {
        panel.Visible = !panel.Visible;
        if (panel.Visible)
        {
            currentIndex = 0;
            UpdateUI();
        }
    }

    private void NextStep()
    {
        if (currentIndex < steps.Count - 1)
        {
            currentIndex++;
            UpdateUI();
        }
    }

    private void PrevStep()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateUI();
        }
    }
}
