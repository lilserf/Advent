using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2023
{
    internal class Day03 : DayLineLoaderBasic
    {
        private Dictionary<Vec2, char> m_symbols = new();

        private List<(Vec2, int)> m_partNums = new();

        public override void ParseInputLine(string line, int lineNum)
        {
            for(int i=0; i < line.Length; i++)
            {
                if (line[i] == '.')
                    continue;
                if (line[i] <= '9' && line[i] >= '0')
                {
                    int j = i + 1;
                    while (j < line.Length && line[j] <= '9' && line[j] >= '0')
                    {
                        j++;
                    }
                    int val = int.Parse(line.Substring(i, j-i));
                    m_partNums.Add((new Vec2(lineNum, i), val));
                    i = j-1; // Because we'll increment anyway
                }
                else
                    m_symbols[new Vec2(lineNum, i)] = line[i];
            }
        }

        private (bool, Vec2) CheckNeighbors(Vec2 pos, int width)
        {
            for (int i = 0; i < width; i++)
            {
                foreach (var diff in Vec2.Adjacent)
                {
                    Vec2 target = pos + new Vec2(0, i) + diff;
                    if (m_symbols.ContainsKey(target))
                    {
                        //Console.WriteLine($"Found {m_symbols[target]} at {target}!");
                        return (true, target);
                    }
                }
            }

            return (false, Vec2.Zero);
        }

        List<int> m_validParts = new();

        Dictionary<Vec2, List<int> > m_gears = new();

        public override string Part1()
        {

            foreach(var entry in m_partNums)
            {
                int width = entry.Item2.ToString().Length;

                var result = CheckNeighbors(entry.Item1, width);
                if (result.Item1)
                {
                    m_validParts.Add(entry.Item2);
                    if (m_symbols[result.Item2] == '*')
                    {
                        if(!m_gears.ContainsKey(result.Item2))
                        {
                            m_gears[result.Item2] = new List<int>();
                        }

                        m_gears[result.Item2].Add(entry.Item2);
                    }
                }
            }

            return m_validParts.Sum().ToString();
        }

        public override string Part2()
        {
            var filtered = m_gears.Where(x => x.Value.Count() == 2);
            return filtered.Select(x => x.Value.Aggregate(1, (acc, val) => acc * val)).Sum().ToString();
        }
    }
}
