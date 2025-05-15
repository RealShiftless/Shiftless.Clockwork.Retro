using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics.CodeAnalysis;

namespace Shiftless.Clockwork.Retro.Input
{
    public readonly struct KeyCombo
    {
        // Values
        public readonly bool Control;
        public readonly bool Alt;
        public readonly bool Shift;

        public readonly Keys Key;


        // Constructor
        public KeyCombo(bool control, bool alt, bool shift, Keys key)
        {
            Control = control;
            Alt = alt;
            Shift = shift;
            Key = key;
        }


        // Func
        public override string ToString() => $"{(Control ? "Ctrl+" : "")}{(Alt ? "Alt+" : "")}{(Shift ? "Shift+" : "")}{Key}";

        public override int GetHashCode() => HashCode.Combine(Control, Alt, Shift, Key);
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is KeyCombo combo)
                return this == combo;

            return false;
        }


        // Operators
        public static bool operator ==(KeyCombo left, KeyCombo right) => left.Control == right.Control && left.Alt == right.Alt && left.Shift == right.Shift && left.Key == right.Key;
        public static bool operator !=(KeyCombo left, KeyCombo right) => !(left == right);
    }
}
