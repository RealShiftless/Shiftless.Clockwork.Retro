using Shiftless.Clockwork.Retro.Mathematics;
using Shiftless.Common.Mathematics;
using System;

namespace Shiftless.Clockwork.Retro.Objects
{
    public sealed class SpriteObjectManager
    {
        // Constants
        public const int BUCKET_SIZE = 16;


        // Values
        public readonly GameBase Game;

        private readonly SpriteObject[] _spriteObjects = new SpriteObject[SpriteObject.MAX];

        private readonly List<byte> _activeObjects = [];
        private readonly Queue<byte> _freeObjects = [];

        private bool _requiresUpdate;

        private Vec2i16 _offset;

        private byte[] _bucketIndices = new byte[SpriteObject.MAX];
        private ushort[,] _screenBuckets = new ushort[15, 9];


        // Properties
        public bool RequiresUpdate => _requiresUpdate;

        internal ushort[,] ScreenBuckets => _screenBuckets;


        // Constructor
        internal SpriteObjectManager(GameBase game)
        {
            // Values
            Game = game;

            // Initialize all sprite objects
            for (byte i = 0; i < SpriteObject.MAX; i++)
            {
                _spriteObjects[i] = new(this, i);
                _freeObjects.Enqueue(i);
            }
        }


        // Func
        public SpriteObject ActivateDirect(byte id, Vec2i16 position, SpriteMode mode, PaletteIndex palette, byte[] textures)
        {
            _activeObjects.Add(id);

            _spriteObjects[id].SetActive(true);
            _spriteObjects[id].Activate(position, mode, palette, textures);

            return _spriteObjects[id];
        }
        public SpriteObject Activate(byte id, Vec2i16 position, SpriteMode mode, PaletteIndex palette, byte[] textures)
        {
            if (_spriteObjects[id].IsActive)
                throw new InvalidOperationException($"{nameof(SpriteObject)} was already active!");

            return ActivateDirect(id, position, mode, palette, textures);
        }
        public SpriteObject ActivateNext(Vec2i16 position, SpriteMode mode, PaletteIndex palette, byte[] textures)
        {
            if (_freeObjects.Count == 0)
                throw new OutOfMemoryException("All sprite objects where active!");

            return ActivateDirect(_freeObjects.Dequeue(), position, mode, palette, textures);
        }

        public void Deactivate(SpriteObject obj)
        {
            if (!obj.IsActive)
                throw new InvalidOperationException($"{nameof(SpriteObject)} was already inactive!");

            _activeObjects.Remove(obj.Id);
            _freeObjects.Enqueue(obj.Id);

            obj.SetActive(false);
        }
    }
}
