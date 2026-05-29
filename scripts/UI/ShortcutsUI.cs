using Godot;

namespace Reactistry.scripts.UI;

public class ShortcutsUI : Control
{
    private Control sidebar;
    private Button toggleButton;
    private readonly float animationDuration = 0.2f;
    private bool expanded = true;
    private Tween tween;
    private Vector2 expandedPosition;
    private Vector2 collapsedPosition;

    public void Init(bool firstRun)
    {
        sidebar = GetNode<Control>("Sidebar/PanelContainer");
        toggleButton = GetNode<Button>("Sidebar/ToggleButton");

        tween = new Tween();
        AddChild(tween);

        expandedPosition = sidebar.RectPosition;
        collapsedPosition = expandedPosition - new Vector2(sidebar.RectSize.x, 0);

        if (!firstRun)
        {
            TogglePanel();
        }
    }

    public void OnButtonPressed()
    {
        TogglePanel();
    }

    private void TogglePanel()
    {
        expanded = !expanded;
        toggleButton.Text = expanded ? "<" : ">";

        tween.StopAll();

        tween.InterpolateProperty(
            sidebar,
            "rect_position",
            sidebar.RectPosition,
            expanded ? expandedPosition : collapsedPosition,
            animationDuration,
            Tween.TransitionType.Cubic,
            Tween.EaseType.Out);

        tween.Start();
    }
}
