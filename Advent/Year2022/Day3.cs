using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2022
{

    internal class Day3 : DayLineLoaderBasic
    {
        List<string> m_lines = new();

        public override void ParseInputLine(string line, int lineNum)
        {
            m_lines.Add(line);
        }

        public override string Part1()
        {
            int sum = 0;
            foreach(string line in m_lines)
            {
                string part1 = line.Substring(0, line.Length / 2);
                string part2 = line.Substring(line.Length / 2);

                var common = part1.Intersect(part2).First();
                var prio = GetPrio(common);
                sum += prio;
            }

            return sum.ToString();
        }

        int GetPrio(char common)
        {
            return (common >= 'a' && common <= 'z') ? common - 'a' + 1 : common - 'A' + 27;
        }

        public override string Part2()
        {
            int i = 0;
            int sum = 0;
            while(i < m_lines.Count)
            {
                var l1 = m_lines[i];
                var l2 = m_lines[i + 1];
                var l3 = m_lines[i + 2];

                var badge = l1.Intersect(l2).Intersect(l3).First();
                sum += GetPrio(badge);

                i += 3;
            }

            return sum.ToString();
        }
    }
}
