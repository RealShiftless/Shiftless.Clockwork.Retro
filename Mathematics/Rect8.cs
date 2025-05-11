using OpenTK.Graphics.ES20;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Shiftless.Clockwork.Retro.Mathematics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Rect8(int x, int y, int width, int height)
    {
        // Values
        private byte _x = (byte)x;
        private byte _y = (byte)y;
        private byte _width = (byte)width;
        private byte _height = (byte)height;


        // Properties
        public int X
        {
            readonly get => _x;
            set => _x = (byte)value;
        }
        public int Y
        {
            readonly get => _y;
            set => _y = (byte)value;
        }

        public int Width
        {
            readonly get => _width;
            set => _width = (byte)value;
        }
        public int Height
        {
            readonly get => _height;
            set => _height = (byte)value;
        }

        public readonly byte Left => _x;
        public readonly byte Bottom => _y;
        public readonly byte Right => (byte)(_x + _width);
        public readonly byte Top => (byte)(_y + _height);

        public readonly bool IsOutOfBounds
        {
            get
            {
                int maxX = _x + _width;
                int maxY = _y + _height;

                return maxX > byte.MaxValue || maxY > byte.MaxValue;
            }
        }

        private readonly OOBState OutOfBounds
        {
            get
            {
                int maxX = _x + _width;
                int maxY = _y + _height;

                OOBState state = OOBState.None;

                if(maxX > byte.MaxValue)
                    state |= OOBState.Horizontal;

                if(maxY > byte.MaxValue)
                    state |= OOBState.Vertical;

                return state;
            }
        }


        // Func
        // I made this a sepperate func first for some functionality but that isn't needed no more, but I kinda like this better so i roll widdit
        private readonly bool Contains(int x, int y, OOBState state) => state switch
        {
            // So like, I need to check differently based on if the rect wraps around.
            // If the rect wraps around it means we need to do an or check instead of an and

            // Normal check
            OOBState.None => x >= Left && x < Right && y >= Bottom && y < Top,

            // Fully wrapped check
            OOBState.Horizontal | OOBState.Vertical => (x >= Left || x < Right) && (y >= Left || y < Right),

            // Just do it horizontally
            OOBState.Horizontal => (x >= Left || x < Right) && y >= Bottom && y < Top,

            // And ofc do it vertically aswell :)
            OOBState.Vertical => x >= Left && x < Right && (y >= Left || y < Right),

            // God only knows how we got here, if we got here :(
            _ => throw new Exception("OOBState went unhandled?!?!"),
        };

        public readonly bool Contains(int x, int y)
        {
            x = (byte)x;
            y = (byte)y;

            return Contains(x, y, OutOfBounds);
        }

        public readonly bool Contains(Point8 point) => Contains(point.X, point.Y);

        public readonly bool Overlaps(Rect8 rect)
        {
            OOBState state = OutOfBounds;

            return AxisOverlaps(Left, Right, rect.Left, rect.Right) &&
                   AxisOverlaps(Bottom, Top, rect.Bottom, rect.Top);
        }

        private static bool AxisOverlaps(byte aStart, byte aEnd, byte bStart, byte bEnd)
        {
            // Check if A wraps
            bool aWraps = aEnd < aStart;
            // Check if B wraps
            bool bWraps = bEnd < bStart;

            if (!aWraps && !bWraps)
            {
                // Normal case
                return aStart < bEnd && aEnd > bStart;
            }
            else if (aWraps && bWraps)
            {
                // Both wrap: guaranteed overlap unless they’re totally disjoint
                return true;
            }
            else if (aWraps)
            {
                // A wraps, B doesn't
                return aStart < bEnd || aEnd > bStart;
            }
            else // bWraps
            {
                // B wraps, A doesn't
                return bStart < aEnd || bEnd > aStart;
            }
        }

        public readonly Rect8 Translate(int x, int y) => new(X + x, Y + y, width, height);
        public readonly Rect8 Translate(Point8 point) => Translate(point.X, point.Y);
        public readonly Rect8 Translate(Direction direction, int amount) => direction switch
        {
            Direction.Up => Translate(0, amount),
            Direction.Left => Translate(-amount, 0),
            Direction.Down => Translate(0, -amount),
            Direction.Right => Translate(amount, 0),
            _ => Translate(0, 0)
        };
        public readonly Rect8 Translate(Direction direction) => Translate(direction, 1);

        public readonly int DistanceFrom(Rect8 rect, Axis axis)
        {
            if(axis == Axis.Horizontal)
            {
                if (Right < rect.Left)
                    return rect.Left - Right;
                else if(Left > rect.Right)
                    return Left - rect.Right;
            }
            else
            {
                if(Top < rect.Bottom)
                    return rect.Bottom - Top;
                else if(Bottom > rect.Top)
                    return Bottom - rect.Top;
            }

            return 0;
        }
        


        // Enum
        [Flags]
        private enum OOBState
        {
            None       = 0b00,
            Horizontal = 0b01,
            Vertical   = 0b10
        }
    }


}
