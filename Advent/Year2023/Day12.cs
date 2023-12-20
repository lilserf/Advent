using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2023
{
    internal class Day12 : DayLineLoaderBasic
    {
        struct Row
        {
            public string Springs;
            public List<int> Groups;

            public List<int> BrokenGroups;
            public List<int> UnknownGroups;
            public List<string> JustStuff;
        }

        List<Row> m_rows = new();

        public override void ParseInputLine(string line, int lineNum)
        {
            var split = line.Split(' ');

            Row r = new Row();
            r.Springs = split[0];
            r.Groups = split[1].Split(',').Select(int.Parse).ToList();
            var b = SplitLike(r.Springs);
            r.BrokenGroups = b.Where(x => x.Contains('#')).Select(x => x.Length).ToList();
            r.UnknownGroups = b.Where(x => x.Contains('?')).Select(x => x.Length).ToList();
            r.JustStuff = b.Where(x => !x.Contains('.')).ToList();

            m_rows.Add(r);
        }

        public IEnumerable<string> SplitLike(string Springs)
        {
            char prev = Springs[0];
            string part = "" + prev;
            for (int i = 1; i < Springs.Length; i++)
            {
                if (Springs[i] == prev)
                {
                    part += prev;
                }
                else
                {
                    yield return part;
                    part = "" + Springs[i];
                    prev = Springs[i];
                }
            }

            yield return part;
        }



        public override string Part1()
        {
            foreach(Row r in m_rows)
            {
                Console.WriteLine(r.Springs);
                //Console.WriteLine($"Broken: {r.BrokenGroups.Aggregate("", (acc, x) => acc += x + ",")}");
                //Console.WriteLine($"Unknown: {r.UnknownGroups.Aggregate("", (acc, x) => acc += x + ",")}");
                Console.WriteLine($"JustStuff: {r.JustStuff.Aggregate("", (acc, x) => acc += x + ",")}");
            }

            return "";
        }

        public override string Part2()
        {
            return "";
        }
    }
}
