using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2023
{
    internal class Day11 : DayLineLoaderBasic
    {
        List<string> m_lines = new();

        string m_columns = string.Empty;
        List<int> m_columnJumps = new();
        List<int> m_rowJumps = new();
        Dictionary<int, Vec2> m_galaxies = new();

        public override void ParseInputLine(string line, int lineNum)
        {
            // First add the line
            if(!line.Contains('#'))
            {
                m_rowJumps.Add(lineNum);
            }

            m_lines.Add(line);

            // Then OR the columns
            if (m_columns == string.Empty)
            {
                m_columns = line;
            }
            else
            {
                m_columns = string.Join("", m_columns.Zip(line, (a, b) =>
                {
                    if (a == '#' || b == '#')
                        return '#';
                    else
                        return '.';
                }));
            }

        }


        public override void ParseInputLinesEnd(StreamReader sr)
        {
            List<int> indices = new();

            for(int i=0; i < m_columns.Length; i++)
            {
                if (m_columns[i] == '.')
                    m_columnJumps.Add(i);
            }

            int galNum = 1;
            for(int i=0; i < m_lines.Count; i++)
            {
                for(int j=0; j < m_lines[i].Length; j++)
                {
                    if (m_lines[i][j] == '#')
                    {
                        m_galaxies.Add(galNum, new Vec2(i, j));
                        galNum++;
                    }
                }
            }
        }

        int NumJumps(Vec2 a, Vec2 b)
        {
            int rows = m_rowJumps.Count(x => (a.X < x && x < b.X) || (b.X < x && x < a.X));
            int cols = m_columnJumps.Count(y => (a.Y < y && y < b.Y) || (b.Y < y && y < a.Y));

            return rows + cols;
        }

        public long DoWork(long expandValue)
        {
            long sum = 0;

            for (int i = 0; i < m_galaxies.Count; i++)
            {
                var gal1 = m_galaxies.ElementAt(i);
                for (int j = i; j < m_galaxies.Count; j++)
                {
                    var gal2 = m_galaxies.ElementAt(j);

                    int dist = Vec2.ManhattanDistance(gal1.Value, gal2.Value);

                    //Console.WriteLine($"G{gal1.Key} => G{gal2.Key}: {dist} squares");

                    int jumps = NumJumps(gal1.Value, gal2.Value);
                    //Console.WriteLine($"  {jumps} jumps found");

                    sum += dist + (jumps * (expandValue-1));
                }
            }

            return sum;
        }

        public override string Part1()
        {
            var sum = DoWork(2);

            return $"{sum}";
        }

        public override string Part2()
        {
            var sum = DoWork(1000000);
            return $"{sum}";
        }
    }
}
