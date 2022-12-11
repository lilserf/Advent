using Advent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent2020.Year2022
{
    public struct Item
    {
        static int IdCounter = 0;
        public Item(long worry)
        {
            Id = IdCounter++;
            WorryLevel = worry;
        }

        public Item(Item other)
        {
            Id = other.Id;
            WorryLevel = other.WorryLevel;
        }
        public int Id { get; }
        public long WorryLevel { get; set; }
    }
    class Monkey
    {
        public Monkey(int id)
        {
            Id = id;
        }

        public Monkey(Monkey other)
        {
            Id = other.Id;
            InspectCount = other.InspectCount;
            Operation = other.Operation;
            TestDivisibleBy= other.TestDivisibleBy;
            TrueMonkey= other.TrueMonkey;
            FalseMonkey= other.FalseMonkey;
            foreach(var item in other.Items)
            {
                Items.Enqueue(new Item(item));
            }
        }

        public int Id { get; set; }
        public Queue<Item> Items { get; set; } = new Queue<Item>();

        public long InspectCount { get; set; } = 0;

        public Func<long, long> Operation { get; set; }
        public int TestDivisibleBy;
        public int TrueMonkey;
        public int FalseMonkey;
    }

    internal class Day11 : DayLineLoaderBasic
    {
        Dictionary<int, Monkey> m_monkeys = new();

        Monkey m_curr;
        public override void ParseInputLine(string line)
        {
            if(line.StartsWith("Monkey "))
            {
                var id = int.Parse(line.Substring(7, line.IndexOf(':') - 7));
                if(m_curr != null)
                    m_monkeys[m_curr.Id] = m_curr;
                m_curr = new Monkey(id);
            }

            if(line.StartsWith("  Starting items:"))
            {
                var nums = line.Substring(17).Split(',').Select(x => int.Parse(x));
                foreach(var n in nums)
                {
                    m_curr.Items.Enqueue(new Item(n));
                }
            }

            if(line.StartsWith("  Test: divisible by "))
            {
                var num = int.Parse(line.Substring(21));
                m_curr.TestDivisibleBy = num;
            }

            if(line.StartsWith("    If true: throw to monkey "))
            {
                var num = int.Parse(line.Substring(29));
                m_curr.TrueMonkey = num;
            }

            if (line.StartsWith("    If false: throw to monkey "))
            {
                var num = int.Parse(line.Substring(30));
                m_curr.FalseMonkey = num;
            }

        }

        long magicMod = 1;

        public override void ParseInputLinesEnd(StreamReader sr)
        {
            base.ParseInputLinesEnd(sr);
            m_monkeys[m_curr.Id] = m_curr;

            foreach(var m in m_monkeys.Values)
            {
                magicMod *= m.TestDivisibleBy;
            }
        }

        void SetupMonkey(int id, Func<long, long> operation)
        {
            if(m_monkeys.ContainsKey(id))
            {
                m_monkeys[id].Operation = operation;
            }
        }


        void RealSetup()
        {
            SetupMonkey(0, (old => old * 11));
            SetupMonkey(1, (old => old + 8));
            SetupMonkey(2, (old => old * 3));
            SetupMonkey(3, (old => old + 4));
            SetupMonkey(4, (old => old * old));
            SetupMonkey(5, (old => old + 2));
            SetupMonkey(6, (old => old + 3));
            SetupMonkey(7, (old => old + 5));
        }

        void TestSetup()
        {
            SetupMonkey(0, (old => old * 19));
            SetupMonkey(1, (old => old + 6));
            SetupMonkey(2, (old => old * old));
            SetupMonkey(3, (old => old + 3));
        }

        void RunMonkey(Dictionary<int, Monkey> monkeys, int id, bool reduceWorry = true)
        {
            Monkey m = monkeys[id];
            while(m.Items.Count> 0)
            {
                var item = m.Items.Dequeue();
                m.InspectCount++;
                item.WorryLevel = m.Operation(item.WorryLevel);

                if (reduceWorry)
                    item.WorryLevel /= 3;

                if (item.WorryLevel < 0)
                    Debugger.Break();
                if (item.WorryLevel % m.TestDivisibleBy == 0)
                {
                    monkeys[m.TrueMonkey].Items.Enqueue(item);
                }
                else
                {
                    item.WorryLevel = item.WorryLevel % magicMod;
                    monkeys[m.FalseMonkey].Items.Enqueue(item);
                }
            }
        }

        void RunRound(Dictionary<int, Monkey> monkeys, bool reduceWorry = true)
        {
            foreach(var (id, m) in monkeys)
            {
                RunMonkey(monkeys, id, reduceWorry);
            }
        }

        public void PrintInspectionCount(Dictionary<int, Monkey> monkeys, int round)
        {
            Console.WriteLine($"== After round {round} ==");
            foreach (var m in monkeys.Values)
            {
                Console.WriteLine($"Monkey {m.Id} inspected items {m.InspectCount} times.");
            }
        }

        public override string Part1()
        {
            RealSetup();

            Dictionary<int, Monkey> monkeys = new();
            foreach(var m in m_monkeys.Values)
            {
                monkeys[m.Id] = new Monkey(m);
            }

            for(int i=0; i < 20; i++)
            {
                RunRound(monkeys);
            }

            PrintInspectionCount(monkeys, 20);

            var top2 = monkeys.Values.Select(x => x.InspectCount).OrderDescending().Take(2);
            return (top2.ElementAt(0) * top2.ElementAt(1)).ToString();
        }

        public override string Part2()
        {
            // Reset to initial state
            Dictionary<int, Monkey> monkeys = new();
            foreach (var m in m_monkeys.Values)
            {
                monkeys[m.Id] = new Monkey(m);
            }

            for (int i=0; i < 10000; i++)
            {
                RunRound(monkeys, false);

                if(i % 1000 == 0)
                {
                    PrintInspectionCount(monkeys, i);
                }
                if(i == 20)
                {
                    PrintInspectionCount(monkeys, i);
                }
            }

            PrintInspectionCount(monkeys, 10000);

            var top2 = monkeys.Values.Select(x => x.InspectCount).OrderDescending().Take(2);
            return (top2.ElementAt(0) * top2.ElementAt(1)).ToString();
        }
    }
}
