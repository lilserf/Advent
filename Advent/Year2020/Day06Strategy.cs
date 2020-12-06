using Advent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent2020.Year2020
{
    class Day06Strategy : DayInputFileStrategy
    {
        List<HashSet<char>> m_data = new List<HashSet<char>>();
        HashSet<char> m_curr = new HashSet<char>();

        List<List<string>> m_data2 = new List<List<string>>();
        List<string> m_curr2 = new List<string>();

        public Day06Strategy(string file) : base(file)
        {

        }

        protected override void ParseInputLine(string line)
        {
            if(line == "")
            {
                m_data.Add(m_curr);
                m_curr = new HashSet<char>();

                m_data2.Add(m_curr2);
                m_curr2 = new List<string>();
            }
            else
            {
                foreach(char c in line)
                {
                    m_curr.Add(c);
                }
                m_curr2.Add(line);
            }
        }

        protected override void ParseInputLinesEnd(StreamReader sr)
        {
            m_data.Add(m_curr);
            m_data2.Add(m_curr2);
        }

        public override string Part1()
        {
            return m_data.Sum(x => x.Count).ToString();
        }

        public override string Part2()
        {
            int sum = 0;
            foreach(var list in m_data2)
            {
                var combined = list.Aggregate(func: (result, next) => new string(result.Intersect(next).ToArray()));

                sum += combined.Count();
            }

            return sum.ToString();
        }
    }
}
