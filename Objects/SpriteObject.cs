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

        public short MinX => _position.X;
        public short MinY => _position.Y;
        public short MaxX => (short)(_position.X + _mode.GetSize().Width);
        public short MaxY => (short)(_position.Y + _mode.GetSize().Height);


        // Interface Prop
        float ISpatialObject2D.MinX => MinX;
        float ISpatialObject2D.MinY => MinY;
        float ISpatialObject2D.MaxX => MaxX;
        float ISpatialObject2D.MaxY => MaxY;


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
