using Advent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent2020.Year2022
{
    internal class Day14 : DayLineLoaderBasic
    {
        Dictionary<Vec2, char> m_map = new();

        public override void ParseInputLine(string line)
        {
            var split = line.Split(" -> ");

            for(int i=1; i < split.Length; i++)
            {
                Vec2 start = Vec2.Parse(split[i-1]);
                Vec2 end = Vec2.Parse(split[i]);
                Vec2 step = (end - start).ClampToUnit();

                var curr = start;
                while(curr != end)
                {
                    m_map[curr] = '#';
                    curr += step;
                }
                m_map[end] = '#';
            }
        }

        public override void ParseInputLinesEnd(StreamReader sr)
        {
            m_map[new Vec2(500, 0)] = '+';
        }

        int m_abyss;

        public void DrawMap()
        {
            int minX = m_map.Keys.Min(x => x.X);
            int maxX = m_map.Keys.Max(x => x.X);

            int minY = m_map.Keys.Min(x => x.Y);
            int maxY = m_map.Keys.Max(x => x.Y);

            m_abyss = maxY + 2;

            for(int y = minY; y <= maxY; y++)
            {
                for(int x = minX; x <= maxX; x++)
                {
                    var curr = new Vec2(x, y);

                    if (m_map.ContainsKey(curr))
                    {
                        Console.Write($"{m_map[curr]}");
                    }
                    else
                    {
                        Console.Write(".");
                    }
                }
                Console.WriteLine();
            }
        }

        char GetMapChar(Vec2 pos)
        {
            if (m_map.ContainsKey(pos))
                return m_map[pos];
            else
                return '.';
        }

        bool IsBlocked(Vec2 pos, bool floor)
        {
            char c = GetMapChar(pos);
            
            return c == '#' || c == 'o' || (floor && pos.Y == m_abyss);
        }

        bool SimulateSand(bool floor)
        {
            Vec2 curr = new Vec2(500, 0);

            while(curr.Y < m_abyss)
            {
                var down = curr + Vec2.Down;
                var downLeft = curr + Vec2.DownLeft;
                var downRight = curr + Vec2.DownRight;

                if(IsBlocked(down, floor))
                {
                    if(IsBlocked(downLeft, floor))
                    {
                        if(IsBlocked(downRight, floor))
                        {
                            m_map[curr] = 'o';
                            return true;
                        }
                        else
                        {
                            curr = downRight;
                        }
                    }
                    else
                    { 
                        curr = downLeft;
                    }
                }
                else
                {
                    curr = down;
                }
            }

            return false;
        }

        int m_part1Count = 0;

        public override string Part1()
        {
            DrawMap();

            bool rest = true;
            int count = 0;
            while(rest)
            {
                rest = SimulateSand(false);
                count++;
            }

            DrawMap();
            m_part1Count = count - 1;
            return m_part1Count.ToString();
        }

        public override string Part2()
        {
            Vec2 src = new Vec2(500, 0);
            char source = m_map[src];

            int count = m_part1Count;
            while(source == '+')
            {
                SimulateSand(true);
                count++;
                source = m_map[src];
            }

            DrawMap();
            return count.ToString();
        }
    }
}
