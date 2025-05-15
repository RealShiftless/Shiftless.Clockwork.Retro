using Shiftless.Clockwork.Retro.Mathematics;
using Shiftless.Common.Mathematics;

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


        // Properties
        public bool RequiresUpdate => _requiresUpdate;


        // Constructor
        internal SpriteObjectManager(GameBase game)
        {
            Game = game;

            // Initialize all sprite objects
            for (byte i = 0; i < SpriteObject.MAX; i++)
            {
                _spriteObjects[i] = new(this, i);
                _freeObjects.Enqueue(i);
            }
        }


        // Func
        public SpriteObject Activate(Vec2i16 position, SpriteMode mode, PaletteIndex palette, byte[] textures)
        {
            byte index = _freeObjects.Dequeue();
            _activeObjects.Add(index);

            _spriteObjects[index].Activate(position, mode, palette, textures);

            return _spriteObjects[index];
        }

        public void Deactivate(SpriteObject obj)
        {
            if (!obj.IsActive)
                throw new InvalidOperationException($"{nameof(SpriteObject)} was already inactive!");

            _activeObjects.Remove(obj.Id);
            _freeObjects.Enqueue(obj.Id);
        }
    }
}
