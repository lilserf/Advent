using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent2020.Year2022
{
    internal class Day8 : DayMapInputFileStrategy
    {
        Dictionary<Vec2, int> m_map = new();
        private int SIZE = 0;

        public Day8(string input)
            : base(input)
        {

        }

        private int NumVisible(Vec2 start, Vec2 step)
        {
            Vec2 curr = start;
            int numVisible = 0;
            int maxHeight = -1;

            while(m_map.ContainsKey(curr))
            {
                int currVal = m_map[curr];
                if (currVal > maxHeight)
                {
                    numVisible++;
                    maxHeight = currVal;
                }
                if (currVal == 9)
                    break;

                curr += step;

            }

            return numVisible;
        }

        private bool IsBlocked(Vec2 pos, Vec2 step)
        {
            var value = m_map[pos];
            var curr = pos + step;

            while(m_map.ContainsKey(curr))
            {
                if (m_map[curr] >= value)
                {
                    return true;
                }
                curr += step;
            }

            return false;
        }

        private bool IsTreeVisible(Vec2 pos)
        {
            return
                !IsBlocked(pos, Vec2.Left) ||
                !IsBlocked(pos, Vec2.Right) ||
                !IsBlocked(pos, Vec2.Up) ||
                !IsBlocked(pos, Vec2.Down);
        }

        private int ViewDistance(Vec2 pos, Vec2 step)
        {
            var value = m_map[pos];
            var curr = pos + step;
            var distance = 0;

            while (m_map.ContainsKey(curr))
            {
                if (m_map[curr] >= value)
                {
                    return distance + 1;
                }
                curr += step;
                distance++;
            }

            return distance;
        }

        private int SceneScore(Vec2 pos)
        {
            return ViewDistance(pos, Vec2.Up) * 
                ViewDistance(pos, Vec2.Down) *
                ViewDistance(pos, Vec2.Left) *
                ViewDistance(pos, Vec2.Right);
        }


        public override string Part1()
        {
            int total = (2 * SIZE) + (2 * (SIZE -2));

            for(int i=1; i < SIZE-1; i++)
            {
                for(int j=1; j < SIZE-1; j++)
                {
                    total += IsTreeVisible(new Vec2(i, j)) ? 1 : 0;
                }
            }

            return total.ToString();
        }

        public override string Part2()
        {
            int max = 0;

            for (int i = 1; i < SIZE - 1; i++)
            {
                for (int j = 1; j < SIZE - 1; j++)
                { 
                    max = Math.Max(max, SceneScore(new Vec2(i, j)));
                }
            }

            return max.ToString();
        }

        protected override void ReadMapCharacter(Vec2 position, char c)
        {
            m_map[position] = int.Parse($"{c}");
            SIZE = Math.Max(SIZE, position.X+1);
        }
    }
}
