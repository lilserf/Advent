using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent
{
    class Program
    {

        static void Main(string[] args)
        {
            int year = -1;
            if(args.Length > 0)
            {
                year = int.Parse(args[0]);
            }

            ReflectionRunner rr = new ReflectionRunner();
            rr.RunLatest(year);
        }

        public static void RunAndProfile(IDay day)
        {
            day.Reset();

            DateTime beforePart1Time = DateTime.Now;
            day.Part1();
            DateTime afterPart1Time = DateTime.Now;

            DateTime beforePart2Time = DateTime.Now;
            day.Part2();
            DateTime afterPart2Time = DateTime.Now;

            Console.WriteLine($"Part 1 took {(afterPart1Time - beforePart1Time).TotalMilliseconds}ms");
            Console.WriteLine($"Part 2 took {(afterPart2Time - beforePart2Time).TotalMilliseconds}ms");
        }

        private static void Run(IDay day)
        {
            day.Reset();
            day.Part1();
            day.Part2();
        }
    }
}
