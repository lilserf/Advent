using System;
using System.Diagnostics;

namespace Advent
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct Vec4 : IEquatable<Vec4>
    {


        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int W { get; set; }
        public Vec4(int x, int y, int z, int w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public static Vec4 operator *(Vec4 a, int m)
        {
            return new Vec4(a.X * m, a.Y * m, a.Z * m, a.W * m);
        }
        public static Vec4 operator *(int m, Vec4 a)
        {
            return a * m;
        }
        public static Vec4 operator +(Vec4 a, Vec4 b)
        {
            return new Vec4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        }
        public static Vec4 operator -(Vec4 a, Vec4 b)
        {
            return new Vec4(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        }
        public static Vec4 operator /(Vec4 a, Vec4 b)
        {
            return new Vec4(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W);
        }
        public static Vec4 operator /(Vec4 a, int b)
        {
            return new Vec4(a.X / b, a.Y / b, a.Z / b, a.W / b);
        }
        public static bool operator ==(Vec4 a, Vec4 b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;
        }
        public static bool operator !=(Vec4 a, Vec4 b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            if (obj is Vec4)
            {
                return ((Vec4)obj) == this;
            }
            return false;
        }
        public bool Equals(Vec4 other)
        {
            return (other == this);
        }
        public override int GetHashCode()
        {
            return Hash.Create(X, Y);
        }
        public static int ManhattanDistance(Vec4 a, Vec4 b)
        {
            return Math.Abs(b.X - a.X) + Math.Abs(b.Y - a.Y) + Math.Abs(b.Z - a.Z) + Math.Abs(b.W - a.W);
        }
        public override string ToString()
        {
            return $"[{X},{Y},{Z},{W}]";
        }
        public string DebuggerDisplay { get { return ToString(); } }

    }
}
