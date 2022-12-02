using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent2020.Year2022
{


    internal class Day2 : DayLineLoaderBasic
    {
        enum RPS
        {
            Rock = 0,
            Paper = 1,
            Scissors = 2
        };

        int[] Points = new int[]
        {
            1,
            2,
            3
        };

        enum Result
        {
            Lose,
            Draw,
            Win
        }

        int[] ResultPoints = new int[]
        {
            0,
            3,
            6
        };

        List<(RPS, RPS)> m_plays = new();
        List<(RPS, Result)> m_results = new();

        public override void ParseInputLine(string line)
        {
            var split = line.Split(' ');
            var opp = (RPS)(split[0][0] - 'A');
            var me = (RPS)(split[1][0] - 'X');
            m_plays.Add((opp, me));

            var result = (Result)(split[1][0] - 'X');
            m_results.Add((opp, result));
        }

        Result Match(RPS opp, RPS me)
        {
            if (opp == me) return Result.Draw;

            if (opp == RPS.Rock && me == RPS.Scissors ||
                opp == RPS.Scissors && me == RPS.Paper ||
                opp == RPS.Paper && me == RPS.Rock)
                return Result.Lose;

            return Result.Win;
        }

        RPS AchieveResult(RPS opp, Result result)
        {
            if (result == Result.Draw) return opp;

            if(result == Result.Win)
            {
                switch(opp)
                {
                    case RPS.Rock: return RPS.Paper;
                    case RPS.Scissors: return RPS.Rock;
                    case RPS.Paper: return RPS.Scissors;
                }
            }
            else
            {
                switch (opp)
                {
                    case RPS.Rock: return RPS.Scissors;
                    case RPS.Scissors: return RPS.Paper;
                    case RPS.Paper: return RPS.Rock;
                }
            }

            throw new InvalidOperationException();
        }

        public override string Part1()
        {
            int score = 0;
            foreach(var play in m_plays)
            {
                var result = Match(play.Item1, play.Item2);
                score += ResultPoints[(int)result] + Points[(int)play.Item2];
            }

            return score.ToString();
        }

        public override string Part2()
        {
            int score = 0;
            foreach(var result in m_results)
            {
                RPS me = AchieveResult(result.Item1, result.Item2);
                score += ResultPoints[(int)result.Item2] + Points[(int)me];
            }

            return score.ToString();
        }
    }
}
