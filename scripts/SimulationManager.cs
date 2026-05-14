using System.Collections.Generic;
using Godot;

public class SimulationManager : Node
{
    private const int TicksPerSecond = 5;
    private const float TickRate = 1.0f / TicksPerSecond;
    private float elapsedTime;
    private readonly Dictionary<Vector2, Belt> belts = new Dictionary<Vector2, Belt>();
    private TileMap tileMap;

    public override void _Ready()
    {
        tileMap = GetNode<TileMap>("/root/Game/TileMap");

        belts.Add(new Vector2(0, 0), new Belt());
        belts.Add(new Vector2(1, 0), new Belt());
        belts.Add(new Vector2(2, 0), new Belt());
        belts.Add(new Vector2(3, 0), new Belt());
        belts.Add(new Vector2(4, 0), new Belt());

        DrawBelts();
    }

    public override void _PhysicsProcess(float delta)
    {
        elapsedTime += delta;

        while (elapsedTime >= TickRate)
        {
            Tick();
            elapsedTime -= TickRate;
        }
    }

    private void Tick()
    {
        //foreach (var beltPair in belts)
        //{
        //    var position = beltPair.Key;
        //    var belt = beltPair.Value;

        //    var tileId = 0;

        //    tileMap.SetCell((int)position.x, (int)position.y, tileId);
        //}
    }

    private void DrawBelts()
    {
        foreach (var beltPair in belts)
        {
            var position = beltPair.Key;
            var belt = beltPair.Value;

            var tileId = 0;

            tileMap.SetCell((int)position.x, (int)position.y, tileId);
        }
    }
}
