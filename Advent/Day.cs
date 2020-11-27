using System;

namespace Advent
{
    class Day : IDay
    {
        private readonly IDayStrategy m_strategy;

        public Day(IDayStrategy strategy)
        {
            m_strategy = strategy;
        }

        public void Initialize() => m_strategy.Initialize();
        public void Reset() => m_strategy.Reset();
        public override string ToString() => m_strategy.ToString();

        public void Part1()
        {
            string result = m_strategy.Part1();
            Console.WriteLine("Part 1 result:");
            Console.WriteLine(result);
            Console.WriteLine();
        }

        public void Part2()
        {
            string result = m_strategy.Part2();
            Console.WriteLine("Part 2 result:");
            Console.WriteLine(result);
            Console.WriteLine();
        }
    }
}
