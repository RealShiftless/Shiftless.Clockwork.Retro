using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Shiftless.Clockwork.Retro.Input
{
    public sealed class Keybind
    {
        // Values
        private bool _pressed;
        private Keys? _key;


        // Properties
        public bool IsPressed => _pressed;
        public Keys? Key => _key;


        // Events
        public event Action<Keybind>? OnPressed;
        public event Action<Keybind>? OnReleased;


        // Constructor
        public Keybind(Keys? key = null, Action<Keybind>? onPressed = null, Action<Keybind>? onReleased = null)
        {
            _key = key;

            OnPressed = onPressed;
            OnReleased = onReleased;
        }


        // Func
        internal void Press()
        {
            _pressed = true;
            OnPressed?.Invoke(this);
        }
        internal void Release()
        {
            _pressed = false;
            OnReleased?.Invoke(this);
        }
    }
}
