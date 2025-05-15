using Shiftless.Clockwork.Retro.Rendering;

namespace Shiftless.Clockwork.Retro.Objects
{
    public enum SpriteMode
    {
        Sprite1x1 = 0,
        Sprite2x2,
        Sprite3x3,
        Sprite4x4,
        Sprite1x2,
        Sprite2x1,
        Sprite2x3,
        Sprite3x2
    }

    public static class SpriteModeUtil
    {
        private const int SPRITE_1 = Tileset.TILE_PIXEL_AREA;
        private const int SPRITE_2 = Tileset.TILE_PIXEL_AREA * 2;
        private const int SPRITE_3 = Tileset.TILE_PIXEL_AREA * 3;
        private const int SPRITE_4 = Tileset.TILE_PIXEL_AREA * 4;

        public static (int Width, int Height) GetSize(this SpriteMode spriteMode) => spriteMode switch
        {
            SpriteMode.Sprite1x1 => (SPRITE_1, SPRITE_1),
            SpriteMode.Sprite2x2 => (SPRITE_2, SPRITE_2),
            SpriteMode.Sprite3x3 => (SPRITE_3, SPRITE_3),
            SpriteMode.Sprite4x4 => (SPRITE_4, SPRITE_4),
            SpriteMode.Sprite1x2 => (SPRITE_1, SPRITE_2),
            SpriteMode.Sprite2x1 => (SPRITE_2, SPRITE_1),
            SpriteMode.Sprite2x3 => (SPRITE_2, SPRITE_3),
            SpriteMode.Sprite3x2 => (SPRITE_3, SPRITE_2),

            _ => throw new ArgumentOutOfRangeException()
        };

        public static int GetSpriteCount(this SpriteMode spriteMode) => spriteMode switch
        {
            SpriteMode.Sprite1x1 => 1,
            SpriteMode.Sprite2x2 => 4,
            SpriteMode.Sprite3x3 => 9,
            SpriteMode.Sprite4x4 => 16,
            SpriteMode.Sprite1x2 => 2,
            SpriteMode.Sprite2x1 => 2,
            SpriteMode.Sprite2x3 => 6,
            SpriteMode.Sprite3x2 => 6,

            _ => throw new ArgumentOutOfRangeException()
        };
    }


}
