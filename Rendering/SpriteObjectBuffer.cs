using Shiftless.Clockwork.Retro.Objects;

namespace Shiftless.Clockwork.Retro.Rendering
{
    internal sealed class SpriteObjectBuffer
    {
        // Constants
        public const int BUCKET_SIZE = 16;


        // Values
        public readonly SpriteObjectManager Manager;

        private ushort[,] _buckets = new ushort[(int)MathF.Ceiling((float)Renderer.NATIVE_WIDTH / BUCKET_SIZE), (int)MathF.Ceiling((float)Renderer.NATIVE_HEIGHT / BUCKET_SIZE)];

        private int _objectsHandle;
        private int _bucketsHandle;


        // Constructor
        internal SpriteObjectBuffer(SpriteObjectManager spriteObjectManager)
        {
            Manager = spriteObjectManager;
        }
    }
}
