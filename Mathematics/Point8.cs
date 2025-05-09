using Shiftless.Common.Mathematics;
using Shiftless.Common.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Shiftless.Clockwork.Retro.Mathematics
{
    /// <summary>
    /// Defines a 2 byte point in space.
    /// </summary>
    /// <param name="x">The x component.</param>
    /// <param name="y">The y component.</param>
    [StructLayout(LayoutKind.Sequential)]
    public struct Point8(int x, int y) : IByteSerializable, IEquatable<Point8>
    {
        // Defaults
        public static Point8 Zero = new Point8(0);
        public static Point8 One = new Point8(1);


        // Values
        private byte _x = (byte)x;
        private byte _y = (byte)y;


        // Properties
        /// <summary>
        /// Defines the size of the <see cref="Point8"/> struct in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Unsafe.SizeOf<Point8>();

        /// <summary>
        /// X Component.
        /// </summary>
        public int X
        {
            readonly get => _x;
            set => _x = (byte)value;
        }
        /// <summary>
        /// Y Component.
        /// </summary>
        public int Y
        {
            readonly get => _y;
            set => _y = (byte)value;
        }


        // Constructors
        public Point8(byte v) : this(v, v) { }
        public Point8() : this(0, 0) { }


        // Math func
        public readonly Point8 Add(int x, int y) => new((byte)(X + x), (byte)(Y + y));
        public readonly Point8 Add(in Point8 point) => Add(point.X, point.Y);

        public readonly Point8 Translate(Direction direction, int amount) => direction switch
        {
            Direction.Up => Add(0, amount),
            Direction.Right => Add(amount, 0),
            Direction.Down => Add(0, -amount),
            Direction.Left => Add(-amount, 0),
            _ => Add(0, 0)
        };

        public readonly Point8 Subtract(int x, int y) => new((byte)(X - x), (byte)(Y - y));
        public readonly Point8 Subtract(in Point8 point) => Subtract(point.X, point.Y);

        public readonly Point8 Multiply(int v) => new(X * v, Y * v);
        public readonly Point8 Multiply(in Point8 point) => new(X * point.X, Y * point.Y);
        public readonly Point8 Multiply(int x, int y) => new(X * x, Y * y);

        public readonly Point8 Divide(int v) => new(X / v, Y / v);

        public readonly Point8 Mod(int mX, int mY) => new(MHelp.Mod(X, mX), MHelp.Mod(Y, mY));
        public readonly Point8 Mod(in Point8 m) => Mod(m.X, m.Y);
        public readonly Point8 Mod(int m) => Mod(m, m);

        public readonly Point8 WithX(int x) => new((byte)x, Y);
        public readonly Point8 WithY(int y) => new(X, (byte)y);

        // Overrides
        public readonly override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is Point8 pV)
                return Equals(pV);
            else if(obj is (int x, int y))
                return X == x && Y == y;

            return false;
        }
        public readonly override int GetHashCode() => X << 8 | Y;

        public readonly override string ToString() => $"{{ x: {X}, y: {Y} }}";


        // Operators
        public static Point8 operator +(Point8 point1, Point8 point2) => point1.Add(point2);
        public static Point8 operator +(Point8 point1, Vec2i point2) => new(point1.X + point2.X, point1.Y + point2.Y);
        public static Point8 operator -(Point8 point1, Point8 point2) => point1.Subtract(point2);
        public static Point8 operator -(Point8 point1, Vec2i point2) => new(point1.X - point2.X, point1.Y - point2.Y);

        public static Point8 operator *(Point8 point1, int v) => point1.Multiply(v);
        public static Point8 operator *(Point8 point1, Point8 point2) => point1.Multiply(point2);
        public static Point8 operator *(Point8 point1, (int x, int y) v) => point1.Multiply(v.x, v.y);

        public static Point8 operator /(Point8 point1, int v) => point1.Divide(v);

        public static bool operator ==(Point8 point1, Point8 point2) => point1.X == point2.X && point1.Y == point2.Y;
        public static bool operator !=(Point8 point1, Point8 point2) => point1.X != point2.X || point1.Y != point2.Y;


        // Cast Operator
        public static explicit operator Point8(Vec2i value) => new(value.X, value.Y);


        // Interface
        public readonly bool Equals(Point8 other) => this == other;


        // Serialization
        readonly byte[] IByteSerializable.Serialize() => [_x, _y];
        void IByteSerializable.Deserialize(ByteStream stream)
        {
            X = stream.Read();
            Y = stream.Read();
        }

        
    }
}
