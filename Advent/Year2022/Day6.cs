using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2022
{
    internal class Day6 : DayLineLoaderBasic
    {
        string m_input;

        public override void ParseInputLine(string line, int lineNum)
        {
            m_input = line;
        }

        public int FirstUniquePacket(string input, int length)
        {
            Queue<char> recent = new();
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                recent.Enqueue(c);
                if (recent.Count == length)
                {
                    if (recent.Distinct().Count() == length)
                        return (i + 1);
                    recent.Dequeue();
                }
            }

            throw new InvalidOperationException();
        }

        public override string Part1()
        {
            //var input = "nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg";
            return FirstUniquePacket(m_input, 4).ToString();
        }

        public override string Part2()
        {
            return FirstUniquePacket(m_input, 14).ToString();
        }
    }
}
