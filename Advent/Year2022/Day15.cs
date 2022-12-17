using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent2020.Year2022
{
    internal class Day15 : DayLineLoaderBasic
    {
        Dictionary<Vec2, Vec2> m_sensors = new();
        Dictionary<Vec2, int> m_distances = new();

        public override void ParseInputLine(string line)
        {
            Regex reg = new Regex(@"Sensor at x=(-?\d+), y=(-?\d+): closest beacon is at x=(-?\d+), y=(-?\d+)");

            var match = MatchLine(line, reg);

            if(match.Success)
            {
                var sensorX = int.Parse(match.Groups[1].Value);
                var sensorY = int.Parse(match.Groups[2].Value);
                
                var beaconX = int.Parse(match.Groups[3].Value);
                var beaconY = int.Parse(match.Groups[4].Value);

                m_sensors.Add(new Vec2(sensorX, sensorY), new Vec2(beaconX, beaconY));
            }
        }

        public (bool, Vec2) GetSpanAtDistanceAndY(Vec2 start, int distance, int yValue)
        {
            var top = start + new Vec2(0, -distance);
            var bottom = start + new Vec2(0, distance);
            var left = start + new Vec2(-distance, 0);
            var right = start + new Vec2(distance, 0);

            if(top.Y <= yValue && bottom.Y >= yValue)
            {
                var vertOffset = yValue - start.Y;
                var leftEdge = start.X - (distance - Math.Abs(vertOffset));
                var rightEdge = start.X + (distance - Math.Abs(vertOffset));

                return (true, new Vec2(leftEdge, rightEdge));
            }
            else
            {
                return (false, Vec2.Zero);
            }
        }

        (bool, Vec2) CollapseSpan(Vec2 a, Vec2 b)
        {
            if((a.Y < b.X) || (b.Y < a.X))
            {
                return (false, Vec2.Zero);
            }

            Vec2 result = new Vec2();
            result.X = Math.Min(a.X, b.X);
            result.Y = Math.Max(a.Y, b.Y);

            return (true, result);
        }

        public override string Part1()
        {
            HashSet<Vec2> spans = new();

            //int yVal = 10;
            int yVal = 2000000;

            //int yVal = 8;

            foreach (var (s, b) in m_sensors)
            {
                var dist = Vec2.ManhattanDistance(s, b);
                m_distances[s] = dist;

                (var success, Vec2 span) = GetSpanAtDistanceAndY(s, dist, yVal);
                if(success)
                {
                    spans.Add(span);
                }
            }

            var min = spans.Count == 0 ? 0 : spans.Min(s => s.X);
            var max = spans.Count == 0 ? 0 : spans.Max(x => x.Y);

            m_min = min;
            m_max = max;

            Queue<Vec2> left = new Queue<Vec2>(spans);
            HashSet<Vec2> eaten = new();

            while(left.Count > 1)
            {
                var span = left.Dequeue();
                if (eaten.Contains(span))
                    continue;
                
                foreach(var other in left)
                {
                    (var success, Vec2 combined) = CollapseSpan(span, other);
                    if(success)
                    {
                        eaten.Add(other);
                        span = combined;
                    }
                }

                left.Enqueue(span);
            }

            int sum2 = max - min + 1;

            int sum = 0;
            foreach(var span in left)
            {
                sum += span.Y - span.X + 1;
            }

            var possible = m_sensors.Values.Where(s => s.Y == yVal).ToHashSet();

            Console.WriteLine($"Min is {min}, Max is {max}");
            foreach(var s in possible)
            {
                Console.WriteLine($"  Beacon at {s} is inside span: {s.X >= min && s.X <= max}");
            }

            sum -= possible.Count;

            return sum.ToString();
        }

        int m_min;
        int m_max;

        public override string Part2()
        {
            return "";
        }
    }
}
