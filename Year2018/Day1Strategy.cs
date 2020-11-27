using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2018
{
    class Day1Strategy : DayInputFileStrategy
    {
        List<int> m_deltas = new List<int>();

        public Day1Strategy(string file)
            : base(file)
        {
            
        }

        public override string Part1()
        {
            int freq = 0;
            foreach(int delta in m_deltas)
            {
                freq += delta;
            }

            return $"Frequency is {freq}";
        }

        public override string Part2()
        {
            Dictionary<int, int> counts = new Dictionary<int, int>();
            counts[0] = 1;
            bool done = false;

            int freq = 0;
            int index = 0;
            while(!done)
            {
                freq += m_deltas[index];

                if (counts.ContainsKey(freq))
                {
                    counts[freq]++;
                }
                else
                {
                    counts[freq] = 1;
                }

                if (counts[freq] == 2)
                    return $"First frequency hit twice is {freq}";

                index++;

                if(index == m_deltas.Count)
                {
                    index = 0;
                }
            }

            return "Error";
        }


        protected override void ParseInputLine(string line)
        {
            m_deltas.Add(int.Parse(line));
        }
    }
}
