using OpenTK.Mathematics;
using Shiftless.Clockwork.Retro.Input;
using Shiftless.Clockwork.Retro.Mathematics;
using Shiftless.Clockwork.Retro.Rendering;

namespace Shiftless.Clockwork.Retro
{
    public abstract class GameBase
    {
        // Values
        public readonly Window Window;

        public readonly Time Time;

        public readonly Renderer Renderer;
        public readonly Tilemap Tilemap;
        public readonly Tileset Tileset;

        public readonly InputManager Input;

        private float _tickDelay;
        private float _tickTime;


        // Constructor
        public GameBase(GameSettings settings)
        {
            // Create the stuff
            Window = new Window(settings.WindowTitle, settings.WindowSize, settings.RenderFrequency);

            Time = new();

            Renderer = new Renderer(Window);
            Tilemap = new Tilemap();
            Tileset = new Tileset();

            Input = new(this);

            // Set some delay stuff
            _tickDelay = 1f / settings.TickRate;

            // Bind to some callbacks
            Window.OnLoad += Window_Load;
            Window.OnUpdateFrame += Window_Update;
            Window.OnRenderFrame += Window_Render;
        }

        protected void Run()
        {
            // Run the window
            Window.Run();
        }


        // Callbacks
        private void Window_Load()
        {
            Renderer.Initialize();
            Tilemap.Initialize();
            Tileset.Initialize();

            Load();
        }
        private void Window_Render(float deltaTime)
        {
            Renderer.Render();
        }
        private void Window_Update(float deltaTime)
        {
            Time.Update(deltaTime);
            Update(deltaTime);

            _tickTime += deltaTime;
            if (_tickTime > _tickDelay)
            {
                Tick();
                _tickTime = 0;
            }

            Tilemap.Update();
        }


        // Virtuals
        protected virtual void Load() { }
        protected virtual void Update(float deltaTime) { }
        protected virtual void Tick() { }
    }

    public struct GameSettings()
    {
        public string WindowTitle = "Shiftless.RetroEngine";
        public Vector2i WindowSize = new(1440, 810);

        public int TickRate = 20;

        public int RenderFrequency = 144;
    }
}
