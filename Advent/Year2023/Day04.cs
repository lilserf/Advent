using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2023
{
    internal class Day04 : DayLineLoaderBasic
    {
        struct Card
        {
            public int Id;
            public List<int> Winners;
            public List<int> Numbers;

            public Card(int id)
            {
                Id = id;
            }

            public static Card FromString(string str)
            {
                var halves = str.Split('|');
                var first = halves[0].Split(':');
                int id = int.Parse(first[0].Substring(5));

                Card card = new Card(id);
                card.Winners = first[1].Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                card.Numbers = halves[1].Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                return card;
            }

            public int Score()
            {
                return Winners.Intersect(Numbers).Count();
            }
        }

        private Dictionary<int, Card> m_cards = new();

        public override void ParseInputLine(string line, int lineNum)
        {
            var card = Card.FromString(line);
            m_cards[card.Id] = card;
        }

        public override string Part1()
        {
            return m_cards.Values.Select(c => c.Score()).Select(n => (int)Math.Pow(2, n-1)).Sum().ToString();
        }

        public override string Part2()
        {
            int sum = 0;
            int maxId = m_cards.Keys.Max();

            Dictionary<int, int> cards = new Dictionary<int, int>();
            for(int i=1; i <= maxId; i++)
            {
                cards[i] = 1;
            }

            for(int i=1; i <= maxId; i++)
            {
                int numCards = cards[i];
                int score = m_cards[i].Score();
                for(int j=1; j <= score; j++)
                {
                    cards[i + j] += numCards;
                }
            }

            return cards.Values.Sum().ToString();
        }
    }
}
