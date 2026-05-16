namespace ChemFactory.scripts;

public class Constants
{
    public const int TicksPerSecond = 20;
    public const float TickRate = 1.0f / TicksPerSecond;
    public const int PixelsPerTile = 16;
    public const float ItemSpeed = 2;

    public static class TileSet
    {
        public const int BaseId = 0;
        public const int OverlayId = 1;
        public const int PipesYOffset = 0;
        public const int BuildingsYOffset = 1;
    }
}
