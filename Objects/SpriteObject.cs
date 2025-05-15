using Shiftless.Clockwork.Retro.Mathematics;
using Shiftless.Common.Mathematics;

namespace Shiftless.Clockwork.Retro.Objects
{
    public sealed class SpriteObject : ISpatialObject2D
    {
        // Constants
        public const byte MAX = 40;


        // Values
        public readonly SpriteObjectManager Manager;
        public readonly byte Id;

        private bool _isActive;
        private Vec2i16 _position;

        private SpriteMode _mode;
        private PaletteIndex _palette;

        private byte[] _textures = null!;


        // Properties
        public bool IsActive => _isActive;

        float ISpatialObject2D.MinX => _position.X;
        float ISpatialObject2D.MinY => _position.Y;

        float ISpatialObject2D.MaxX => _position.X + _mode.GetSize().Width;
        float ISpatialObject2D.MaxY => _position.Y + _mode.GetSize().Width;


        // Constructor
        internal SpriteObject(SpriteObjectManager manager, byte id)
        {
            Manager = manager;
            Id = id;
        }


        // Func
        internal void Activate(Vec2i16 position, SpriteMode mode, PaletteIndex palette, byte[] textures)
        {
            if (textures.Length != mode.GetSpriteCount())
                throw new ArgumentException($"Invallid textures length for sprite mode {mode}, expected {mode.GetSpriteCount()} got {textures.Length}!");

            _isActive = true;

            _position = position;
            _mode = mode;
            _palette = palette;
            _textures = textures;
        }

        public void Deactivate()
        {
            Manager.Deactivate(this);
        }

        internal void SetActive(bool activity) => _isActive = activity;
    }
}
