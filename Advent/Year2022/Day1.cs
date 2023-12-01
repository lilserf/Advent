using Advent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2022
{
    internal class Day1 : DayLineLoaderBasic
    {
        List<List<int>> m_elves = new();
        List<int> m_curr = new();

        public override void ParseInputLine(string line)
        {
            if(string.IsNullOrEmpty(line))
            {
                m_elves.Add(m_curr);
                m_curr = new();
            }
            else
            {
                m_curr.Add(int.Parse(line));
            }

        }

        public override void ParseInputLinesEnd(StreamReader sr)
        {
            base.ParseInputLinesEnd(sr);
            m_elves.Add(m_curr);
        }

        public override string Part1()
        {
            return m_elves.Max(x => x.Sum()).ToString();
        }

        public override string Part2()
        {
            return m_elves.Select(x => x.Sum()).OrderDescending().Take(3).Sum().ToString();
        }
    }
}
