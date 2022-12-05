using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent2020.Year2022
{
    internal class Day5 : DayLineLoaderBasic
    {
        Stack<char>[] m_stacks = new Stack<char>[9];

        List<(int qty, int from, int to)> m_moves = new();

        public override void ParseInputLine(string line)
        {
            var match = MatchLine(line, new Regex(@"move (\d+) from (\d+) to (\d+)"));
            if(match.Success)
            {
                int qty = int.Parse(match.Groups[1].Value);
                int from = int.Parse(match.Groups[2].Value);
                int to = int.Parse(match.Groups[3].Value);
                m_moves.Add((qty, from, to));
            }
        }

        public void Push(int i, char c)
        {
            if (m_stacks[i-1] == null)
                m_stacks[i-1] = new Stack<char>();
            m_stacks[i-1].Push(c);
        }

        public void Push(int i, string s)
        {
            foreach(var c in s)
            {
                Push(i, c);
            }
        }

        public void Setup()
        {
            m_stacks = new Stack<char>[9];

            Push(1, "HRBDZFLS");
            Push(2, "TBMZR");
            Push(3, "ZLCHNS");
            Push(4, "SCFJ");
            Push(5, "PGHWRZB");
            Push(6, "VJZGDNMT");
            Push(7, "GLNWFSPQ");
            Push(8, "MZR");
            Push(9, "MCLGVRT");
        }

        public char Pop(int i)
        {
            return m_stacks[i - 1].Pop();
        }

        public void Move(int from, int to)
        {
            Push(to, Pop(from));
        }

        public override string Part1()
        {
            Setup();

            foreach ((int qty, int from, int to) in m_moves)
            {
                for(int i=0; i <qty; i++)
                {
                    Move(from, to);
                }
            }

            string s = "";
            foreach(var stack in m_stacks)
            {
                s += stack.Peek();
            }

            return s;
        }

        public override string Part2()
        {
            Setup();

            foreach ((int qty, int from, int to) in m_moves)
            {
                Stack<char> temp = new();
                for (int i = 0; i < qty; i++)
                {
                    temp.Push(Pop(from));
                }

                while(temp.Count> 0)
                {
                    Push(to, temp.Pop());
                }
            }

            string s = "";
            foreach (var stack in m_stacks)
            {
                s += stack.Peek();
            }

            return s;
        }
    }
}
