using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2023
{
    internal class Day02 : DayLineLoaderBasic
    {
        struct Draw
        {
            public int Red;
            public int Green;
            public int Blue;

            public bool Possible(Draw limit)
            {
                return Red <= limit.Red && Green <= limit.Green && Blue <= limit.Blue;
            }
        }

        struct Game
        {
            public int Id;
            public List<Draw> Draws;

            public bool Possible(Draw limit)
            {
                return Draws.All(x => x.Possible(limit));
            }

            public Draw MinRequired()
            {
                int red = Draws.Max(x => x.Red);
                int green = Draws.Max(x => x.Green);
                int blue = Draws.Max(x => x.Blue);

                return new Draw() { Red = red, Green = green, Blue = blue };
            }
        }

        List<Game> m_games = new();

        public override void ParseInputLine(string line)
        {
            var a = line.Split(":");
            Game g = new Game();
            g.Id = int.Parse(a[0].Substring(5));
            g.Draws = new();

            var draws = a[1].Trim().Split(";");
            foreach (var draw in draws)
            {
                var words = draw.Trim().Split(',');
                Draw d = new Draw();
                foreach(var word in words)
                {
                    var stuff = word.Trim().Trim(',').Split(" ");
                    int val = int.Parse(stuff[0]);
                    if (stuff[1] == "red")
                        d.Red = val;
                    else if (stuff[1] == "green")
                        d.Green = val;
                    else if (stuff[1] == "blue")
                        d.Blue = val;
                    else
                        throw new InvalidOperationException();
                }

                g.Draws.Add(d);
            }

            m_games.Add(g);
        }

        public override string Part1()
        {
            Draw limit = new Draw() { Red = 12, Green = 13, Blue= 14 };

            var valid = m_games.Where(x => x.Possible(limit));

            return valid.Select(x => x.Id).Sum().ToString();
        }

        public override string Part2()
        {
            var powers = m_games.Select(x => x.MinRequired()).Select(y => y.Red * y.Green * y.Blue).Sum();

            return powers.ToString();
        }
    }
}
