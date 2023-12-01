using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2022
{
    internal class Day12 : DayMapInputFileStrategy
    {
        Dictionary<Vec2, int> m_map = new();
        Vec2 m_start;
        Vec2 m_end;

        public Day12(string inputFile)
            : base(inputFile)
        {

        }

        protected override void ReadMapCharacter(Vec2 position, char c)
        {
            if(c >= 'a' && c <= 'z')
            {
                m_map[position] = Math.Abs('a' - c);
            }
            else if(c == 'S')
            {
                m_map[position] = 0;
                m_start = position;
            }
            else if(c == 'E')
            {
                m_map[position] = 25;
                m_end = position;
            }
        }

        struct State : IComparable<State>
        {
            public State(Vec2 position, int steps)
                : this(position)
            {
                Steps = steps;
            }
            public State(Vec2 position)
            {
                Position = position;
            }
            public Vec2 Position { get; set; }
            public int Steps { get; set; }

            public int CompareTo(State other)
            {
                return Steps - other.Steps;
            }
        }

        int FindBestPath(MinHeap<State> heap)
        {
            HashSet<Vec2> visited = new();

            while (heap.Count() > 0)
            {
                State curr = heap.Pop();
                visited.Add(curr.Position);

                var currHeight = m_map[curr.Position];
                foreach (var step in Vec2.CardinalAdjacent)
                {
                    Vec2 next = curr.Position + step;
                    if (!m_map.ContainsKey(next))
                        continue;

                    if (visited.Contains(next))
                    {
                        continue;
                    }

                    if (next == m_end)
                    {
                        return (curr.Steps + 1);
                    }

                    var stepHeight = m_map[next];
                    if (stepHeight - currHeight <= 1)
                    {
                        visited.Add(next);
                        heap.Add(new State(next, curr.Steps + 1));
                    }
                }
            }

            throw new InvalidOperationException();
        }

        public override string Part1()
        {
            MinHeap<State> heap = new(10000);
            heap.Add(new State(m_start));

            return FindBestPath(heap).ToString();
        }

        public override string Part2()
        {
            MinHeap<State> heap = new(10000);

            foreach(var pos in m_map.Keys)
            {
                if (m_map[pos] == 0)
                {
                    heap.Add(new State(pos));
                }
            }

            return FindBestPath(heap).ToString();
        }

    }
}
