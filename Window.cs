using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Shiftless.Clockwork.Retro
{
    public sealed class Window
    {
        // Values
        private GameWindow _glWindow;


        // Events
        internal event Action? OnLoad;
        internal event Action<float>? OnUpdateFrame;
        internal event Action<float>? OnRenderFrame;

        internal event Action<Window>? OnResized;

        internal event Action<KeyboardKeyEventArgs>? OnKeyPressed;
        internal event Action<KeyboardKeyEventArgs>? OnKeyReleased;

        internal WindowState _lastWindowState = WindowState.Normal;


        // Properties
        public Vector2i ClientSize
        {
            get => _glWindow.ClientSize;
            set => _glWindow.ClientSize = value;
        }

        // TODO: Make input manager and make this private :)
        public KeyboardState KeyboardState => _glWindow.KeyboardState;
        internal MouseState MouseState => _glWindow.MouseState;


        // Constructor
        internal Window(string title, Vector2i size, int updateFrequency)
        {
            _glWindow = new(new() { UpdateFrequency = updateFrequency }, new()
            {
                Title = title,
                ClientSize = size
            });

            _glWindow.Load += Load;
            _glWindow.RenderFrame += RenderFrame;
            _glWindow.UpdateFrame += UpdateFrame;

            _glWindow.Resize += Resized;

            _glWindow.KeyDown += KeyboardKeyPressed;
            _glWindow.KeyUp += KeyboardKeyReleased;
        }






        // Func
        internal void Run() => _glWindow.Run();


        private void Load()
        {
            GL.ClearColor(0, 0, 0, 1);
            OnLoad?.Invoke();
        }

        private void UpdateFrame(FrameEventArgs e)
        {
            if (KeyboardState.IsKeyPressed(Keys.F11))
            {
                WindowState nextState = _glWindow.WindowState == WindowState.Fullscreen ? _lastWindowState : WindowState.Fullscreen;
                _lastWindowState = _glWindow.WindowState;
                _glWindow.WindowState = nextState;
            }

            OnUpdateFrame?.Invoke((float)e.Time);
        }
        private void RenderFrame(FrameEventArgs e)
        {
            OnRenderFrame?.Invoke((float)e.Time);
            _glWindow.SwapBuffers();
        }

        private void Resized(ResizeEventArgs obj) => OnResized?.Invoke(this);

        private void KeyboardKeyReleased(KeyboardKeyEventArgs obj)
        {
            OnKeyPressed?.Invoke(obj);
        }

        private void KeyboardKeyPressed(KeyboardKeyEventArgs obj)
        {
            OnKeyPressed?.Invoke(obj);
        }
    }
}
