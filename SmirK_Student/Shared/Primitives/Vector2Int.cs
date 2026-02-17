using System.Runtime.CompilerServices;

namespace SmirK_Student.Shared.Primitives
{
    /// <summary>
    /// Целочисленный двумерный вектор.
    /// Базовые методы и перегрузку операторов перетянул с оригинального Vector2.
    /// </summary>
    public struct Vector2Int
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Vector2Int(int value) : this(value, value)
        {
        }

        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Vector2Int Zero
        {
            get => default;
        }

        public static Vector2Int One
        {
            get => new Vector2Int(1);
        }

        public static Vector2Int UnitX
        {
            get => new Vector2Int(1, 0);
        }

        public static Vector2Int UnitY
        {
            get => new Vector2Int(0, 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int operator +(Vector2Int left, Vector2Int right)
        {
            return new Vector2Int(
                left.X + right.X,
                left.Y + right.Y
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int operator /(Vector2Int left, Vector2Int right)
        {
            return new Vector2Int(
                left.X / right.X,
                left.Y / right.Y
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int operator /(Vector2Int value1, int value2)
        {
            return new Vector2Int(
                value1.X / value2,
                value1.Y / value2
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector2Int left, Vector2Int right)
        {
            return (left.X == right.X)
                   && (left.Y == right.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector2Int left, Vector2Int right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int operator *(Vector2Int left, Vector2Int right)
        {
            return new Vector2Int(
                left.X * right.X,
                left.Y * right.Y
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int operator *(Vector2Int left, int right)
        {
            return new Vector2Int(
                left.X * right,
                left.Y * right
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int operator *(int left, Vector2Int right)
        {
            return right * left;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int operator -(Vector2Int left, Vector2Int right)
        {
            return new Vector2Int(
                left.X - right.X,
                left.Y - right.Y
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int operator -(Vector2Int value)
        {
            return Zero - value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Abs(Vector2Int value)
        {
            return new Vector2Int(
                Math.Abs(value.X),
                Math.Abs(value.Y)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Add(Vector2Int left, Vector2Int right)
        {
            return left + right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Clamp(Vector2Int value1, Vector2Int min, Vector2Int max)
        {
            return Min(Max(value1, min), max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(Vector2Int value1, Vector2Int value2)
        {
            float distanceSquared = DistanceSquared(value1, value2);
            return MathF.Sqrt(distanceSquared);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int DistanceSquared(Vector2Int value1, Vector2Int value2)
        {
            Vector2Int difference = value1 - value2;
            return Dot(difference, difference);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Divide(Vector2Int left, Vector2Int right)
        {
            return left / right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Divide(Vector2Int left, int divisor)
        {
            return left / divisor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Dot(Vector2Int value1, Vector2Int value2)
        {
            return (value1.X * value2.X)
                   + (value1.Y * value2.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Max(Vector2Int value1, Vector2Int value2)
        {
            return new Vector2Int(
                (value1.X > value2.X) ? value1.X : value2.X,
                (value1.Y > value2.Y) ? value1.Y : value2.Y
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Min(Vector2Int value1, Vector2Int value2)
        {
            return new Vector2Int(
                (value1.X < value2.X) ? value1.X : value2.X,
                (value1.Y < value2.Y) ? value1.Y : value2.Y
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Multiply(Vector2Int left, Vector2Int right)
        {
            return left * right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Multiply(Vector2Int left, int right)
        {
            return left * right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Multiply(int left, Vector2Int right)
        {
            return left * right;
        }
    }
}