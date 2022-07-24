#if REAL_T_IS_DOUBLE
using real_t = System.Double;
#else
using real_t = System.Single;
#endif
using System;
using System.Runtime.InteropServices;

namespace Godot
{
    /// <summary>
    /// 2-element structure that can be used to represent 2D grid coordinates or pairs of integers.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2i : IEquatable<Vector2i>
    {
        /// <summary>
        /// Enumerated index values for the axes.
        /// Returned by <see cref="MaxAxisIndex"/> and <see cref="MinAxisIndex"/>.
        /// </summary>
        public enum Axis
        {
            /// <summary>
            /// The vector's X axis.
            /// </summary>
            X = 0,
            /// <summary>
            /// The vector's Y axis.
            /// </summary>
            Y
        }

        /// <summary>
        /// The vector's X component. Also accessible by using the index position <c>[0]</c>.
        /// </summary>
        public int x;

        /// <summary>
        /// The vector's Y component. Also accessible by using the index position <c>[1]</c>.
        /// </summary>
        public int y;

        /// <summary>
        /// Access vector components using their index.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown when the given the <paramref name="index"/> is not 0 or 1.
        /// </exception>
        /// <value>
        /// <c>[0]</c> is equivalent to <see cref="x"/>,
        /// <c>[1]</c> is equivalent to <see cref="y"/>.
        /// </value>
        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        return;
                    case 1:
                        y = value;
                        return;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Helper method for deconstruction into a tuple.
        /// </summary>
        public void Deconstruct(out int x, out int y)
        {
            x = this.x;
            y = this.y;
        }

        /// <summary>
        /// Returns a new vector with all components in absolute values (i.e. positive).
        /// </summary>
        /// <returns>A vector with <see cref="Mathf.Abs(int)"/> called on each component.</returns>
        public Vector2i Abs()
        {
            return new Vector2i(Mathf.Abs(x), Mathf.Abs(y));
        }

        /// <summary>
        /// Returns this vector's angle with respect to the X axis, or (1, 0) vector, in radians.
        ///
        /// Equivalent to the result of <see cref="Mathf.Atan2(real_t, real_t)"/> when
        /// called with the vector's <see cref="y"/> and <see cref="x"/> as parameters: <c>Mathf.Atan2(v.y, v.x)</c>.
        /// </summary>
        /// <returns>The angle of this vector, in radians.</returns>
        public real_t Angle()
        {
            return Mathf.Atan2(y, x);
        }

        /// <summary>
        /// Returns the angle to the given vector, in radians.
        /// </summary>
        /// <param name="to">The other vector to compare this vector to.</param>
        /// <returns>The angle between the two vectors, in radians.</returns>
        public real_t AngleTo(Vector2i to)
        {
            return Mathf.Atan2(Cross(to), Dot(to));
        }

        /// <summary>
        /// Returns the angle between the line connecting the two points and the X axis, in radians.
        /// </summary>
        /// <param name="to">The other vector to compare this vector to.</param>
        /// <returns>The angle between the two vectors, in radians.</returns>
        public real_t AngleToPoint(Vector2i to)
        {
            return Mathf.Atan2(y - to.y, x - to.x);
        }

        /// <summary>
        /// Returns the aspect ratio of this vector, the ratio of <see cref="x"/> to <see cref="y"/>.
        /// </summary>
        /// <returns>The <see cref="x"/> component divided by the <see cref="y"/> component.</returns>
        public real_t Aspect()
        {
            return x / (real_t)y;
        }

        /// <summary>
        /// Returns a new vector with all components clamped between the
        /// components of <paramref name="min"/> and <paramref name="max"/> using
        /// <see cref="Mathf.Clamp(int, int, int)"/>.
        /// </summary>
        /// <param name="min">The vector with minimum allowed values.</param>
        /// <param name="max">The vector with maximum allowed values.</param>
        /// <returns>The vector with all components clamped.</returns>
        public Vector2i Clamp(Vector2i min, Vector2i max)
        {
            return new Vector2i
            (
                Mathf.Clamp(x, min.x, max.x),
                Mathf.Clamp(y, min.y, max.y)
            );
        }

        /// <summary>
        /// Returns the cross product of this vector and <paramref name="with"/>.
        /// </summary>
        /// <param name="with">The other vector.</param>
        /// <returns>The cross product vector.</returns>
        public int Cross(Vector2i with)
        {
            return x * with.y - y * with.x;
        }

        /// <summary>
        /// Returns the squared distance between this vector and <paramref name="to"/>.
        /// This method runs faster than <see cref="DistanceTo"/>, so prefer it if
        /// you need to compare vectors or need the squared distance for some formula.
        /// </summary>
        /// <param name="to">The other vector to use.</param>
        /// <returns>The squared distance between the two vectors.</returns>
        public int DistanceSquaredTo(Vector2i to)
        {
            return (to - this).LengthSquared();
        }

        /// <summary>
        /// Returns the distance between this vector and <paramref name="to"/>.
        /// </summary>
        /// <param name="to">The other vector to use.</param>
        /// <returns>The distance between the two vectors.</returns>
        public real_t DistanceTo(Vector2i to)
        {
            return (to - this).Length();
        }

        /// <summary>
        /// Returns the dot product of this vector and <paramref name="with"/>.
        /// </summary>
        /// <param name="with">The other vector to use.</param>
        /// <returns>The dot product of the two vectors.</returns>
        public int Dot(Vector2i with)
        {
            return x * with.x + y * with.y;
        }

        /// <summary>
        /// Returns the length (magnitude) of this vector.
        /// </summary>
        /// <seealso cref="LengthSquared"/>
        /// <returns>The length of this vector.</returns>
        public real_t Length()
        {
            int x2 = x * x;
            int y2 = y * y;

            return Mathf.Sqrt(x2 + y2);
        }

        /// <summary>
        /// Returns the squared length (squared magnitude) of this vector.
        /// This method runs faster than <see cref="Length"/>, so prefer it if
        /// you need to compare vectors or need the squared length for some formula.
        /// </summary>
        /// <returns>The squared length of this vector.</returns>
        public int LengthSquared()
        {
            int x2 = x * x;
            int y2 = y * y;

            return x2 + y2;
        }

        /// <summary>
        /// Returns the axis of the vector's highest value. See <see cref="Axis"/>.
        /// If both components are equal, this method returns <see cref="Axis.X"/>.
        /// </summary>
        /// <returns>The index of the highest axis.</returns>
        public Axis MaxAxisIndex()
        {
            return x < y ? Axis.Y : Axis.X;
        }

        /// <summary>
        /// Returns the axis of the vector's lowest value. See <see cref="Axis"/>.
        /// If both components are equal, this method returns <see cref="Axis.Y"/>.
        /// </summary>
        /// <returns>The index of the lowest axis.</returns>
        public Axis MinAxisIndex()
        {
            return x < y ? Axis.X : Axis.Y;
        }

        /// <summary>
        /// Returns a vector composed of the <see cref="Mathf.PosMod(int, int)"/> of this vector's components
        /// and <paramref name="mod"/>.
        /// </summary>
        /// <param name="mod">A value representing the divisor of the operation.</param>
        /// <returns>
        /// A vector with each component <see cref="Mathf.PosMod(int, int)"/> by <paramref name="mod"/>.
        /// </returns>
        public Vector2i PosMod(int mod)
        {
            Vector2i v = this;
            v.x = Mathf.PosMod(v.x, mod);
            v.y = Mathf.PosMod(v.y, mod);
            return v;
        }

        /// <summary>
        /// Returns a vector composed of the <see cref="Mathf.PosMod(int, int)"/> of this vector's components
        /// and <paramref name="modv"/>'s components.
        /// </summary>
        /// <param name="modv">A vector representing the divisors of the operation.</param>
        /// <returns>
        /// A vector with each component <see cref="Mathf.PosMod(int, int)"/> by <paramref name="modv"/>'s components.
        /// </returns>
        public Vector2i PosMod(Vector2i modv)
        {
            Vector2i v = this;
            v.x = Mathf.PosMod(v.x, modv.x);
            v.y = Mathf.PosMod(v.y, modv.y);
            return v;
        }

        /// <summary>
        /// Returns a vector with each component set to one or negative one, depending
        /// on the signs of this vector's components, or zero if the component is zero,
        /// by calling <see cref="Mathf.Sign(int)"/> on each component.
        /// </summary>
        /// <returns>A vector with all components as either <c>1</c>, <c>-1</c>, or <c>0</c>.</returns>
        public Vector2i Sign()
        {
            Vector2i v = this;
            v.x = Mathf.Sign(v.x);
            v.y = Mathf.Sign(v.y);
            return v;
        }

        /// <summary>
        /// Returns a perpendicular vector rotated 90 degrees counter-clockwise
        /// compared to the original, with the same length.
        /// </summary>
        /// <returns>The perpendicular vector.</returns>
        public Vector2i Orthogonal()
        {
            return new Vector2i(y, -x);
        }

        // Constants
        private static readonly Vector2i _zero = new Vector2i(0, 0);
        private static readonly Vector2i _one = new Vector2i(1, 1);

        private static readonly Vector2i _up = new Vector2i(0, -1);
        private static readonly Vector2i _down = new Vector2i(0, 1);
        private static readonly Vector2i _right = new Vector2i(1, 0);
        private static readonly Vector2i _left = new Vector2i(-1, 0);

        /// <summary>
        /// Zero vector, a vector with all components set to <c>0</c>.
        /// </summary>
        /// <value>Equivalent to <c>new Vector2i(0, 0)</c>.</value>
        public static Vector2i Zero { get { return _zero; } }
        /// <summary>
        /// One vector, a vector with all components set to <c>1</c>.
        /// </summary>
        /// <value>Equivalent to <c>new Vector2i(1, 1)</c>.</value>
        public static Vector2i One { get { return _one; } }

        /// <summary>
        /// Up unit vector. Y is down in 2D, so this vector points -Y.
        /// </summary>
        /// <value>Equivalent to <c>new Vector2i(0, -1)</c>.</value>
        public static Vector2i Up { get { return _up; } }
        /// <summary>
        /// Down unit vector. Y is down in 2D, so this vector points +Y.
        /// </summary>
        /// <value>Equivalent to <c>new Vector2i(0, 1)</c>.</value>
        public static Vector2i Down { get { return _down; } }
        /// <summary>
        /// Right unit vector. Represents the direction of right.
        /// </summary>
        /// <value>Equivalent to <c>new Vector2i(1, 0)</c>.</value>
        public static Vector2i Right { get { return _right; } }
        /// <summary>
        /// Left unit vector. Represents the direction of left.
        /// </summary>
        /// <value>Equivalent to <c>new Vector2i(-1, 0)</c>.</value>
        public static Vector2i Left { get { return _left; } }

        /// <summary>
        /// Constructs a new <see cref="Vector2i"/> with the given components.
        /// </summary>
        /// <param name="x">The vector's X component.</param>
        /// <param name="y">The vector's Y component.</param>
        public Vector2i(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Constructs a new <see cref="Vector2i"/> from an existing <see cref="Vector2i"/>.
        /// </summary>
        /// <param name="vi">The existing <see cref="Vector2i"/>.</param>
        public Vector2i(Vector2i vi)
        {
            this.x = vi.x;
            this.y = vi.y;
        }

        /// <summary>
        /// Constructs a new <see cref="Vector2i"/> from an existing <see cref="Vector2"/>
        /// by rounding the components via <see cref="Mathf.RoundToInt(real_t)"/>.
        /// </summary>
        /// <param name="v">The <see cref="Vector2"/> to convert.</param>
        public Vector2i(Vector2 v)
        {
            this.x = Mathf.RoundToInt(v.x);
            this.y = Mathf.RoundToInt(v.y);
        }

        /// <summary>
        /// Adds each component of the <see cref="Vector2i"/>
        /// with the components of the given <see cref="Vector2i"/>.
        /// </summary>
        /// <param name="left">The left vector.</param>
        /// <param name="right">The right vector.</param>
        /// <returns>The added vector.</returns>
        public static Vector2i operator +(Vector2i left, Vector2i right)
        {
            left.x += right.x;
            left.y += right.y;
            return left;
        }

        /// <summary>
        /// Subtracts each component of the <see cref="Vector2i"/>
        /// by the components of the given <see cref="Vector2i"/>.
        /// </summary>
        /// <param name="left">The left vector.</param>
        /// <param name="right">The right vector.</param>
        /// <returns>The subtracted vector.</returns>
        public static Vector2i operator -(Vector2i left, Vector2i right)
        {
            left.x -= right.x;
            left.y -= right.y;
            return left;
        }

        /// <summary>
        /// Returns the negative value of the <see cref="Vector2i"/>.
        /// This is the same as writing <c>new Vector2i(-v.x, -v.y)</c>.
        /// This operation flips the direction of the vector while
        /// keeping the same magnitude.
        /// </summary>
        /// <param name="vec">The vector to negate/flip.</param>
        /// <returns>The negated/flipped vector.</returns>
        public static Vector2i operator -(Vector2i vec)
        {
            vec.x = -vec.x;
            vec.y = -vec.y;
            return vec;
        }

        /// <summary>
        /// Multiplies each component of the <see cref="Vector2i"/>
        /// by the given <see langword="int"/>.
        /// </summary>
        /// <param name="vec">The vector to multiply.</param>
        /// <param name="scale">The scale to multiply by.</param>
        /// <returns>The multiplied vector.</returns>
        public static Vector2i operator *(Vector2i vec, int scale)
        {
            vec.x *= scale;
            vec.y *= scale;
            return vec;
        }

        /// <summary>
        /// Multiplies each component of the <see cref="Vector2i"/>
        /// by the given <see langword="int"/>.
        /// </summary>
        /// <param name="scale">The scale to multiply by.</param>
        /// <param name="vec">The vector to multiply.</param>
        /// <returns>The multiplied vector.</returns>
        public static Vector2i operator *(int scale, Vector2i vec)
        {
            vec.x *= scale;
            vec.y *= scale;
            return vec;
        }

        /// <summary>
        /// Multiplies each component of the <see cref="Vector2i"/>
        /// by the components of the given <see cref="Vector2i"/>.
        /// </summary>
        /// <param name="left">The left vector.</param>
        /// <param name="right">The right vector.</param>
        /// <returns>The multiplied vector.</returns>
        public static Vector2i operator *(Vector2i left, Vector2i right)
        {
            left.x *= right.x;
            left.y *= right.y;
            return left;
        }

        /// <summary>
        /// Divides each component of the <see cref="Vector2i"/>
        /// by the given <see langword="int"/>.
        /// </summary>
        /// <param name="vec">The dividend vector.</param>
        /// <param name="divisor">The divisor value.</param>
        /// <returns>The divided vector.</returns>
        public static Vector2i operator /(Vector2i vec, int divisor)
        {
            vec.x /= divisor;
            vec.y /= divisor;
            return vec;
        }

        /// <summary>
        /// Divides each component of the <see cref="Vector2i"/>
        /// by the components of the given <see cref="Vector2i"/>.
        /// </summary>
        /// <param name="vec">The dividend vector.</param>
        /// <param name="divisorv">The divisor vector.</param>
        /// <returns>The divided vector.</returns>
        public static Vector2i operator /(Vector2i vec, Vector2i divisorv)
        {
            vec.x /= divisorv.x;
            vec.y /= divisorv.y;
            return vec;
        }

        /// <summary>
        /// Gets the remainder of each component of the <see cref="Vector2i"/>
        /// with the components of the given <see langword="int"/>.
        /// This operation uses truncated division, which is often not desired
        /// as it does not work well with negative numbers.
        /// Consider using <see cref="PosMod(int)"/> instead
        /// if you want to handle negative numbers.
        /// </summary>
        /// <example>
        /// <code>
        /// GD.Print(new Vector2i(10, -20) % 7); // Prints "(3, -6)"
        /// </code>
        /// </example>
        /// <param name="vec">The dividend vector.</param>
        /// <param name="divisor">The divisor value.</param>
        /// <returns>The remainder vector.</returns>
        public static Vector2i operator %(Vector2i vec, int divisor)
        {
            vec.x %= divisor;
            vec.y %= divisor;
            return vec;
        }

        /// <summary>
        /// Gets the remainder of each component of the <see cref="Vector2i"/>
        /// with the components of the given <see cref="Vector2i"/>.
        /// This operation uses truncated division, which is often not desired
        /// as it does not work well with negative numbers.
        /// Consider using <see cref="PosMod(Vector2i)"/> instead
        /// if you want to handle negative numbers.
        /// </summary>
        /// <example>
        /// <code>
        /// GD.Print(new Vector2i(10, -20) % new Vector2i(7, 8)); // Prints "(3, -4)"
        /// </code>
        /// </example>
        /// <param name="vec">The dividend vector.</param>
        /// <param name="divisorv">The divisor vector.</param>
        /// <returns>The remainder vector.</returns>
        public static Vector2i operator %(Vector2i vec, Vector2i divisorv)
        {
            vec.x %= divisorv.x;
            vec.y %= divisorv.y;
            return vec;
        }

        /// <summary>
        /// Performs a bitwise AND operation with this <see cref="Vector2i"/>
        /// and the given <see langword="int"/>.
        /// </summary>
        /// <param name="vec">The vector to AND with.</param>
        /// <param name="and">The integer to AND with.</param>
        /// <returns>The result of the bitwise AND.</returns>
        public static Vector2i operator &(Vector2i vec, int and)
        {
            vec.x &= and;
            vec.y &= and;
            return vec;
        }

        /// <summary>
        /// Performs a bitwise AND operation with this <see cref="Vector2i"/>
        /// and the given <see cref="Vector2i"/>.
        /// </summary>
        /// <param name="vec">The left vector to AND with.</param>
        /// <param name="andv">The right vector to AND with.</param>
        /// <returns>The result of the bitwise AND.</returns>
        public static Vector2i operator &(Vector2i vec, Vector2i andv)
        {
            vec.x &= andv.x;
            vec.y &= andv.y;
            return vec;
        }

        /// <summary>
        /// Returns <see langword="true"/> if the vectors are equal.
        /// </summary>
        /// <param name="left">The left vector.</param>
        /// <param name="right">The right vector.</param>
        /// <returns>Whether or not the vectors are equal.</returns>
        public static bool operator ==(Vector2i left, Vector2i right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns <see langword="true"/> if the vectors are not equal.
        /// </summary>
        /// <param name="left">The left vector.</param>
        /// <param name="right">The right vector.</param>
        /// <returns>Whether or not the vectors are not equal.</returns>
        public static bool operator !=(Vector2i left, Vector2i right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Compares two <see cref="Vector2i"/> vectors by first checking if
        /// the X value of the <paramref name="left"/> vector is less than
        /// the X value of the <paramref name="right"/> vector.
        /// If the X values are exactly equal, then it repeats this check
        /// with the Y values of the two vectors.
        /// This operator is useful for sorting vectors.
        /// </summary>
        /// <param name="left">The left vector.</param>
        /// <param name="right">The right vector.</param>
        /// <returns>Whether or not the left is less than the right.</returns>
        public static bool operator <(Vector2i left, Vector2i right)
        {
            if (left.x == right.x)
            {
                return left.y < right.y;
            }
            return left.x < right.x;
        }

        /// <summary>
        /// Compares two <see cref="Vector2i"/> vectors by first checking if
        /// the X value of the <paramref name="left"/> vector is greater than
        /// the X value of the <paramref name="right"/> vector.
        /// If the X values are exactly equal, then it repeats this check
        /// with the Y values of the two vectors.
        /// This operator is useful for sorting vectors.
        /// </summary>
        /// <param name="left">The left vector.</param>
        /// <param name="right">The right vector.</param>
        /// <returns>Whether or not the left is greater than the right.</returns>
        public static bool operator >(Vector2i left, Vector2i right)
        {
            if (left.x == right.x)
            {
                return left.y > right.y;
            }
            return left.x > right.x;
        }

        /// <summary>
        /// Compares two <see cref="Vector2i"/> vectors by first checking if
        /// the X value of the <paramref name="left"/> vector is less than
        /// or equal to the X value of the <paramref name="right"/> vector.
        /// If the X values are exactly equal, then it repeats this check
        /// with the Y values of the two vectors.
        /// This operator is useful for sorting vectors.
        /// </summary>
        /// <param name="left">The left vector.</param>
        /// <param name="right">The right vector.</param>
        /// <returns>Whether or not the left is less than or equal to the right.</returns>
        public static bool operator <=(Vector2i left, Vector2i right)
        {
            if (left.x == right.x)
            {
                return left.y <= right.y;
            }
            return left.x < right.x;
        }

        /// <summary>
        /// Compares two <see cref="Vector2i"/> vectors by first checking if
        /// the X value of the <paramref name="left"/> vector is greater than
        /// or equal to the X value of the <paramref name="right"/> vector.
        /// If the X values are exactly equal, then it repeats this check
        /// with the Y values of the two vectors.
        /// This operator is useful for sorting vectors.
        /// </summary>
        /// <param name="left">The left vector.</param>
        /// <param name="right">The right vector.</param>
        /// <returns>Whether or not the left is greater than or equal to the right.</returns>
        public static bool operator >=(Vector2i left, Vector2i right)
        {
            if (left.x == right.x)
            {
                return left.y >= right.y;
            }
            return left.x > right.x;
        }

        /// <summary>
        /// Converts this <see cref="Vector2i"/> to a <see cref="Vector2"/>.
        /// </summary>
        /// <param name="value">The vector to convert.</param>
        public static implicit operator Vector2(Vector2i value)
        {
            return new Vector2(value.x, value.y);
        }

        /// <summary>
        /// Converts a <see cref="Vector2"/> to a <see cref="Vector2i"/>.
        /// </summary>
        /// <param name="value">The vector to convert.</param>
        public static explicit operator Vector2i(Vector2 value)
        {
            return new Vector2i(value);
        }

        /// <summary>
        /// Returns <see langword="true"/> if the vector is equal
        /// to the given object (<see paramref="obj"/>).
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns>Whether or not the vector and the object are equal.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Vector2i)
            {
                return Equals((Vector2i)obj);
            }

            return false;
        }

        /// <summary>
        /// Returns <see langword="true"/> if the vectors are equal.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>Whether or not the vectors are equal.</returns>
        public bool Equals(Vector2i other)
        {
            return x == other.x && y == other.y;
        }

        /// <summary>
        /// Serves as the hash function for <see cref="Vector2i"/>.
        /// </summary>
        /// <returns>A hash code for this vector.</returns>
        public override int GetHashCode()
        {
            return y.GetHashCode() ^ x.GetHashCode();
        }

        /// <summary>
        /// Converts this <see cref="Vector2i"/> to a string.
        /// </summary>
        /// <returns>A string representation of this vector.</returns>
        public override string ToString()
        {
            return $"({x}, {y})";
        }

        /// <summary>
        /// Converts this <see cref="Vector2i"/> to a string with the given <paramref name="format"/>.
        /// </summary>
        /// <returns>A string representation of this vector.</returns>
        public string ToString(string format)
        {
            return $"({x.ToString(format)}, {y.ToString(format)})";
        }
    }
}
