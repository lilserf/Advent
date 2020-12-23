using Advent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent2020.Year2020
{
	class Day22Strategy : DayLineLoaderBasic
	{

		Queue<int> m_deck1;
		Queue<int> m_deck2;

		int m_currPlayerNum = 0;

		Queue<int> m_currDeck = new Queue<int>();

		public override void ParseInputLine(string line)
		{
			if (string.IsNullOrWhiteSpace(line))
				return;

			var match = Regex.Match(line, @"Player (\d+):");
			if (match.Success)
			{
				int num = int.Parse(match.Groups[1].Value);
				if(m_currDeck.Count > 0)
				{
					m_deck1 = m_currDeck;
					m_currDeck = new Queue<int>();
				}
				m_currPlayerNum = num;
			}
			else
			{
				m_currDeck.Enqueue(int.Parse(line));
			}
		
		}

		Queue<int> m_origDeck1;
		Queue<int> m_origDeck2;

		public override void ParseInputLinesEnd(StreamReader sr)
		{
			m_deck2 = m_currDeck;

			m_origDeck1 = new Queue<int>(m_deck1);
			m_origDeck2 = new Queue<int>(m_deck2);
		}

		public bool PlayRound()
		{
			var card1 = m_deck1.Dequeue();
			var card2 = m_deck2.Dequeue();

			if(card1 > card2)
			{
				m_deck1.Enqueue(card1);
				m_deck1.Enqueue(card2);
			}
			else
			{
				m_deck2.Enqueue(card2);
				m_deck2.Enqueue(card1);
			}

			return m_deck1.Any() && m_deck2.Any();
		}

		public override string Part1()
		{
			while(PlayRound())
			{
				// do nothing
			}

			var winner = m_deck1.Any() ? m_deck1 : m_deck2;
			long sum = Score(winner);

			return sum.ToString() ;
		}


		long Score(Queue<int> winner)
		{
			int mult = winner.Count();
			long sum = 0;
			while (winner.Any())
			{
				var c = winner.Dequeue();
				sum += c * mult;
				mult--;
			}
			return sum;
		}

		private string StringRep(Queue<int> deck1, Queue<int> deck2)
		{
			var d1 = deck1.Aggregate("", (s, x) => s + " " + x);
			var d2 = deck2.Aggregate("", (s, x) => s + " " + x);

			return $"{d1}|{d2}";
		}

		private int PlayRecursiveGame(Queue<int> p1, Queue<int> p2, int game = 1)
		{
			HashSet<string> pastRounds = new HashSet<string>();

			int round = 0;
			// play rounds
			while (p1.Any() && p2.Any())
			{
				round++;
				string sr = StringRep(p1, p2);
				if (pastRounds.Contains(sr))
				{
					//Console.WriteLine("Saw same game state! Player 1 wins!");
					return 1;
				}
				pastRounds.Add(sr);

				int winner = 0;
				var c1 = p1.Dequeue();
				var c2 = p2.Dequeue();

				//Console.Write($"Round {round} (Game {game}): Player 1 plays {c1}, Player 2 plays {c2}... ");

				if (c1 <= p1.Count() && c2 <= p2.Count())
				{
					var new1 = new Queue<int>(p1.Take(c1));
					var new2 = new Queue<int>(p2.Take(c2));

					//Console.WriteLine($"Recursing to a new game!");
					winner = PlayRecursiveGame(new1, new2, game+1);
					//Console.WriteLine($"Player {winner} wins round {round} of game {game}!\t({p1.Count()}/{p2.Count()})");
				}
				else
				{
					winner = (c1 > c2) ? 1 : 2;
					//Console.WriteLine($"Player {winner} wins round {round} of game {game}!\t({p1.Count()}/{p2.Count()})");
				}

				if (winner == 1)
				{
					p1.Enqueue(c1);
					p1.Enqueue(c2);
				}
				else
				{
					p2.Enqueue(c2);
					p2.Enqueue(c1);
				}
			}

			int gameWinner = (p1.Any()) ? 1 : 2;
			//Console.WriteLine($"Game {game} won by player {gameWinner}! Returning to game {game-1}");
			//Console.ReadKey();
			return gameWinner;
		}

		public override string Part2()
		{
			m_deck1 = new Queue<int>(m_origDeck1);
			m_deck2 = new Queue<int>(m_origDeck2);

			//m_deck1 = new Queue<int>(new int[] { 9, 2, 6, 3, 1 });
			//m_deck2 = new Queue<int>(new int[] { 5, 8, 4, 7, 10 });
			var winner = PlayRecursiveGame(m_deck1, m_deck2);


			return Score(winner == 1 ? m_deck1 : m_deck2).ToString();
		}
	}
}
