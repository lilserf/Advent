using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent2020.Year2022
{
    internal class Day17 : DayLineLoaderBasic
    {
        private string m_jets;

        private const int MAP_WIDTH = 7;

        private char[,] m_map = new char[10000,MAP_WIDTH];
        private int m_highest = 0;

        private enum Shape
        {
            Horiz,
            Plus,
            Angle,
            Vert,
            Square
        }

        private int[] HEIGHT = { 1, 3, 3, 4, 2 };
        private int[] WIDTH = { 4, 3, 3, 1, 2 };

        private Vec2 MoveLeftRight(Shape shape, Vec2 current, Vec2 direction)
        {
            Vec2 newPos = current + direction;
            if(newPos.X < 0 || (newPos.X + WIDTH[(int)shape] > MAP_WIDTH))
            {
                return current;
            }
            // TODO: check for collision with existing shapes
            return newPos;
        }

        private (Vec2, bool) MoveDown(Shape shape, Vec2 current)
        {
            // Up because we're putting 0 at the bottom of the stack here
            Vec2 next = current + Vec2.Up;
            Vec2 bottom = new Vec2(current.X, current.Y - HEIGHT[(int)shape]);

            if (bottom.Y <= m_highest && (CheckForLanding(shape, bottom)))
            { 
                return (current, true);
            }
            else
            {
                return (next, false);
            }
        }

        private bool CheckForLanding(Shape shape, Vec2 bottom)
        {
            if (bottom.Y < 0)
                return true;

            switch(shape)
            {
                case Shape.Plus:
                    {
                        // Bottom sticky-outy bit
                        Vec2 point = new Vec2(bottom.X + 1, bottom.Y);
                        if (m_map[point.Y, point.X] == '#')
                        {
                            return true;
                        }

                        point = new Vec2(bottom.X, bottom.Y + 1);
                        // Crossbar
                        if (m_map[point.Y, point.X] == '#' || m_map[point.Y, point.X+2] == '#')
                        {
                            return true;
                        }
                        break;
                    }
                default:
                    {
                        for (int i = 0; i < WIDTH[(int)shape]; i++)
                        {
                            Vec2 point = new Vec2(bottom.X + i, bottom.Y);
                            if (m_map[point.Y, point.X] == '#')
                            {
                                // Some square in this row is filled
                                return true;
                            }
                        }
                        break;
                    }
            }

            return false;
        }

        public override void ParseInputLine(string line)
        {
            m_jets = line;
        }

        private void ClearMap()
        {
            for(int col = 0; col < m_map.GetLength(0); col++)
            {
                for(int row = 0; row < m_map.GetLength(1); row++)
                {
                    m_map[col, row] = '.';
                }
            }
        }

        private void BlitShape(Shape shape, Vec2 pos)
        {
            switch (shape)
            {
                case Shape.Horiz:
                    {
                        for(int i=0; i < WIDTH[(int)shape]; i++)
                        {
                            m_map[pos.Y, pos.X + i] = '#';
                        }
                        break;
                    }
                case Shape.Vert:
                    {
                        for(int i=0; i < HEIGHT[(int)shape]; i++)
                        {
                            m_map[pos.Y - i, pos.X] = '#';
                        }
                        break;
                    }
                case Shape.Square:
                    {
                        m_map[pos.Y, pos.X] = '#';
                        m_map[pos.Y-1, pos.X] = '#';
                        m_map[pos.Y, pos.X+1] = '#';
                        m_map[pos.Y-1, pos.X+1] = '#';
                        break;
                    }
                case Shape.Angle:
                    {
                        m_map[pos.Y - 0, pos.X + 2] = '#';
                        m_map[pos.Y - 1, pos.X + 2] = '#';
                        m_map[pos.Y - 2, pos.X + 2] = '#';
                        m_map[pos.Y - 2, pos.X + 1] = '#';
                        m_map[pos.Y - 2, pos.X + 0] = '#';
                        break;
                    }
                case Shape.Plus:
                    {
                        m_map[pos.Y - 0, pos.X + 1] = '#';
                        m_map[pos.Y - 1, pos.X + 1] = '#';
                        m_map[pos.Y - 2, pos.X + 1] = '#';

                        m_map[pos.Y - 1, pos.X + 0] = '#';
                        m_map[pos.Y - 1, pos.X + 2] = '#';

                        break;
                    }

            }
        }

        private void PrintMap(Vec2 curr, Shape shape)
        {
            for(int row = Math.Max(curr.Y + HEIGHT[(int)shape], m_highest); row >= 0; row--)
            {
                for(int col = 0; col < MAP_WIDTH; col++)
                {
                    if (row <= curr.Y && row > curr.Y - HEIGHT[(int)shape] && col >= curr.X && col < curr.X + WIDTH[(int)shape])
                    {
                        Console.Write('@');
                    }
                    else
                    {
                        Console.Write(m_map[row, col]);
                    }
                }
                Console.WriteLine();
            }
        }

        public override string Part1()
        {
            ClearMap();
            Shape[] shapes = { Shape.Horiz, Shape.Plus, Shape.Angle, Shape.Vert, Shape.Square };
            int jet = 0;
            int shape = 0;

            for(int count = 0; count < 2022; count++)
            {
                bool landed = false;
                Shape curr = shapes[shape];
                Vec2 pos = new Vec2(2, m_highest + 2 + HEIGHT[(int)curr]);

                while(!landed)
                {
                    char jetDir = m_jets[jet];
                    pos = MoveLeftRight(curr, pos, jetDir == '<' ? Vec2.Left : Vec2.Right);
                    Console.WriteLine("Jet pushed " + (jetDir == '<' ? "Left" : "Right"));
                    PrintMap(pos, curr);
                    (pos, landed) = MoveDown(curr, pos);
                    Console.WriteLine($"Rock falls 1 unit, landed: {landed}");
                    PrintMap(pos, curr);

                    jet = (jet + 1) % m_jets.Length;

                    Console.ReadKey();
                    Console.WriteLine("--------------");
                }

                BlitShape(curr, pos);
                m_highest = pos.Y + 1;
                shape = (shape + 1) % shapes.Length;

            }

            return "blah";
        }

        public override string Part2()
        {
            throw new NotImplementedException();
        }
    }
}
