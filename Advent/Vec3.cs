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
        public static double Distance(Vec3 a, Vec3 b)
        {
            return Math.Sqrt((b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y) + (b.Z - a.Z) * (b.Z - a.Z));
        }

        public override string ToString()
        {
            return $"[{X},{Y},{Z}]";
        }
        
        public static Vec3 Parse(string s)
        {
            var split = s.Split(',');
            return new Vec3(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]));
        }

        public string DebuggerDisplay { get { return ToString(); } }

    }

    public struct Vec3Rotation
    {
        public static readonly Vec3Rotation ZRight = new Vec3Rotation(Dest.PositiveY, Dest.NegativeX, Dest.PositiveZ);
        public static readonly Vec3Rotation ZLeft = new Vec3Rotation(Dest.NegativeY, Dest.PositiveX, Dest.PositiveZ);
        public static readonly Vec3Rotation Z180 = new Vec3Rotation(Dest.NegativeX, Dest.NegativeY, Dest.PositiveZ);

        public static readonly Vec3Rotation XRight = new Vec3Rotation(Dest.PositiveX, Dest.NegativeZ, Dest.PositiveY);
        public static readonly Vec3Rotation XLeft = new Vec3Rotation(Dest.PositiveX, Dest.PositiveZ, Dest.NegativeY);
        public static readonly Vec3Rotation X180 = new Vec3Rotation(Dest.PositiveX, Dest.NegativeY, Dest.NegativeZ);

        public static readonly Vec3Rotation YRight = new Vec3Rotation(Dest.NegativeZ, Dest.PositiveY, Dest.PositiveX);
        public static readonly Vec3Rotation YLeft = new Vec3Rotation(Dest.PositiveZ, Dest.PositiveY, Dest.NegativeX);
        public static readonly Vec3Rotation Y180 = new Vec3Rotation(Dest.NegativeX, Dest.PositiveY, Dest.NegativeZ);

        public static readonly Vec3Rotation None = new Vec3Rotation(Dest.PositiveX, Dest.PositiveY, Dest.PositiveZ);

        public enum Dest
        {
            PositiveX,
            NegativeX,
            PositiveY,
            NegativeY,
            PositiveZ,
            NegativeZ,
        }

        Dest m_x, m_y, m_z;

        public Vec3Rotation(Dest x, Dest y, Dest z)
        {
            m_x = x;
            m_y = y;
            m_z = z;
        }

        private void ApplyOne(int src, Vec3 dest, Dest op)
        {
            switch(op)
            {
                case Dest.PositiveX: dest.X = src; break;
                case Dest.NegativeX: dest.X = -src; break;
                case Dest.PositiveY: dest.Y = src; break;
                case Dest.NegativeY: dest.Y = -src; break;
                case Dest.PositiveZ: dest.Z = src; break;
                case Dest.NegativeZ: dest.Z = -src; break;
                default: throw new InvalidOperationException();
            }
        }

        public Vec3 Apply(Vec3 a)
        {
            Vec3 b = new Vec3();
            ApplyOne(a.X, b, m_x);
            ApplyOne(a.Y, b, m_y);
            ApplyOne(a.Z, b, m_z);
            return b;
        }

        public static Vec3Rotation operator*(Vec3Rotation a, Vec3Rotation b)
        {

        }

        public static Vec3 operator*(Vec3 a, Vec3Rotation b)
        {
            return b.Apply(a);
        }
    }
}
