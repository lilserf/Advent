using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2023
{
    internal class Day01 : DayLineLoaderBasic
    {
        List<int> m_values = new();
        List<int> m_partAValues = new();

        static string[] DIGITS =
        {
            "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0"
        };

        static Dictionary<string, int> DIGIT_VALUES = new Dictionary<string, int>()
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8 },
            { "nine", 9 },
            { "1", 1 },
            { "2", 2 },
            { "3", 3 },
            { "4", 4 },
            { "5", 5 },
            { "6", 6 },
            { "7", 7 },
            { "8", 8 },
            { "9", 9 },
            { "0", 0 },
        };

        private void oldParseInput(string line)
        {
            int i = 0;
            int firstVal = -1;
            while (!int.TryParse(line.AsSpan().Slice(i, 1), out firstVal))
            {
                i++;
            }

            int lastVal = -1;
            i = line.Length - 1;
            while (!int.TryParse(line.AsSpan().Slice(i, 1), out lastVal))
            {
                i--;
            }

            if (firstVal == -1 || lastVal == -1) throw new InvalidOperationException();

            int val = int.Parse($"{firstVal}{lastVal}");
            m_partAValues.Add(val);
        }

        public override void ParseInputLine(string line, int lineNum)
        {
            oldParseInput(line);
            List<(int, string)> m_found = new List<(int, string)>();
            foreach(var d in DIGITS)
            {
                int firstIndex = line.IndexOf(d);
                int lastIndex = line.LastIndexOf(d);

                if(firstIndex > -1) m_found.Add((firstIndex, d));
                if(lastIndex > -1) m_found.Add((lastIndex, d));
            }

            var min = m_found.Min();
            var max = m_found.Max();

            int val = int.Parse($"{DIGIT_VALUES[min.Item2]}{DIGIT_VALUES[max.Item2]}");
            m_values.Add(val);
        }

        public override string Part1()
        {
            return m_partAValues.Sum().ToString();
        }

        public override string Part2()
        {
            return m_values.Sum().ToString();
        }
    }
}
