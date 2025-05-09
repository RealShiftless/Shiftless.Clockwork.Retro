using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Shiftless.Clockwork.Retro.Mathematics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vec2i16(short x, short y) : IEquatable<Vec2i16>
    {
        // Values
        /// <summary>
        /// X Component.
        /// </summary>
        public short X = x;

        /// <summary>
        /// Y Component.
        /// </summary>
        public short Y = y;


        // Properties
        /// <summary>
        /// Defines a unit-length <see cref="Vec2i16"/> that points towards the X-axis.
        /// </summary>
        public static readonly Vec2i16 UnitX = new(1, 0);

        /// <summary>
        /// Defines a unit-length <see cref="Vec2i16"/> that points towards the Y-axis.
        /// </summary>
        public static readonly Vec2i16 UnitY = new(0, 1);

        /// <summary>
        /// Defines a <see cref="Vec2i16"/> with all components set to 0.
        /// </summary>
        public static readonly Vec2i16 Zero = new(0, 0);

        /// <summary>
        /// Defines a <see cref="Vec2i16"/> with all components set to 1.
        /// </summary>
        public static readonly Vec2i16 One = new(1, 1);

        /// <summary>
        /// Defines the size of the <see cref="Vec2i16"/> struct in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Unsafe.SizeOf<Vec2i16>();

        /// <summary>
        /// Gets the manhattan length of the vector.
        /// </summary>
        public readonly int ManhattanLength => Math.Abs(X) + Math.Abs(Y);

        /// <summary>
        /// Gets the squared euclidean length of the vector.
        /// </summary>
        public readonly int EuclideanLengthSquared => (X * X) + (Y * Y);

        /// <summary>
        /// Gets the euclidean length of the vector.
        /// </summary>
        public readonly float EuclideanLength => MathF.Sqrt((X * X) + (Y * Y));

        /// <summary>
        /// Gets the perpendicular vector to the right of this <see cref="Vec2i16"/>
        /// </summary>
        public readonly Vec2i16 PerpendicularRight => new(Y, (short)(-X));

        /// <summary>
        /// Gets the perpendicular vector to the left of this <see cref="Vec2i16"/>
        /// </summary>
        public readonly Vec2i16 PerpendicularLeft => new((short)(-Y), X);


        // Indexer
        /// <summary>
        /// Gets or sets a component at the specified index in the <see cref="Vec2i16"/>.
        /// </summary>
        /// <param name="index">The index of the component (0 for X, 1 for Y).</param>
        /// <returns>The component at the specified index.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown when the index is outside the valid range (0 <= index < 2).</exception>
        public short this[int index]
        {
            readonly get
            {
#if DEBUG
                if ((uint)index >= 2)
                    throw new IndexOutOfRangeException($"Tried to access 2 dimensional vector at {index}");
#endif

                return GetElementUnsafe(in this, index);
            }
            set
            {
#if DEBUG
                if ((uint)index >= 2)
                    throw new IndexOutOfRangeException($"Tried to access 2 dimensional vector at {index}");
#endif

                GetElementUnsafe(this, index) = value;
            }
        }


        // Constructor
        public Vec2i16(short v) : this(v, v) { }


        // Func

        /// <summary>
        /// Checks if the current <see cref="Vec2i16"/> is equal to another <see cref="Vec2i16"/>.
        /// </summary>
        /// <param name="other">The other <see cref="Vec2i16"/> to compare with.</param>
        /// <returns>True if the <see cref="Vec2i16"/>s are equal; otherwise, false.</returns>
        public readonly bool Equals(Vec2i16 other) => this == other;

        /// <summary>
        /// Returns a new <see cref="Vec2i16"/> with the absolute values of the components.
        /// </summary>
        /// <returns>A new <see cref="Vec2i16"/> with absolute components.</returns>
        public readonly Vec2i16 Abs() => new(Math.Abs(X), Math.Abs(Y));

        /// <summary>
        /// Returns a new <see cref="Vec2i16"/> with the absolute values of the components.
        /// </summary>
        /// <param name="vec">The <see cref="Vec2i16"/> to operate on.</param>
        /// <returns>A new <see cref="Vec2i16"/> with absolute components.</returns>
        public static Vec2i16 Abs(in Vec2i16 vec) => vec.Abs();

        /// <summary>
        /// Returns a new <see cref="Vec2i16"/> with the minimum components between the current <see cref="Vec2i16"/> and the provided <see cref="Vec2i16"/>.
        /// </summary>
        /// <param name="v">The <see cref="Vec2i16"/> to compare with.</param>
        /// <returns>A new <see cref="Vec2i16"/> with minimum components.</returns>
        public readonly Vec2i16 ComponentMin(in Vec2i16 v) => new(Math.Min(X, v.X), Math.Min(Y, v.Y));

        /// <summary>
        /// Returns a new <see cref="Vec2i16"/> with the minimum components between two <see cref="Vec2i16"/>s.
        /// </summary>
        /// <param name="v1">The first <see cref="Vec2i16"/> to compare.</param>
        /// <param name="v2">The second <see cref="Vec2i16"/> to compare.</param>
        /// <returns>A new <see cref="Vec2i16"/> with minimum components.</returns>
        public static Vec2i16 ComponentMin(in Vec2i16 v1, in Vec2i16 v2) => v1.ComponentMin(v2);

        /// <summary>
        /// Returns a new <see cref="Vec2i16"/> with the maximum components between the current <see cref="Vec2i16"/> and the provided <see cref="Vec2i16"/>.
        /// </summary>
        /// <param name="v">The <see cref="Vec2i16"/> to compare with.</param>
        /// <returns>A new <see cref="Vec2i16"/> with maximum components.</returns>
        public readonly Vec2i16 ComponentMax(in Vec2i16 v) => new(Math.Max(X, v.X), Math.Max(Y, v.Y));

        /// <summary>
        /// Returns a new <see cref="Vec2i16"/> with the maximum components between two <see cref="Vec2i16"/>s.
        /// </summary>
        /// <param name="v1">The first <see cref="Vec2i16"/> to compare.</param>
        /// <param name="v2">The second <see cref="Vec2i16"/> to compare.</param>
        /// <returns>A new <see cref="Vec2i16"/> with maximum components.</returns>
        public static Vec2i16 ComponentMax(in Vec2i16 v1, in Vec2i16 v2) => v1.ComponentMax(v2);

        /// <summary>
        /// Calculates the dot product of the current <see cref="Vec2i16"/> and another <see cref="Vec2i16"/>.
        /// </summary>
        /// <param name="other">The other <see cref="Vec2i16"/> to calculate the dot product with.</param>
        /// <returns>The dot product of the two <see cref="Vec2i16"/>s.</returns>
        public readonly int Dot(in Vec2i16 other) => X * other.X + Y * other.Y;

        /// <summary>
        /// Calculates the dot product between two <see cref="Vec2i16"/>s.
        /// </summary>
        /// <param name="v1">The first <see cref="Vec2i16"/>.</param>
        /// <param name="v2">The second <see cref="Vec2i16"/>.</param>
        /// <returns>The dot product of the two <see cref="Vec2i16"/>s.</returns>
        public static int Dot(in Vec2i16 v1, in Vec2i16 v2) => v1.Dot(v2);

        /// <summary>
        /// Calculates the cross product of the current <see cref="Vec2i16"/> and another <see cref="Vec2i16"/>.
        /// </summary>
        /// <param name="other">The other <see cref="Vec2i16"/> to calculate the cross product with.</param>
        /// <returns>The cross product of the two <see cref="Vec2i16"/>s.</returns>
        public readonly int Cross(in Vec2i16 other) => X * other.Y - Y * other.X;

        /// <summary>
        /// Calculates the cross product between two <see cref="Vec2i16"/>s.
        /// </summary>
        /// <param name="v1">The first <see cref="Vec2i16"/>.</param>
        /// <param name="v2">The second <see cref="Vec2i16"/>.</param>
        /// <returns>The cross product of the two <see cref="Vec2i16"/>s.</returns>
        public static int Cross(in Vec2i16 v1, in Vec2i16 v2) => v1.Cross(v2);

        /// <summary>
        /// Clamps the components of the current <see cref="Vec2i16"/> between the specified minimum and maximum <see cref="Vec2i16"/>s.
        /// </summary>
        /// <param name="min">The minimum bounds for the <see cref="Vec2i16"/>.</param>
        /// <param name="max">The maximum bounds for the <see cref="Vec2i16"/>.</param>
        /// <returns>A new <see cref="Vec2i16"/> clamped within the specified bounds.</returns>
        public readonly Vec2i16 Clamp(in Vec2i16 min, in Vec2i16 max) => new(
            Math.Clamp(X, min.X, max.X),
            Math.Clamp(Y, min.Y, max.Y)
        );

        /// <summary>
        /// Clamps the components of the provided <see cref="Vec2i16"/> between the specified minimum and maximum <see cref="Vec2i16"/>s.
        /// </summary>
        /// <param name="value">The <see cref="Vec2i16"/> to clamp.</param>
        /// <param name="min">The minimum bounds for the <see cref="Vec2i16"/>.</param>
        /// <param name="max">The maximum bounds for the <see cref="Vec2i16"/>.</param>
        /// <returns>A new <see cref="Vec2i16"/> clamped within the specified bounds.</returns>
        public static Vec2i16 Clamp(in Vec2i16 value, in Vec2i16 min, in Vec2i16 max) => value.Clamp(min, max);

        /// <summary>
        /// Returns a new <see cref="Vec2i16"/> with the X component set to the specified value, and the Y component unchanged.
        /// </summary>
        /// <param name="x">The new X component value.</param>
        /// <returns>A new <see cref="Vec2i16"/> with the updated X value.</returns>
        public readonly Vec2i16 WithX(short x) => new(x, Y);

        /// <summary>
        /// Returns a new <see cref="Vec2i16"/> with the Y component set to the specified value, and the X component unchanged.
        /// </summary>
        /// <param name="y">The new Y component value.</param>
        /// <returns>A new <see cref="Vec2i16"/> with the updated Y value.</returns>
        public readonly Vec2i16 WithY(short y) => new(X, y);

        /// <summary>
        /// Returns a new <see cref="Vec2i16"/> with the components negated (both X and Y set to their opposites).
        /// </summary>
        /// <returns>A new <see cref="Vec2i16"/> with negated components.</returns>
        public readonly Vec2i16 Negate() => new((short)(-X), (short)(-Y));

        /// <summary>
        /// Negates the components of the provided <see cref="Vec2i16"/>.
        /// </summary>
        /// <param name="v">The <see cref="Vec2i16"/> to negate.</param>
        /// <returns>A new <see cref="Vec2i16"/> with negated components.</returns>
        public static Vec2i16 Negate(in Vec2i16 v) => v.Negate();

        /// <summary>
        /// Returns a new <see cref="Vec2i16"/> with the minimum components between two <see cref="Vec2i16"/>s.
        /// </summary>
        /// <param name="v1">The first <see cref="Vec2i16"/>.</param>
        /// <param name="v2">The second <see cref="Vec2i16"/>.</param>
        /// <returns>A new <see cref="Vec2i16"/> with minimum components.</returns>
        public static Vec2i16 Min(in Vec2i16 v1, in Vec2i16 v2) => new(Math.Min(v1.X, v2.X), Math.Min(v1.Y, v2.Y));

        /// <summary>
        /// Returns a new <see cref="Vec2i16"/> with the maximum components between two <see cref="Vec2i16"/>s.
        /// </summary>
        /// <param name="v1">The first <see cref="Vec2i16"/>.</param>
        /// <param name="v2">The second <see cref="Vec2i16"/>.</param>
        /// <returns>A new <see cref="Vec2i16"/> with maximum components.</returns>
        public static Vec2i16 Max(in Vec2i16 v1, in Vec2i16 v2) => new(Math.Max(v1.X, v2.X), Math.Max(v1.Y, v2.Y));


        // Override Func

        /// <summary>
        /// Returns a hash code for this <see cref="Vec2i16"/>.
        /// </summary>
        /// <returns>The hash code for the <see cref="Vec2i16"/>.</returns>
        public readonly override int GetHashCode() => HashCode.Combine(X, Y);

        /// <summary>
        /// Returns a string representation of the <see cref="Vec2i16"/>.
        /// </summary>
        /// <returns>A string representing the <see cref="Vec2i16"/> in the format "{ x: X, y: Y }".</returns>
        public readonly override string ToString() => $"{{ x: {X}, y: {Y} }}";

        /// <summary>
        /// Checks if this <see cref="Vec2i16"/> is equal to another object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if the object is equal to this <see cref="Vec2i16"/>; otherwise, false.</returns>
        public readonly override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is Vec2i16 vec2i)
                return this == vec2i;
            if (obj is Vector2i vector2i)
                return this == vector2i;

            return false;
        }


        // Operator Func

        /// <summary>
        /// Adds two <see cref="Vec2i16"/>s together and returns the result.
        /// </summary>
        /// <param name="a">The first <see cref="Vec2i16"/>.</param>
        /// <param name="b">The second <see cref="Vec2i16"/>.</param>
        /// <returns>The resulting <see cref="Vec2i16"/> from the addition.</returns>
        public static Vec2i16 operator +(Vec2i16 a, Vec2i16 b) => new((short)(a.X + b.X), (short)(a.Y + b.Y));

        /// <summary>
        /// Subtracts the second <see cref="Vec2i16"/> from the first <see cref="Vec2i16"/> and returns the result.
        /// </summary>
        /// <param name="a">The first <see cref="Vec2i16"/>.</param>
        /// <param name="b">The second <see cref="Vec2i16"/>.</param>
        /// <returns>The resulting <see cref="Vec2i16"/> from the subtraction.</returns>
        public static Vec2i16 operator -(Vec2i16 a, Vec2i16 b) => new((short)(a.X - b.X), (short)(a.Y - b.Y));

        /// <summary>
        /// Multiplies two <see cref="Vec2i16"/>s component-wise and returns the result.
        /// </summary>
        /// <param name="a">The first <see cref="Vec2i16"/>.</param>
        /// <param name="b">The second <see cref="Vec2i16"/>.</param>
        /// <returns>The resulting <see cref="Vec2i16"/> from the multiplication.</returns>
        public static Vec2i16 operator *(Vec2i16 a, Vec2i16 b) => new((short)(a.X * b.X), (short)(a.Y * b.Y));

        /// <summary>
        /// Multiplies the <see cref="Vec2i16"/> by a scalar and returns the result.
        /// </summary>
        /// <param name="a">The <see cref="Vec2i16"/>.</param>
        /// <param name="scalar">The scalar value.</param>
        /// <returns>The resulting <see cref="Vec2i16"/> from the multiplication.</returns>
        public static Vec2i16 operator *(Vec2i16 a, int scalar) => new((short)(a.X * scalar), (short)(a.Y * scalar));

        /// <summary>
        /// Multiplies the scalar by the <see cref="Vec2i16"/> and returns the result.
        /// </summary>
        /// <param name="scalar">The scalar value.</param>
        /// <param name="a">The <see cref="Vec2i16"/>.</param>
        /// <returns>The resulting <see cref="Vec2i16"/> from the multiplication.</returns>
        public static Vec2i16 operator *(int scalar, Vec2i16 a) => a * scalar;

        /// <summary>
        /// Compares two <see cref="Vec2i16"/> instances for equality.
        /// </summary>
        /// <param name="a">The first <see cref="Vec2i16"/>.</param>
        /// <param name="b">The second <see cref="Vec2i16"/>.</param>
        /// <returns><c>true</c> if the components of <see cref="a"/> and <see cref="b"/> are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(Vec2i16 a, Vec2i16 b) => a.X == b.X && a.Y == b.Y;

        /// <summary>
        /// Compares two <see cref="Vec2i16"/> instances for inequality.
        /// </summary>
        /// <param name="a">The first <see cref="Vec2i16"/>.</param>
        /// <param name="b">The second <see cref="Vec2i16"/>.</param>
        /// <returns><c>true</c> if the components of <see cref="a"/> and <see cref="b"/> are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(Vec2i16 a, Vec2i16 b) => a.X != b.X || a.Y != b.Y;

        /// <summary>
        /// Implicitly converts from a <see cref="Vector2i"/> to a <see cref="Vec2i16"/>.
        /// </summary>
        /// <param name="v">The <see cref="Vector2i"/> to convert.</param>
        /// <returns>The corresponding <see cref="Vec2i16"/>.</returns>
        public static implicit operator Vec2i16(Vector2i v) => new((short)(v.X), (short)(v.Y));

        /// <summary>
        /// Implicitly converts from a <see cref="Vector2i"/> to a <see cref="Vec2i16"/>.
        /// </summary>
        /// <param name="v">The <see cref="Vector2i"/> to convert.</param>
        /// <returns>The corresponding <see cref="Vec2i16"/>.</returns>
        public static explicit operator Vec2i16(Vec2 v) => new((short)v.X, (short)v.Y);

        /// <summary>
        /// Explicitly converts from a <see cref="Vector2"/> to a <see cref="Vec2i16"/>.
        /// </summary>
        /// <param name="v">The <see cref="Vector2"/> to convert.</param>
        /// <returns>The corresponding <see cref="Vec2i16"/>.</returns>
        public static explicit operator Vec2i16(Vector2 v) => (Vector2i)v;

        /// <summary>
        /// Implicitly converts from a tuple <c>(int x, int y)</c> to a <see cref="Vec2i16"/>.
        /// </summary>
        /// <param name="v">The tuple to convert.</param>
        /// <returns>The corresponding <see cref="Vec2i16"/>.</returns>
        public static implicit operator Vec2i16((short x, short y) v) => new(v.x, v.y);


        // Static func
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ref short GetElementUnsafe(in Vec2i16 v, int index)
        {
            ref short address = ref Unsafe.AsRef(in v.X);
            return ref Unsafe.Add(ref address, index);
        }
    }
}
