using System;
using System.Diagnostics;

namespace Advent
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct Vec2 : IEquatable<Vec2>
    {
        public static readonly Vec2 Zero = new Vec2(0, 0);
        public static readonly Vec2 Up = new Vec2(0, -1);
        public static readonly Vec2 UpRight = new Vec2(1, -1);
        public static readonly Vec2 Right = new Vec2(1, 0);
        public static readonly Vec2 DownRight = new Vec2(1, 1);
        public static readonly Vec2 Down = new Vec2(0, 1);
        public static readonly Vec2 DownLeft = new Vec2(-1, 1);
        public static readonly Vec2 Left = new Vec2(-1, 0);
        public static readonly Vec2 UpLeft = new Vec2(-1, -1);

        public static readonly Vec2[] Adjacent = new[] { Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft };
        public static readonly Vec2[] CardinalAdjacent = new[] { Up, Right, Down, Left };


        public int X { get; set; }
        public int Y { get; set; }
        public Vec2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Vec2 operator *(Vec2 a, int m)
        {
            return new Vec2(a.X * m, a.Y * m);
        }
        public static Vec2 operator *(int m, Vec2 a)
        {
            return a * m;
        }
        public static Vec2 operator +(Vec2 a, Vec2 b)
        {
            return new Vec2(a.X + b.X, a.Y + b.Y);
        }
        public static Vec2 operator -(Vec2 a, Vec2 b)
        {
            return new Vec2(a.X - b.X, a.Y - b.Y);
        }
        public static Vec2 operator /(Vec2 a, Vec2 b)
        {
            return new Vec2(a.X / b.X, a.Y / b.Y);
        }
        public static Vec2 operator /(Vec2 a, int b)
        {
            return new Vec2(a.X / b, a.Y / b);
        }
        public static bool operator ==(Vec2 a, Vec2 b)
        {
            return a.X == b.X && a.Y == b.Y;
        }
        public static bool operator !=(Vec2 a, Vec2 b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            if (obj is Vec2)
            {
                return ((Vec2)obj) == this;
            }
            return false;
        }
        public bool Equals(Vec2 other)
        {
            return (other == this);
        }
        public override int GetHashCode()
        {
            return Hash.Create(X, Y);
        }
        public static int ManhattanDistance(Vec2 a, Vec2 b)
        {
            return Math.Abs(b.X - a.X) + Math.Abs(b.Y - a.Y);
        }
        public override string ToString()
        {
            return $"[{X},{Y}]";
        }
        public string DebuggerDisplay { get { return ToString(); } }

        public void Rotate90CounterClockwise()
        {
            X = X * -1;
            var temp = X;
            X = Y;
            Y = temp;
        }
    }
}
