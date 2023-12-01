using Advent;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Advent.Year2022
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
            if(DoForShape(shape, newPos, CheckForCollision))
            {
                return current;
            }

            return newPos;
        }

        private bool CheckForCollision(Vec2 arg)
        {
            if (m_map[arg.Y, arg.X] == '#')
                return true;
            return false;
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

        private bool Blit(Vec2 pos)
        {
            m_map[pos.Y, pos.X] = '#';
            return true;
        }

        private bool DoForShape(Shape shape, Vec2 pos, Func<Vec2, bool> op)
        {
            bool result = false;
            switch (shape)
            {
                case Shape.Horiz:
                    {
                        for(int i=0; i < WIDTH[(int)shape]; i++)
                        {
                            bool b = op(new Vec2(pos.X + i, pos.Y));
                            result = result || b;
                        }
                        break;
                    }
                case Shape.Vert:
                    {
                        for(int i=0; i < HEIGHT[(int)shape]; i++)
                        {
                            bool b = op(new Vec2(pos.X, pos.Y-i));
                            result = result || b;
                        }
                        break;
                    }
                case Shape.Square:
                    {
                        bool b = op(new Vec2(pos.X, pos.Y));
                        result = result || b;
                        b = op(new Vec2(pos.X, pos.Y - 1));
                        result = result || b;
                        b = op(new Vec2(pos.X + 1, pos.Y));
                        result = result || b;
                        b = op(new Vec2(pos.X + 1, pos.Y - 1));
                        result = result || b;
                        break;
                    }
                case Shape.Angle:
                    {
                        bool b = op(new Vec2(pos.X + 2, pos.Y - 0));
                        result = result || b;
                        b = op(new Vec2(pos.X + 2, pos.Y - 1));
                        result = result || b;
                        b = op(new Vec2(pos.X + 2, pos.Y - 2));
                        result = result || b;
                        b = op(new Vec2(pos.X + 1, pos.Y - 2));
                        result = result || b;
                        b = op(new Vec2(pos.X + 0, pos.Y - 2));
                        result = result || b;
                        break;
                    }
                case Shape.Plus:
                    {
                        bool b = op(new Vec2(pos.X + 1, pos.Y - 0));
                        result = result || b;
                        b = op(new Vec2(pos.X + 1, pos.Y - 1));
                        result = result || b;
                        b = op(new Vec2(pos.X + 1, pos.Y - 2));
                        result = result || b;

                        b = op(new Vec2(pos.X + 0, pos.Y - 1));
                        result = result || b;
                        b = op(new Vec2(pos.X + 2, pos.Y - 1));
                        result = result || b;

                        break;
                    }

            }
            return result;
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

        public void DoWork(int num)
        {
            ClearMap();
            Shape[] shapes = { Shape.Horiz, Shape.Plus, Shape.Angle, Shape.Vert, Shape.Square };
            int jet = 0;
            int shape = 0;
            m_highest = 0;

            for (int count = 0; count < num; count++)
            {
                bool landed = false;
                Shape curr = shapes[shape];
                Vec2 pos = new Vec2(2, m_highest + 2 + HEIGHT[(int)curr]);

                while (!landed)
                {
                    char jetDir = m_jets[jet];
                    pos = MoveLeftRight(curr, pos, jetDir == '<' ? Vec2.Left : Vec2.Right);
                    //Console.WriteLine("Jet pushed " + (jetDir == '<' ? "Left" : "Right"));
                    //PrintMap(pos, curr);
                    (pos, landed) = MoveDown(curr, pos);
                    //Console.WriteLine($"Rock falls 1 unit, landed: {landed}");
                    //PrintMap(pos, curr);

                    jet = (jet + 1) % m_jets.Length;

                    //Console.ReadKey();
                    //Console.WriteLine("--------------");
                }

                DoForShape(curr, pos, Blit);
                m_highest = Math.Max(m_highest, pos.Y + 1);
                shape = (shape + 1) % shapes.Length;

            }

        }

        public override string Part1()
        {
            DoWork(2022);
            return $"Highest is {m_highest}";
        }

        public override string Part2()
        {
            // Need to do this:
            //In this case, you can memorize the current place in the jet pattern, together with the current rock shape. If you know how many rocks had stopped and the height of the tower the last time you saw that pair, then you know the size of the cycle. If you also keep track of the maximum vertical distance that a rock falls before stopping, then you know that all the rows below the distance from the top are set in stone. :-) That lets you verify the pattern against the tower to see that the cycle really is repeating.
            DoWork(0);
            return "stub";
        }
    }
}
