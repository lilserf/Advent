using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2023
{
    internal class Day06 : DayLineLoaderBasic
    {
        List<int> m_times = new();
        List<int> m_distances = new();

        public override void ParseInputLine(string line, int lineNum)
        {
            if(lineNum == 0)
            {
                m_times = line.Substring(9).Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToList();
            }
            else
            {
                m_distances = line.Substring(9).Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToList();
            }
        }

        long Distance(long raceLength, long chargeTime)
        {
            // Race is N seconds long
            // Button is held for B seconds
            // Distance is B * (N-B)
            return chargeTime * (raceLength - chargeTime);
        }

        public override string Part1()
        {
            Dictionary<int, int> ways = new();

            foreach(var (time, dist) in m_times.Zip(m_distances))
            {
                for(int i=1; i < time; i++)
                {
                    long myDist = Distance(time, i);
                    if(myDist > dist)
                    {
                        if(!ways.ContainsKey(time))
                        {
                            ways[time] = 0;
                        }
                        ways[time]++;
                    }
                }
            }

            return ways.Values.Aggregate(1, (acc, val) => acc * val).ToString();
        }

        public override string Part2()
        {
            int ways = 0;

            long time = long.Parse(m_times.Aggregate("", (acc, val) => acc += val.ToString()));
            long dist = long.Parse(m_distances.Aggregate("", (acc, val) => acc += val.ToString()));

            for(int i=1; i < time; i++)
            {
                long myDist = Distance(time, i);
                if(myDist > dist)
                {
                    ways++;
                }
            }

            return ways.ToString();
        }
    }
}
