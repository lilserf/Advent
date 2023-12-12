using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2023
{
    internal class Day09 : DayLineLoaderBasic
    {
        List<List<int>> m_sequences = new();

        public override void ParseInputLine(string line, int lineNum)
        {
            var s = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var nums = s.Select(int.Parse).ToList();
            m_sequences.Add(nums);
        }

        IEnumerable<int> Diff(IEnumerable<int> nums)
        {
            for(int i=0; i < nums.Count()-1; i++)
            {
                yield return nums.ElementAt(i+1) - nums.ElementAt(i);
            }
        }

        int Extrapolate(IEnumerable<int> sequence)
        {
            Stack<int> lastDigits = new();

            while(sequence.Any(x => x != 0))
            {
                lastDigits.Push(sequence.Last());
                sequence = Diff(sequence).ToList();
            }

            int val = 0;
            while(lastDigits.Count() > 0)
            {
                val = val + lastDigits.Pop();
            }

            return val;
        }

        public override string Part1()
        {
            int sum = 0;

            foreach(var seq in m_sequences)
            {
                int val = Extrapolate(seq);

                Console.WriteLine($"Extrapolated value is {val}");

                sum += val;
            }

            return sum.ToString();
        }

        int ExtrapolateB(IEnumerable<int> sequence)
        {
            Stack<int> firstDigits = new();

            while (sequence.Any(x => x != 0))
            {
                firstDigits.Push(sequence.First());
                sequence = Diff(sequence).ToList();
            }

            int val = 0;
            while (firstDigits.Count() > 0)
            {
                val = firstDigits.Pop() - val;
            }

            return val;
        }

        public override string Part2()
        {
            int sum = 0;

            foreach (var seq in m_sequences)
            {
                int val = ExtrapolateB(seq);

                Console.WriteLine($"Extrapolated value is {val}");

                sum += val;
            }

            return sum.ToString();
        }
    }
}
