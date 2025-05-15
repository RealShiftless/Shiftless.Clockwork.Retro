namespace Shiftless.Clockwork.Retro.Mathematics
{
    [Flags]
    public enum TileTransform
    {
        Rotate0 = 0b0000,
        Rotate90 = 0b0001,
        Rotate180 = 0b0010,
        Rotate270 = 0b0011,
        FlipHorizontal = 0b0100,
        FlipVertical = 0b1000
    }
}
