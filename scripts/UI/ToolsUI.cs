using Reactistry.scripts.Models;
using Reactistry.scripts.Utilities;
using Godot;
using Reactistry.scripts.Buildings;

namespace Reactistry.scripts.UI;

public class ToolsUI : Control
{
    private TileMap baseTileMap;
    private Control tools;
    private PackedScene toolButtonUi;
    private BuildController buildController;

    public void Init()
    {
        baseTileMap = GetNode<TileMap>("/root/Game/RendererController/BaseTileMap");
        tools = GetNode<Control>("PanelContainer/Tools");
        buildController = GetNode<BuildController>("/root/Game/BuildController");
        toolButtonUi = GD.Load<PackedScene>("res://scenes/tool_button_ui.tscn");
        InitButtons();
    }

    private void InitButtons()
    {
        for (var i = (int)BuildingType.Pipe; i < (int)BuildingType.End; i++)
        {
            AddBuildingTool((BuildingType)i);
        }
    }

    private void AddBuildingTool(BuildingType buildingType)
    {
        var tileSet = baseTileMap.TileSet;
        var atlas = tileSet.TileGetTexture(Constants.TileSet.BuildingsId);
        var tileSize = tileSet.AutotileGetSize(Constants.TileSet.BuildingsId);
        var tileCoord = buildingType.GetDefaultTileCoord();
        var region = new Rect2(tileCoord * tileSize, tileSize);

        var tileTexture = new AtlasTexture
        {
            Atlas = atlas,
            Region = region,
        };

        var button = toolButtonUi.Instance() as Button;
        button.GetNode<TextureRect>("Image").Texture = tileTexture;
        button.Connect("pressed", buildController, nameof(BuildController.SelectBuilding), [buildingType]);
        button.HintTooltip = $"{buildingType} ({(int)buildingType})";
        button.FocusMode = FocusModeEnum.None;
        tools.AddChild(button);
    }
}
