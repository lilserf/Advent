using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2022
{
    struct Span
    {
        public int Start { get; set; }
        public int End { get; set; }

        public Span(int start, int end)
        {
            Start = start;
            End = end;
        }
        public bool IsInsideOther(Span other)
        {
            return (Start >= other.Start && End <= other.End);
        }

        public bool Overlaps(Span other)
        {
            return !(other.End < Start || other.Start > End);
        }
    }

    internal class Day4 : DayLineLoaderBasic
    {
        List<(Span, Span)> m_spans = new();
        public override void ParseInputLine(string line)
        {
            var split = line.Split(',');
            var first = split[0].Split('-');
            var second = split[1].Split('-');

            var span1 = new Span(int.Parse(first[0]), int.Parse(first[1]));
            var span2 = new Span(int.Parse(second[0]), int.Parse(second[1]));
            m_spans.Add((span1, span2));
        }

        public override string Part1()
        {
            return m_spans.Where(x => x.Item1.IsInsideOther(x.Item2) || x.Item2.IsInsideOther(x.Item1)).Count().ToString();
        }

        public override string Part2()
        {
            return m_spans.Where(x => x.Item1.Overlaps(x.Item2)).Count().ToString();
        }
    }
}
