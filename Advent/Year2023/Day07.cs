using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2023
{

    

    class Hand : IComparable<Hand>
    {
        public static bool Jokers = false;

        public IEnumerable<int> Cards;
        public string StringRep;

        public long Bid;

        public override string ToString()
        {
            return StringRep;
        }

        public Dictionary<int, int> CountHand()
        {
            Dictionary<int, int> counts = new();
            foreach (var c in Cards)
            {
                if (!counts.ContainsKey(c))
                {
                    counts[c] = 0;
                }
                counts[c]++;
            }
            return counts;
        }

        private int GradeInternal(Dictionary<int, int> counts)
        {
            switch (counts.Values.Max())
            {
                case 5:
                    return 6; // five of a kind
                case 4:
                    return 5; // four of a kind
                case 3:
                    // full house or 3 of a kind
                    if (counts.Values.Contains(2))
                        return 4; // full house
                    else
                        return 3;
                case 2:
                    // two pair or one pair
                    if (counts.Values.Count(x => x == 2) == 2)
                        return 2;
                    else
                        return 1;
                case 1:
                    // high card
                    return 0;
            }
            throw new InvalidOperationException("Can't get here");
        }

        public int GradeHand()
        {
            var counts = CountHand();

            return GradeInternal(counts);
        }

        public int GradeHandWithJokers()
        {
            var counts = CountHand();

            int jokerCount = counts.ContainsKey(11) ? counts[11] : 0;
            var nonJokers = counts.Where(x => x.Key != 11).OrderByDescending(x => x.Value);

            if (nonJokers.Any())
            {
                var bestNonJoker = nonJokers.First();
                // Add the jokers to the most populous other card
                counts[bestNonJoker.Key] += jokerCount;
                counts[11] = 0;
            }

            int grade = GradeInternal(counts);
            return grade;
        }

        public int CompareTo(Hand other)
        {
            var myGrade = Jokers ? GradeHandWithJokers() : GradeHand();
            var otherGrade = Jokers ? other.GradeHandWithJokers() : other.GradeHand();

            if(myGrade != otherGrade)
            {
                return myGrade - otherGrade;
            }
            
            for(int i=0; i < 5; i++)
            {
                int myCard = this.Cards.ElementAt(i);
                int otherCard = other.Cards.ElementAt(i);

                myCard = Jokers && myCard == 11 ? 1 : myCard;
                otherCard = Jokers && otherCard == 11 ? 1 : otherCard;

                int val = myCard - otherCard;
                if(val != 0)
                {
                    return val;
                }
            }

            return 0;
        }
    }

    internal class Day07 : DayLineLoaderBasic
    {
        List<Hand> m_hands = new();

        public override void ParseInputLine(string line, int lineNum)
        {
            var split = line.Split(' ');
            Hand h = new Hand();
            h.Bid = long.Parse(split[1]);
            h.StringRep = split[0];
            h.Cards = split[0].Select(c =>
            {
                switch (c)
                {
                    case 'A':
                        return 14;
                    case 'K':
                        return 13;
                    case 'Q':
                        return 12;
                    case 'J':
                        return 11;
                    case 'T':
                        return 10;
                    default:
                        return int.Parse($"{c}");
                }
            }).ToList();
            m_hands.Add(h);
        }

        public override string Part1()
        {
            long sum = 0;
            Hand.Jokers = false;
            m_hands.Sort();
            for(int i=0; i < m_hands.Count; i++)
            {
                int rank = i + 1;
                sum += m_hands[i].Bid * rank;
            }

            return sum.ToString();
        }

        public override string Part2()
        {
            long sum = 0;
            Hand.Jokers = true;
            m_hands.Sort();
            for (int i = 0; i < m_hands.Count; i++)
            {
                int rank = i + 1;
                sum += m_hands[i].Bid * rank;
                Console.WriteLine($"Rank {rank} : {m_hands[i].GradeHandWithJokers()} : {m_hands[i]}");
            }

            return sum.ToString();
        }
    }
}
