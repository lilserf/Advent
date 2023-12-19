using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2023
{
    internal class Day10 : DayLineLoaderBasic
    {
        Dictionary<Vec2, char> m_map = new();
        Dictionary<Vec2, int> m_distance = new();
        Vec2 m_start;

        public override void ParseInputLine(string line, int lineNum)
        {
            int col = 0;
            foreach (char c in line)
            {
                var pos = new Vec2(col, lineNum);
                m_map[pos] = c;
                if (c == 'S')
                    m_start = pos;
                col++;
            }
        }


        Dictionary<char, (Vec2, Vec2)> TILES = new ()
        {
            { '|', (Vec2.Up, Vec2.Down) },
            { '-', (Vec2.Left, Vec2.Right) },
            { 'L', (Vec2.Up, Vec2.Right) },
            { 'J', (Vec2.Up, Vec2.Left) },
            { '7', (Vec2.Down, Vec2.Left) },
            { 'F', (Vec2.Down, Vec2.Right) },
        };


        Vec2 MoveThrough(Vec2 position, Vec2 prevPosition)
        {
            var edge = prevPosition - position;
            char c = m_map[position];

            var choose = (Vec2 edge, Vec2 first, Vec2 second) =>
            {
                if (edge == first)
                    return position + second;
                else
                    return position + first;
            };

            var dirs = TILES[c];
            return choose(edge, dirs.Item1, dirs.Item2);
        }

        public override string Part1()
        {
            m_distance[m_start] = 0;

            Queue<(Vec2, Vec2)> states = new();

            foreach (var pos in Vec2.CardinalAdjacent)
            {
                var check = pos + m_start;

                if (m_map.ContainsKey(check) && m_map[check] != '.')
                {
                    var diff = m_start - check;
                    var dirs = TILES[m_map[check]];

                    if (dirs.Item1 == diff || dirs.Item2 == diff)
                    {
                        states.Enqueue((check, m_start));
                        m_distance[check] = 1;
                    }
                }
            }


            while(true)
            {
                var state = states.Dequeue();

                var newPos = MoveThrough(state.Item1, state.Item2);
                Console.WriteLine($"Move from {state.Item2} to {state.Item1} ends up in {newPos}");

                if(m_distance.ContainsKey(newPos))
                    break;
                m_distance[newPos] = m_distance[state.Item1] + 1;
                Console.WriteLine($"  Distance is {m_distance[newPos]}");

                state.Item2 = state.Item1;
                state.Item1 = newPos;
                states.Enqueue(state);
            }

            return m_distance.Values.Max().ToString();
        }

        public override string Part2()
        {
            // Ugh I don't wanna right now
            return "";
        }
    }
}
