using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent2020.Year2022
{
    internal class Day9 : DayLineLoaderBasic
    {
        List<(Vec2, int)> m_path = new();

        public override void ParseInputLine(string line)
        {
            var split = line.Split(' ');
            Vec2 dir = Vec2.Zero;
            switch(split[0])
            {
                case "U":
                    dir = Vec2.Up;
                    break;
                case "D":
                    dir = Vec2.Down;
                    break;
                case "L":
                    dir = Vec2.Left;
                    break;
                case "R":
                    dir = Vec2.Right;
                    break;
            }
            int dist = int.Parse(split[1]);
            m_path.Add((dir, dist));
        }

        private void Move(ref Vec2 head, ref Vec2 tail, Vec2 move)
        {
            head = head + move;
            Follow(head, ref tail);
        }

        private void Follow(Vec2 leader, ref Vec2 follower)
        {
            var diff = leader - follower;
            if (Math.Abs(diff.X) > 1 || Math.Abs(diff.Y) > 1)
            {
                var norm = diff.ClampToUnit();
                follower += norm;
            }
        }

        public override string Part1()
        {
            Vec2 head = Vec2.Zero;
            Vec2 tail = Vec2.Zero;
            HashSet<Vec2> tailPositions = new();

            foreach(var step in m_path)
            {
                for(int i=0; i < step.Item2; i++)
                {
                    Move(ref head, ref tail, step.Item1);
                    tailPositions.Add(tail);
                }
            }

            return tailPositions.Count.ToString();
        }

        private void Move(ref List<Vec2> snake, Vec2 move)
        {
            snake[0] += move;

            for(int i=1; i < snake.Count; i++)
            {
                Vec2 follower = snake[i];
                Follow(snake[i-1], ref follower);
                snake[i] = follower;
            }
            
        }
        public override string Part2()
        {
            List<Vec2> snake = new();
            HashSet<Vec2> tailPositions = new();
            for(int i=0; i < 10; i++)
            {
                snake.Add(Vec2.Zero);
            }

            foreach (var step in m_path)
            {
                for (int i = 0; i < step.Item2; i++)
                {
                    Move(ref snake, step.Item1);
                    tailPositions.Add(snake[snake.Count - 1]);
                }
            }

            return tailPositions.Count.ToString();
        }
    }
}
