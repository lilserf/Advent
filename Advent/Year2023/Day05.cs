using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent.Year2023
{
    internal class Day05 : DayLineLoaderBasic
    {
        List<long> m_seeds = new();
        List<(long, long)> m_seedRanges = new();
        Regex mapRegex = new Regex(@"(\w+)\-to\-(\w+) map:");

        Dictionary<(string, string), List<Range>> m_maps = new();

        struct Range
        {
            public long Source;
            public long Target;
            public long Length;

            public bool IsMapped(long value)
            {
                return (value >= Source && value < Source + Length);
            }
            public long Map(long value)
            {
                if(IsMapped(value))
                {
                    return Target + (value - Source);
                }
                return value;
            }
            public bool IsMappedReverse(long value)
            {
                return (value >= Target && value < Target + Length);
            }
            public long MapReverse(long value)
            {
                if(IsMappedReverse(value))
                {
                    return Source + (value - Target);
                }
                return value;
            }
        }

        (string, string) m_currentMap;
        List<Range> m_currentRange;
        bool m_started = false;

        public override void ParseInputLine(string line, int lineNum)
        {
            if(lineNum == 0)
            {
                m_seeds = line.Substring(6).Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(long.Parse).ToList();
                m_seedRanges = m_seeds.Chunk(2).Select(x => (x[0], x[1])).ToList();
                return;
            }

            if(mapRegex.IsMatch(line))
            {
                if(m_started)
                {
                    m_maps[m_currentMap] = m_currentRange;
                }
                m_started = true;

                var matches = mapRegex.Matches(line)[0].Groups;
                m_currentMap.Item1 = matches[1].Value;
                m_currentMap.Item2 = matches[2].Value;
                m_currentRange = new List<Range>();
            }
            else
            {
                if(!string.IsNullOrEmpty(line))
                {
                    var nums = line.Split(" ").Select(long.Parse).ToList();
                    Range r = new Range();
                    r.Target = nums[0];
                    r.Source = nums[1];
                    r.Length = nums[2];
                    m_currentRange.Add(r);
                }
            }

        }

        public override void ParseInputLinesEnd(StreamReader sr)
        {
            // Finish this
            m_maps[m_currentMap] = m_currentRange;
        }

        long MapValue(List<Range> rangeList, long value)
        {
            foreach(var r in rangeList)
            {
                if(r.IsMapped(value))
                {
                    return r.Map(value);
                }
            }

            return value;
        }

        long MapValueReverse(List<Range> rangeList, long value)
        {
            foreach (var r in rangeList)
            {
                if (r.IsMappedReverse(value))
                {
                    return r.MapReverse(value);
                }
            }

            return value;
        }


        public long MapLocation(long seed)
        {
            long curr = seed;
            var mapKey = m_maps.Keys.Where(x => x.Item1 == "seed").First();

            while (mapKey.Item2 != "location")
            {
                var map = m_maps[mapKey];
                curr = MapValue(map, curr);

                mapKey = m_maps.Keys.Where(x => x.Item1 == mapKey.Item2).First();
            }

            var endMap = m_maps[mapKey];
            curr = MapValue(endMap, curr);
            return curr;
        }

        public long MapLocationReverse(long location)
        {
            long curr = location;
            var mapKey = m_maps.Keys.Where(x => x.Item2 == "location").First();

            while (mapKey.Item1 != "seed")
            {
                var map = m_maps[mapKey];
                curr = MapValueReverse(map, curr);

                mapKey = m_maps.Keys.Where(x => x.Item2 == mapKey.Item1).First();
            }

            var endMap = m_maps[mapKey];
            curr = MapValueReverse(endMap, curr);
            return curr;
        }

        public override string Part1()
        {
            List<long> locations = new();

            foreach (var seed in m_seeds)
            {
                locations.Add(MapLocation(seed));
            }

            return locations.Min().ToString();
        }

        //IEnumerable<Range> Combine(Range start, Range end)
        //{
        //    // x1 <= C <= x2
        //    // y1 <= C <= y2
        //    // x1 <= y2 && y1 <= x2

        //    long x1 = start.Target;
        //    long x2 = start.Target + start.Length;

        //    long y1 = end.Source;
        //    long y2 = end.Source + end.Length;

        //    bool overlaps = (x1 <= y2 && y1 <= x2);

        //    // |-----------------|
        //    //         |---------------|
        //    //
        //    // |-------|---------|-----|

        //    // |-----------------|
        //    //                        |-----------|
        //    //
        //    // |-----------------|    |-----------|



        //}

        public override string Part2()
        {
            long loc = 0;
            while(true)
            {
                long seed = MapLocationReverse(loc);

                foreach (var range in m_seedRanges)
                {
                    if(seed >= range.Item1 && seed < range.Item1 + range.Item2)
                        return loc.ToString();
                }
                loc++;
            }
        }
    }
}
