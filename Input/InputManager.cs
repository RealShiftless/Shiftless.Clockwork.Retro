using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Shiftless.Clockwork.Retro.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiftless.Clockwork.Retro.Input
{
    public sealed class InputManager
    {
        // Values
        private readonly GameBase _game;

        private float _deadZone = 0.5f;

        private List<Keybind> _keybinds = new List<Keybind>();

        private List<Keybind> _pressedBinds = [];

        public Keys Up = Keys.W;
        public Keys Left = Keys.A;
        public Keys Down = Keys.S;
        public Keys Right = Keys.D;


        // Properties
        public Vector2i MousePosition
        {
            get
            {
                Vector2i screenMousePos = (Vector2i)_game.Window.MouseState.Position;
                Point viewportOrigin = _game.Renderer.Viewport.Location;
                Size size = _game.Renderer.Viewport.Size;

                Vector2i translatedPos = screenMousePos - new Vector2i(viewportOrigin.X, viewportOrigin.Y);

                return new((int)(translatedPos.X / (float)size.Width * Renderer.NATIVE_WIDTH), Renderer.NATIVE_HEIGHT - (int)(translatedPos.Y / (float)size.Height * Renderer.NATIVE_HEIGHT) - 1);
            }
        }

        public float Horizontal => (_game.Window.KeyboardState.IsKeyDown(Right) ? 1f : 0f) - (_game.Window.KeyboardState.IsKeyDown(Left) ? 1f : 0f);
        public int HorizontalI
        {
            get
            {
                float hor = Horizontal;

                if (hor < -_deadZone)
                    return -1;
                if (hor > _deadZone) return 1;

                return 0;
            }
        }

        public float Vertical => (_game.Window.KeyboardState.IsKeyDown(Up) ? 1f : 0f) - (_game.Window.KeyboardState.IsKeyDown(Down) ? 1f : 0f);
        public int VerticalI
        {
            get
            {
                float vert = Vertical;

                if (vert < -_deadZone)
                    return -1;
                if (vert > _deadZone) return 1;

                return 0;
            }
        }


        // Constructor
        internal InputManager(GameBase game)
        {
            _game = game;

            // Window bindings
            _game.Window.OnKeyPressed += KeyPressed;
            _game.Window.OnKeyReleased += KeyReleased;
        }


        // Func
        internal void Update()
        {

        }


        // Key stuff
        private void KeyPressed(KeyboardKeyEventArgs e)
        {
            
        }
        private void KeyReleased(KeyboardKeyEventArgs e)
        {
            Debug.WriteLine(e.Key);
        }

        private bool TryGetKeybind(Keys key, [NotNullWhen(true)] out Keybind? bind)
        {
            bind = _keybinds.FirstOrDefault(c => c.Key == key);
            return bind != null;
        }
    }
}
