using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent.Year2021
{
    struct Operation
    {
        public Vec3 start;
        public Vec3 end;
        public bool state;
    }

    class Cuboid
    {
        public Vec3 Start { get; }
        public Vec3 End { get; }

        public Cuboid(Vec3 start, Vec3 end)
        {
            Start = start;
            End = end;
        }

        public long Size()
        {
            if(Degenerate()) return 0;

            var diff = (End - Start) + new Vec3(1,1,1);

            return (long)diff.X * (long)diff.Y * (long)diff.Z;
        }

        public bool Degenerate()
        {
            return (Start.X > End.X || Start.Y > End.Y || Start.Z > End.Z);
        }

        public bool Intersects(Cuboid other)
        {
            return !
            (other.Start.X > End.X || other.End.X < Start.X ||
             other.Start.Y > End.Y || other.End.Y < Start.Y ||
             other.Start.Z > End.Z || other.End.Z < Start.Z);
        }

        public (IEnumerable<Cuboid>, Cuboid) Shatter(Cuboid other)
        {
            List<Cuboid> newCubes = new();

            if (!Intersects(other))
            {
                newCubes.Add(this);
                return (newCubes, null);
            }

            int x0 = Start.X;
            int x1 = Math.Max(Start.X, other.Start.X);
            int x2 = Math.Min(End.X, other.End.X);
            int x3 = End.X;

            int y0 = Start.Y;
            int y1 = Math.Max(Start.Y, other.Start.Y);
            int y2 = Math.Min(End.Y, other.End.Y);
            int y3 = End.Y;

            int z0 = Start.Z;
            int z1 = Math.Max(Start.Z, other.Start.Z);
            int z2 = Math.Min(End.Z, other.End.Z);
            int z3 = End.Z;

            // Z0 -> Z1 layer
            newCubes.Add(new Cuboid(new Vec3(x0,   y0, z0),  new Vec3(x1-1, y1-1, z1-1)));
            newCubes.Add(new Cuboid(new Vec3(x1,   y0, z0),  new Vec3(x2,   y1-1, z1-1)));
            newCubes.Add(new Cuboid(new Vec3(x2+1, y0, z0),  new Vec3(x3,   y1-1, z1-1)));

            newCubes.Add(new Cuboid(new Vec3(x0,   y1, z0),  new Vec3(x1-1, y2, z1-1)));
            newCubes.Add(new Cuboid(new Vec3(x1,   y1, z0),  new Vec3(x2,   y2, z1-1)));
            newCubes.Add(new Cuboid(new Vec3(x2+1, y1, z0),  new Vec3(x3,   y2, z1-1)));

            newCubes.Add(new Cuboid(new Vec3(x0,   y2+1, z0), new Vec3(x1-1, y3, z1-1)));
            newCubes.Add(new Cuboid(new Vec3(x1,   y2+1, z0), new Vec3(x2,   y3, z1-1)));
            newCubes.Add(new Cuboid(new Vec3(x2+1, y2+1, z0), new Vec3(x3,   y3, z1-1)));

            // Z1 -> Z2 layer
            newCubes.Add(new Cuboid(new Vec3(x0,   y0, z1), new Vec3(x1-1, y1-1, z2)));
            newCubes.Add(new Cuboid(new Vec3(x1,   y0, z1), new Vec3(x2,   y1-1, z2)));
            newCubes.Add(new Cuboid(new Vec3(x2+1, y0, z1), new Vec3(x3,   y1-1, z2)));

            newCubes.Add(new Cuboid(new Vec3(x0,   y1, z1), new Vec3(x1-1, y2, z2)));
            var middle = (new Cuboid(new Vec3(x1,  y1, z1), new Vec3(x2,   y2, z2)));
            newCubes.Add(new Cuboid(new Vec3(x2+1, y1, z1), new Vec3(x3,   y2, z2)));

            newCubes.Add(new Cuboid(new Vec3(x0,   y2+1, z1), new Vec3(x1-1, y3, z2)));
            newCubes.Add(new Cuboid(new Vec3(x1,   y2+1, z1), new Vec3(x2,   y3, z2)));
            newCubes.Add(new Cuboid(new Vec3(x2+1, y2+1, z1), new Vec3(x3,   y3, z2)));

            // Z2 -> Z3 layer
            newCubes.Add(new Cuboid(new Vec3(x0,   y0, z2+1), new Vec3(x1-1, y1-1, z3)));
            newCubes.Add(new Cuboid(new Vec3(x1,   y0, z2+1), new Vec3(x2,   y1-1, z3)));
            newCubes.Add(new Cuboid(new Vec3(x2+1, y0, z2+1), new Vec3(x3,   y1-1, z3)));

            newCubes.Add(new Cuboid(new Vec3(x0,   y1, z2+1), new Vec3(x1-1, y2, z3)));
            newCubes.Add(new Cuboid(new Vec3(x1,   y1, z2+1), new Vec3(x2,   y2, z3)));
            newCubes.Add(new Cuboid(new Vec3(x2+1, y1, z2+1), new Vec3(x3,   y2, z3)));

            newCubes.Add(new Cuboid(new Vec3(x0,   y2+1, z2+1), new Vec3(x1-1, y3, z3)));
            newCubes.Add(new Cuboid(new Vec3(x1,   y2+1, z2+1), new Vec3(x2,   y3, z3)));
            newCubes.Add(new Cuboid(new Vec3(x2+1, y2+1, z2+1), new Vec3(x3,   y3, z3)));

            return (newCubes, middle);

        }
    }

    internal class Day22 : DayLineLoaderBasic
    {
        List<Operation> m_operations = new();

        public override void ParseInputLine(string line)
        {
            Regex r = new Regex(@"(on|off) x=(-?\d+)..(-?\d+),y=(-?\d+)..(-?\d+),z=(-?\d+)..(-?\d+)");

            var match = r.Match(line);
            if(match.Success)
            {
                var op = new Operation();
                op.state = match.Groups[1].Value == "on" ? true : false;

                int x1 = int.Parse(match.Groups[2].Value);
                int x2 = int.Parse(match.Groups[3].Value);
                int y1 = int.Parse(match.Groups[4].Value);
                int y2 = int.Parse(match.Groups[5].Value);
                int z1 = int.Parse(match.Groups[6].Value);
                int z2 = int.Parse(match.Groups[7].Value);

                op.start = new Vec3(x1, y1, z1);
                op.end = new Vec3(x2, y2, z2);
                m_operations.Add(op);
            }
        }

        HashSet<Vec3> m_onCubes = new();

        public long Naive()
        {
            foreach (var op in m_operations)
            {
                if (-50 <= op.start.X && op.start.X <= 50 && -50 <= op.start.Y && op.start.Y <= 50 && -50 <= op.start.Z && op.start.Z <= 50)
                {
                    for (int x = op.start.X; x <= op.end.X; x++)
                    {
                        for (int y = op.start.Y; y <= op.end.Y; y++)
                        {
                            for (int z = op.start.Z; z <= op.end.Z; z++)
                            {
                                if (op.state)
                                {
                                    m_onCubes.Add(new Vec3(x, y, z));
                                }
                                else
                                {
                                    m_onCubes.Remove(new Vec3(x, y, z));
                                }
                            }
                        }
                    }
                }
            }

            return m_onCubes.Count();
        }

        public long Compute(bool init = false)
        {
            List<Cuboid> cuboids = new();

            foreach (var op in m_operations)
            {
                Cuboid c = new Cuboid(op.start, op.end);
                if (init && (c.Start.X < -50 || c.Start.X > 50 || c.Start.Y < -50 || c.Start.Y > 50 || c.Start.Z < -50 || c.Start.Z > 50))
                    continue;

                Queue<Cuboid> toAdd = new();
                toAdd.Enqueue(c);

                while (toAdd.Count > 0)
                {
                    Cuboid next = toAdd.Dequeue();
                    if (cuboids.Any(x => x.Intersects(next)))
                    {
                        var problem = cuboids.First(x => x.Intersects(next));
                        (var newCubes, var overlap) = problem.Shatter(next);

                        if (problem.Size() != newCubes.Sum(x => x.Size()) + overlap.Size())
                            throw new InvalidOperationException();

                        // Remove the big cuboid
                        cuboids.Remove(problem);
                        // Add all the little ones, but not the overlap and not any empty ones
                        cuboids.AddRange(newCubes.Where(x => !x.Degenerate()));

                        // Put the current cuboid back in the list to add
                        toAdd.Enqueue(next);
                    }
                    else
                    {
                        if (op.state)
                        {
                            cuboids.Add(next);
                        }
                    }
                }
            }

            return cuboids.Sum(x => x.Size());
        }

        public override string Part1()
        {
            return Compute(true).ToString();  
        }

        public override string Part2()
        {
            return Compute(false).ToString();
        }
    }
}
