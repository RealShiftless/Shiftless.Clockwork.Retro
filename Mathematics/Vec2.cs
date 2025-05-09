using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Shiftless.Clockwork.Retro.Mathematics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vec2 : IEquatable<Vec2>
    {
        // Values
        /// <summary>
        /// X Component.
        /// </summary>
        public float X;

        /// <summary>
        /// Y Component.
        /// </summary>
        public float Y;


        // Properties
        /// <summary>
        /// Defines a unit-length <see cref="Vec2"/> that points towards the X-axis.
        /// </summary>
        public static readonly Vec2 UnitX = new(1f, 0f);

        /// <summary>
        /// Defines a unit-length <see cref="Vec2"/> that points towards the Y-axis.
        /// </summary>
        public static readonly Vec2 UnitY = new(0f, 1f);

        /// <summary>
        /// Defines a <see cref="Vec2"/> with all components set to 0.
        /// </summary>
        public static readonly Vec2 Zero = new(0f, 0f);

        /// <summary>
        /// Defines a <see cref="Vec2"/> with all components set to 1.
        /// </summary>
        public static readonly Vec2 One = new(1f, 1f);

        /// <summary>
        /// Defines the size of the <see cref="Vec2"/> struct in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Unsafe.SizeOf<Vec2>();

        /// <summary>
        /// Gets the manhattan length of the vector.
        /// </summary>
        public readonly float ManhattanLength => MathF.Abs(X) + MathF.Abs(Y);

        /// <summary>
        /// Gets the squared Euclidean length of the vector.
        /// </summary>
        public readonly float EuclideanLengthSquared => (X * X) + (Y * Y);

        /// <summary>
        /// Gets the Euclidean length of the vector.
        /// </summary>
        public readonly float EuclideanLength => MathF.Sqrt((X * X) + (Y * Y));

        /// <summary>
        /// Gets the perpendicular vector to the right of this <see cref="Vec2"/>
        /// </summary>
        public readonly Vec2 PerpendicularRight => new(Y, -X);

        /// <summary>
        /// Gets the perpendicular vector to the left of this <see cref="Vec2"/>
        /// </summary>
        public readonly Vec2 PerpendicularLeft => new(-Y, X);


        // Indexer
        /// <summary>
        /// Gets or sets a component at the specified index in the <see cref="Vec2"/> (0 for X, 1 for Y).
        /// </summary>
        public float this[int index]
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
        public Vec2(float v) : this(v, v) { }

        public Vec2(float x, float y)
        {
            X = x;
            Y = y;
        }


        // Func

        /// <summary>
        /// Checks if the current <see cref="Vec2"/> is equal to another <see cref="Vec2"/>.
        /// </summary>
        public bool Equals(Vec2 other) => this == other;

        /// <summary>
        /// Returns a new <see cref="Vec2"/> with the absolute values of the components.
        /// </summary>
        public readonly Vec2 Abs() => new(MathF.Abs(X), MathF.Abs(Y));

        /// <summary>
        /// Returns a new <see cref="Vec2"/> with the absolute values of the components.
        /// </summary>
        public static Vec2 Abs(in Vec2 vec) => vec.Abs();

        /// <summary>
        /// Rounds the components of the vector to the nearest integer.
        /// </summary>
        public readonly Vec2 Round() => new(MathF.Round(X), MathF.Round(Y));

        /// <summary>
        /// Rounds the components of the provided vector to the nearest integer.
        /// </summary>
        public static Vec2 Round(in Vec2 vec) => vec.Round();


        /// <summary>
        /// Floors the components of the vector to the nearest integer.
        /// </summary>
        public readonly Vec2 Floor() => new(MathF.Floor(X), MathF.Floor(Y));

        /// <summary>
        /// Floors the components of the provided vector to the nearest integer.
        /// </summary>
        public static Vec2 Floor(in Vec2 vec) => vec.Floor();

        /// <summary>
        /// Ceils the components of the vector to the nearest integer.
        /// </summary>
        public readonly Vec2 Ceil() => new(MathF.Ceiling(X), MathF.Ceiling(Y));

        /// <summary>
        /// Ceils the components of the provided vector to the nearest integer.
        /// </summary>
        public static Vec2 Ceil(in Vec2 vec) => vec.Ceil();

        /// <summary>
        /// Returns a new <see cref="Vec2"/> with the minimum components between the current <see cref="Vec2"/> and the provided <see cref="Vec2"/>.
        /// </summary>
        public readonly Vec2 ComponentMin(in Vec2 v) => new(MathF.Min(X, v.X), MathF.Min(Y, v.Y));

        /// <summary>
        /// Returns a new <see cref="Vec2"/> with the minimum components between two <see cref="Vec2"/>s.
        /// </summary>
        public static Vec2 ComponentMin(in Vec2 v1, in Vec2 v2) => v1.ComponentMin(v2);

        /// <summary>
        /// Returns a new <see cref="Vec2"/> with the maximum components between the current <see cref="Vec2"/> and the provided <see cref="Vec2"/>.
        /// </summary>
        public readonly Vec2 ComponentMax(in Vec2 v) => new(MathF.Max(X, v.X), MathF.Max(Y, v.Y));

        /// <summary>
        /// Returns a new <see cref="Vec2"/> with the maximum components between two <see cref="Vec2"/>s.
        /// </summary>
        public static Vec2 ComponentMax(in Vec2 v1, in Vec2 v2) => v1.ComponentMax(v2);

        /// <summary>
        /// Calculates the dot product of the current <see cref="Vec2"/> and another <see cref="Vec2"/>.
        /// </summary>
        public readonly float Dot(in Vec2 other) => X * other.X + Y * other.Y;

        /// <summary>
        /// Calculates the dot product between two <see cref="Vec2"/>s.
        /// </summary>
        public static float Dot(in Vec2 v1, in Vec2 v2) => v1.Dot(v2);

        /// <summary>
        /// Calculates the cross product of the current <see cref="Vec2"/> and another <see cref="Vec2"/>.
        /// </summary>
        public readonly float Cross(in Vec2 other) => X * other.Y - Y * other.X;

        /// <summary>
        /// Calculates the cross product between two <see cref="Vec2"/>s.
        /// </summary>
        public static float Cross(in Vec2 v1, in Vec2 v2) => v1.Cross(v2);

        /// <summary>
        /// Clamps the components of the current <see cref="Vec2"/> between the specified minimum and maximum <see cref="Vec2"/>s.
        /// </summary>
        public readonly Vec2 Clamp(in Vec2 min, in Vec2 max) => new(
            Math.Clamp(X, min.X, max.X),
            Math.Clamp(Y, min.Y, max.Y)
        );

        /// <summary>
        /// Clamps the components of the provided <see cref="Vec2"/> between the specified minimum and maximum <see cref="Vec2"/>s.
        /// </summary>
        public static Vec2 Clamp(in Vec2 value, in Vec2 min, in Vec2 max) => value.Clamp(min, max);

        /// <summary>
        /// Returns a new <see cref="Vec2"/> with the X component set to the specified value, and the Y component unchanged.
        /// </summary>
        public readonly Vec2 WithX(float x) => new(x, Y);

        /// <summary>
        /// Returns a new <see cref="Vec2"/> with the Y component set to the specified value, and the X component unchanged.
        /// </summary>
        public readonly Vec2 WithY(float y) => new(X, y);

        /// <summary>
        /// Returns a new <see cref="Vec2"/> with the components negated (both X and Y set to their opposites).
        /// </summary>
        public readonly Vec2 Negate() => new(-X, -Y);

        /// <summary>
        /// Negates the components of the provided <see cref="Vec2"/> and returns the result.
        /// </summary>
        public static Vec2 Negate(in Vec2 v) => v.Negate();

        /// <summary>
        /// Returns a new <see cref="Vec2"/> with the minimum components between two <see cref="Vec2"/>s.
        /// </summary>
        public static Vec2 Min(in Vec2 v1, in Vec2 v2) => new(MathF.Min(v1.X, v2.X), MathF.Min(v1.Y, v2.Y));

        /// <summary>
        /// Returns a new <see cref="Vec2"/> with the maximum components between two <see cref="Vec2"/>s.
        /// </summary>
        public static Vec2 Max(in Vec2 v1, in Vec2 v2) => new(MathF.Max(v1.X, v2.X), MathF.Max(v1.Y, v2.Y));

        /// <summary>
        /// Interpolates between two <see cref="Vec2"/> values.
        /// </summary>
        /// <param name="a">The start value.</param>
        /// <param name="b">The end value.</param>
        /// <param name="t">Interpolation factor (typically between 0 and 1).</param>
        /// <returns>Interpolated <see cref="Vec2"/>.</returns>
        public static Vec2 Lerp(Vec2 a, Vec2 b, float t) => a + (b - a) * t;

        // Operators
        public static Vec2 operator +(Vec2 left, Vec2 right) => new(left.X + right.X, left.Y + right.Y);

        public static Vec2 operator -(Vec2 left, Vec2 right) => new(left.X - right.X, left.Y - right.Y);

        public static Vec2 operator *(Vec2 left, Vec2 right) => new(left.X * right.X, left.Y * right.Y);

        public static Vec2 operator /(Vec2 left, Vec2 right) => new(left.X / right.X, left.Y / right.Y);

        public static Vec2 operator *(Vec2 left, float right) => new(left.X * right, left.Y * right);

        public static Vec2 operator /(Vec2 left, float right) => new(left.X / right, left.Y / right);

        public static bool operator ==(Vec2 left, Vec2 right) => left.X == right.X && left.Y == right.Y;

        public static bool operator !=(Vec2 left, Vec2 right) => !(left == right);

        // Cast Operators
        public static implicit operator Vec2(Vector2 vec) => new(vec.X, vec.Y);
        public static implicit operator Vec2(Vector2i vec) => vec;
        public static implicit operator Vec2((float x, float y) vec) => new(vec.x, vec.y);


        // Internal methods
        // Static func
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ref float GetElementUnsafe(in Vec2 v, int index)
        {
            ref float address = ref Unsafe.AsRef(in v.X);
            return ref Unsafe.Add(ref address, index);
        }

        public override readonly string ToString() => $"({X}, {Y})";

        public override readonly bool Equals(object? obj)
        {
            if(obj is Vec2 v)
                return this == v;
            else if(obj is Vector2 vec)
                return this == vec;

            return false;
        }

        public override readonly int GetHashCode() => HashCode.Combine(X, Y);

    }
}