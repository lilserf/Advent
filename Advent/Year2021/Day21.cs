using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2021
{
    class State
    {
        public int[] Positions { get; set; } = { 4, 6 };
        public int[] Scores { get; set; } = { 0, 0 };
        public List<int> Rolls { get; set; } = new List<int>();
        public int NextTurn { get; set; } = 0;

        internal State()
        {
        }

        internal State(State other)
        {
            Positions[0] = other.Positions[0];
            Positions[1] = other.Positions[1];
            Scores[0] = other.Scores[0];
            Scores[1] = other.Scores[1];
            Rolls = other.Rolls.ToList();
            NextTurn = other.NextTurn;
        }
    }

    struct State2
    {
        public int[] Positions { get; set; } = { 4, 6 };
        public int[] Scores { get; set; } = { 0, 0 };

        public State2()
        {
        }

        public State2(State2 other)
        {
            Positions[0] = other.Positions[0];
            Positions[1] = other.Positions[1];
            Scores[0] = other.Scores[0];
            Scores[1] = other.Scores[1];
        }
    }

    internal class Day21 : DayLineLoaderBasic
    {
        int m_die = 1;
        int RollDeterministicDie()
        {
            var curr = m_die;
            m_die++;
            if (m_die > 100)
                m_die = 1;
            return curr;
        }

        IEnumerable<State> RollDiracDie(State state)
        {
            var s1 = new State(state);
            var s2 = new State(state);
            var s3 = new State(state);

            s1.Rolls.Add(1);
            s2.Rolls.Add(2);
            s3.Rolls.Add(3);

            yield return s1;
            yield return s2;
            yield return s3;
        }

        State TakeTurnIfReady(State state)
        {
            if (state.Rolls.Count == 0 || state.Rolls.Count % 3 != 0)
                return state;

            var roll = state.Rolls.Skip(Math.Max(0, state.Rolls.Count() - 3)).Sum();

            var curr = state.Positions[state.NextTurn];

            curr += roll;
            curr = (curr-1) % 10 + 1;

            state.Scores[state.NextTurn] += curr;
            state.Positions[state.NextTurn] = curr;
            state.NextTurn = ((state.NextTurn == 0) ? 1 : 0);
            return state;
        }

        public override string Part1()
        {
            State state = new State();

            while(state.Scores.All(x => x < 1000))
            {
                state.Rolls.Add(RollDeterministicDie());
                state.Rolls.Add(RollDeterministicDie());
                state.Rolls.Add(RollDeterministicDie());
                state = TakeTurnIfReady(state);
            }

            var losing = state.Scores.Min();

            return $"Player 1: {state.Scores[0]}, Player 2: {state.Scores[1]} -- {state.Rolls.Count * losing}";
        }

        Dictionary<int, int> FREQUENCIES = new Dictionary<int, int>
        {
            [3] = 1,
            [4] = 3,
            [5] = 6,
            [6] = 7,
            [7] = 6,
            [8] = 3,
            [9] = 1,
        };

        State2 TakeTurn2(State2 state, int roll, bool player1)
        {
            int index = player1 ? 0 : 1;
            var curr = state.Positions[index];

            curr += roll;
            curr = (curr - 1) % 10 + 1;

            var newState = new State2(state);
            newState.Scores[index] += curr;
            newState.Positions[index] = curr;

            return newState;
        }

        (Dictionary<State2, long>, long) RunGames(Dictionary<State2, long> games, bool player1)
        {
            Dictionary<State2, long> nextGames = new();
            long victories = 0;

            foreach ((var game, var count) in games)
            {
                foreach((var roll, var frequency) in FREQUENCIES)
                {
                    var newState = TakeTurn2(game, roll, player1);
                    var newCount = count * frequency;
                    
                    if(newState.Scores.Any(x => x >= 21))
                    {
                        victories += newCount;
                    }
                    else
                    {
                        if(!nextGames.ContainsKey(newState))
                        {
                            nextGames[newState] = 0;
                        }
                        nextGames[newState] += newCount;
                    }
                }
            }

            return (nextGames, victories);
        }

        public override string Part2()
        {
            // Do this by frequency - don't push a billion copies of game states into a queue, just count how many of each state exist
            Dictionary<State2, long> gameFreqs = new();
            gameFreqs[new State2()] = 1;
            long[] wins = { 0, 0 };
            bool player1Turn = true;

            while(gameFreqs.Count > 0)
            {
                (gameFreqs, long victories) = RunGames(gameFreqs, player1Turn);
                if(player1Turn)
                {
                    wins[0] += victories;
                }
                else
                {
                    wins[1] += victories;
                }
                player1Turn = !player1Turn;
            }

            return $"Player 1 victories: {wins[0]}, Player 2 victories: {wins[1]}";
        }

        public override void ParseInputLine(string line)
        {
        }
    }
}
