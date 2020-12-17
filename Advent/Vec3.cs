using System;
using System.Diagnostics;

namespace Advent
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct Vec3 : IEquatable<Vec3>
    {
        public static readonly Vec3 Zero = new Vec3(0, 0, 0);
        public static readonly Vec3 Up = new Vec3(0, -1, 0);
        public static readonly Vec3 UpRight = new Vec3(1, -1, 0);
        public static readonly Vec3 Right = new Vec3(1, 0, 0);
        public static readonly Vec3 DownRight = new Vec3(1, 1, 0);
        public static readonly Vec3 Down = new Vec3(0, 1, 0);
        public static readonly Vec3 DownLeft = new Vec3(-1, 1, 0);
        public static readonly Vec3 Left = new Vec3(-1, 0, 0);
        public static readonly Vec3 UpLeft = new Vec3(-1, -1, 0);

        public static readonly Vec3 ZPosZero = new Vec3(0, 0, 1);
        public static readonly Vec3 ZPosUp = new Vec3(0, -1, 1);
        public static readonly Vec3 ZPosUpRight = new Vec3(1, -1, 1);
        public static readonly Vec3 ZPosRight = new Vec3(1, 0, 1);
        public static readonly Vec3 ZPosDownRight = new Vec3(1, 1, 1);
        public static readonly Vec3 ZPosDown = new Vec3(0, 1, 1);
        public static readonly Vec3 ZPosDownLeft = new Vec3(-1, 1, 1);
        public static readonly Vec3 ZPosLeft = new Vec3(-1, 0, 1);
        public static readonly Vec3 ZPosUpLeft = new Vec3(-1, -1, 1);

        public static readonly Vec3 ZNegZero = new Vec3(0, 0, -1);
        public static readonly Vec3 ZNegUp = new Vec3(0, -1, -1);
        public static readonly Vec3 ZNegUpRight = new Vec3(1, -1, -1);
        public static readonly Vec3 ZNegRight = new Vec3(1, 0, -1);
        public static readonly Vec3 ZNegDownRight = new Vec3(1, 1, -1);
        public static readonly Vec3 ZNegDown = new Vec3(0, 1, -1);
        public static readonly Vec3 ZNegDownLeft = new Vec3(-1, 1, -1);
        public static readonly Vec3 ZNegLeft = new Vec3(-1, 0, -1);
        public static readonly Vec3 ZNegUpLeft = new Vec3(-1, -1, -1);

        public static readonly Vec3[] Adjacent = new[]
        {
            Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft,
            ZPosZero, ZPosUp, ZPosUpRight, ZPosRight, ZPosDownRight, ZPosDown, ZPosDownLeft, ZPosLeft, ZPosUpLeft,
            ZNegZero, ZNegUp, ZNegUpRight, ZNegRight, ZNegDownRight, ZNegDown, ZNegDownLeft, ZNegLeft, ZNegUpLeft
        };

        public static readonly Vec3[] CardinalAdjacent = new[] { Up, Right, Down, Left, ZPosZero, ZNegZero };


        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public Vec3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vec3 operator *(Vec3 a, int m)
        {
            return new Vec3(a.X * m, a.Y * m, a.Z * m);
        }
        public static Vec3 operator *(int m, Vec3 a)
        {
            return a * m;
        }
        public static Vec3 operator +(Vec3 a, Vec3 b)
        {
            return new Vec3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
        public static Vec3 operator -(Vec3 a, Vec3 b)
        {
            return new Vec3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
        public static Vec3 operator /(Vec3 a, Vec3 b)
        {
            return new Vec3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }
        public static Vec3 operator /(Vec3 a, int b)
        {
            return new Vec3(a.X / b, a.Y / b, a.Z / b);
        }
        public static bool operator ==(Vec3 a, Vec3 b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }
        public static bool operator !=(Vec3 a, Vec3 b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            if (obj is Vec3)
            {
                return ((Vec3)obj) == this;
            }
            return false;
        }
        public bool Equals(Vec3 other)
        {
            return (other == this);
        }
        public override int GetHashCode()
        {
            return Hash.Create(X, Y);
        }
        public static int ManhattanDistance(Vec3 a, Vec3 b)
        {
            return Math.Abs(b.X - a.X) + Math.Abs(b.Y - a.Y) + Math.Abs(b.Z - a.Z);
        }
        public override string ToString()
        {
            return $"[{X},{Y},{Z}]";
        }
        public string DebuggerDisplay { get { return ToString(); } }

    }
}
