using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent.Year2022
{
    internal class Day15 : DayLineLoaderBasic
    {
        Dictionary<Vec2, Vec2> m_sensors = new();
        Dictionary<Vec2, int> m_distances = new();

        public override void ParseInputLine(string line, int lineNum)
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

        HashSet<Vec2> GetSpansForY(int yVal, int xMin, int xMax)
        {
            HashSet<Vec2> spans = new();

            foreach (var (s, b) in m_sensors)
            {
                var dist = Vec2.ManhattanDistance(s, b);
                m_distances[s] = dist;

                (var success, Vec2 span) = GetSpanAtDistanceAndY(s, dist, yVal);
                if (success)
                {
                    if (span.Y > xMax)
                        span.Y = xMax;
                    if (span.X < xMin)
                        span.X = xMin;
                    spans.Add(span);
                }
            }

            return spans;
        }

        int NumEliminatedSpaces(int yVal, int xMin, int xMax)
        {
            HashSet<Vec2> spans = GetSpansForY(yVal, xMin, xMax);

            Queue<Vec2> left = new Queue<Vec2>(spans);
            HashSet<Vec2> eaten = new();

            int timeout = spans.Count;
            int currTime = timeout;
            while (left.Count > 1)
            {
                if(currTime == 0)
                {
                    break;
                }

                var span = left.Dequeue();
                if (eaten.Contains(span))
                    continue;

                foreach (var other in left)
                {
                    (var success, Vec2 combined) = CollapseSpan(span, other);
                    if (success)
                    {
                        currTime = timeout;
                        eaten.Add(other);
                        span = combined;
                    }
                }

                left.Enqueue(span);
                currTime--;
            }

            int sum = 0;
            foreach (var span in left)
            {
                sum += span.Y - span.X + 1;
            }

            return sum;
        }

        public override string Part1()
        {
            //int yVal = 10;
            int yVal = 2000000;

            //int yVal = 8;

            int sum = NumEliminatedSpaces(yVal, -4000000, 8000000);
 
            var possible = m_sensors.Values.Where(s => s.Y == yVal).ToHashSet();

            //Console.WriteLine($"Min is {min}, Max is {max}");
            //foreach(var s in possible)
            //{
            //    Console.WriteLine($"  Beacon at {s} is inside span: {s.X >= min && s.X <= max}");
            //}

            sum -= possible.Count;

            return sum.ToString();
        }

        public override string Part2()
        {
            int yLimit = 4000000;
            //int yLimit = 20;
            int xMax = 4000000;
            //int xMax = 20;

            int foundY = 0;
            for (int y = 0; y < yLimit; y++)
            {
                int elim = NumEliminatedSpaces(y, 0, xMax);

                if(elim < xMax+1)
                {
                    foundY = y;
                    break;
                }
            }

            var spans = GetSpansForY(foundY, 0, xMax);
            for(int x = 0; x < xMax; x++)
            {
                if (spans.All(s => s.X > x || s.Y < x))
                    return ((long)x * 4000000 + foundY).ToString();
            }

            return "";
        }

    }
}
