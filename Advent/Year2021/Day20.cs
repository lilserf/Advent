using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2021
{
    internal class Day20 : DayLineLoaderBasic
    {
        IList<bool> m_algorithm = null;

        Dictionary<Vec2, bool> m_map = new Dictionary<Vec2, bool>();

        int m_currLine = 0;

        public override void ParseInputLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return;
            }

            if (m_algorithm == null)
            {
                m_algorithm = line.Select(c => c == '#' ? true: false).ToList();
                return;
            }

            int col = 0;
            foreach(var c in line)
            {
                m_map[new Vec2(col, m_currLine)] = (c == '#' ? true : false);
                col++;
            }
            m_currLine++;
        }

        public int GetKernel(Dictionary<Vec2, bool> input, Vec2 point, bool outside)
        {
            Vec2[] offsets =
            {
                Vec2.UpLeft, Vec2.Up, Vec2.UpRight, Vec2.Left, Vec2.Zero, Vec2.Right, Vec2.DownLeft, Vec2.Down, Vec2.DownRight,
            };

            int output = 0;
            foreach(var d in offsets)
            {
                bool val = outside;
                var pt = point + d;
                if(input.ContainsKey(pt))
                {
                    val = input[pt];
                }

                output |= (val ? 1 : 0);
                output = output << 1;
            }

            return output >> 1;
        }

        Dictionary<Vec2, bool> CalculateOutputImage(Dictionary<Vec2, bool> input, IList<bool> algo, bool outside)
        {
            Dictionary<Vec2, bool> output = new Dictionary<Vec2, bool>();

            Vec2 min = input.Keys.MinBy(x => x.X + x.Y);
            Vec2 max = input.Keys.MaxBy(x => x.X + x.Y);

            min += Vec2.UpLeft;
            max += Vec2.DownRight;

            for(int i=min.X; i<=max.X; i++)
            {
                for(int j=min.Y;j<=max.Y; j++)
                {
                    Vec2 pt = new Vec2(i, j);
                    var k = GetKernel(input, pt, outside);
                    output[pt] = algo[k];
                }
            }
            
            return output;
        }

        public override string Part1()
        {
            var o = CalculateOutputImage(m_map, m_algorithm, false);

            var o2 = CalculateOutputImage(o, m_algorithm, true);

            return o2.Values.Count(x=> x).ToString();
        }

        public override string Part2()
        {
            // From looking at the algorithm, it's clear that the infinite grid will swap from .... to #### every time you run it
            // So we pass "what are the outside pixels set to" as an extra param
            var curr = m_map;
            for(int i=0; i < 50; i++)
            {
                var newCurr = CalculateOutputImage(curr, m_algorithm, i % 2 == 1);
                curr = newCurr;
            }

            return curr.Values.Count(x => x).ToString();
        }
    }
}
