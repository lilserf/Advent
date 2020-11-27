using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent
{
    class Program
    {
        // if true will run just the first day in s_allDays
        // if false will show a menu to choose which day to run
        private static bool s_runFirstDay = true;

		private static readonly IDay[] s_allDays =
		{
            // 2 types of strategies are provided for Days, or you can write your own. Or you can just implement IDay.
            // new Day(new WhateverStrategy()),
			
			//new Day(new Year2019.Day25Strategy("../../inputs/2019/day25.txt")),
			new Day(new Year2019.Day24Strategy("../../inputs/2019/day24.txt")),
			new Day(new Year2019.Day23Strategy("../../inputs/2019/day23.txt")),
			new Day(new Year2019.Day22Strategy("../../inputs/2019/day22.txt")),
			new Day(new Year2019.Day21Strategy("../../inputs/2019/day21.txt")),
			new Day(new Year2019.Day20Strategy("../../inputs/2019/day20.txt")),
			new Day(new Year2019.Day19Strategy("../../inputs/2019/day19.txt")),
			new Day(new Year2019.Day18Strategy2("../../inputs/2019/day18.txt")), // <-- Second attempt, heh
			new Day(new Year2019.Day17Strategy("../../inputs/2019/day17.txt")),
			new Day(new Year2019.Day16Strategy("../../inputs/2019/day16.txt")),
			new Day(new Year2019.Day15Strategy("../../inputs/2019/day15.txt")),
			new Day(new Year2019.Day14Strategy("../../inputs/2019/day14.txt")),
			new Day(new Year2019.Day13Strategy("../../inputs/2019/day13.txt")),
			new Day(new Year2019.Day12Strategy("../../inputs/2019/day12.txt")),
			new Day(new Year2019.Day11Strategy("../../inputs/2019/day11.txt")),
			new Day(new Year2019.Day10Strategy("../../inputs/2019/day10.txt")),
			new Day(new Year2019.Day09Strategy("../../inputs/2019/day9.txt")),
			new Day(new Year2019.Day08Strategy("../../inputs/2019/day8.txt")),
			new Day(new Year2019.Day07Strategy("../../inputs/2019/day7.txt")),
			new Day(new Year2019.Day06Strategy("../../inputs/2019/day6.txt")),
			new Day(new Year2019.Day05Strategy("../../inputs/2019/day5.txt")),
			new Day(new Year2019.Day04Strategy(402328,864247)),
			new Day(new Year2019.Day03Strategy("../../inputs/2019/day3.txt")),
			new Day(new Year2019.Day02Strategy("../../inputs/2019/day2.txt")),
			new Day(new Year2019.Day01Strategy("../../inputs/2019/day1.txt")),
			//new Year2018.Day24(),
			//new Day(new Year2018.Day1Strategy("../../inputs/2018/day1.txt")),
        };

        static void Main(string[] args)
        {
            ReflectionRunner rr = new ReflectionRunner();
            rr.RunLatest();

            //IEnumerable<IDay> daysToRun = null;
            
            //if (s_runFirstDay)
            //{
            //    daysToRun = new IDay[] { s_allDays[0] };
            //}

            //if (daysToRun != null)
            //{
            //    int dayCount = daysToRun.Count();
            //    foreach (var day in daysToRun)
            //    {
            //        Console.WriteLine($"Running tests for {day.ToString()}");
            //        Console.WriteLine();
            //        day.Initialize();
            //        if (dayCount == 1)
            //        {
            //            RunAndProfile(day);
            //        }
            //        else
            //        {
            //            Run(day);
            //        }
            //        Console.WriteLine();
            //    }
            //    Console.WriteLine("Press any key to continue . . .");
            //    Console.ReadKey();
            //}
            //else
            //{
            //    bool keepGoing = true;
            //    var initialized = new HashSet<IDay>();
            //    int selection = 0;
            //    while (keepGoing)
            //    {
            //        selection = Menu.RunMenu(s_allDays, selection);

            //        if (selection < 0)
            //        {
            //            keepGoing = false;
            //        }
            //        else
            //        {
            //            var day = s_allDays[selection];

            //            Console.Clear();
            //            Console.WriteLine($"Running tests for {day.ToString()}");
            //            Console.WriteLine();

            //            if (!initialized.Contains(day))
            //            {
            //                initialized.Add(day);
            //                day.Initialize();
            //            }
            //            RunAndProfile(day);

            //            Console.WriteLine();
            //            Console.WriteLine("Press any key to continue . . .");
            //            Console.ReadKey();
            //        }
            //    }
            //}
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
