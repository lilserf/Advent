using Advent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2020
{
	class Cup
	{
		public int Id { get; set; }
		public Cup Next { get; set; }
		public Cup Prev { get; set; }

		public Cup(int id)
		{
			Id = id;
			Next = null;
			Prev = null;
		}
	}

	class Day23Strategy : DayLineLoaderBasic
	{
		Cup m_head = null;
		Cup m_tail = null;

		Dictionary<int, Cup> m_cups;

		int MaxCup = 9;

		public void AddCup(int id)
		{
			m_tail.Next = new Cup(id);
			m_tail.Next.Prev = m_tail;
			m_tail = m_tail.Next;
			m_cups[id] = m_tail;
		}

		public override void ParseInputLine(string line)
		{
			m_head = null;
			m_tail = null;
			m_cups = new Dictionary<int, Cup>();

			foreach(char c in line)
			{
				int id = int.Parse($"{c}");
				if(m_head == null)
				{
					m_head = new Cup(id);
					m_tail = m_head;
					m_cups[id] = m_tail;
				}
				else
				{
					AddCup(id);
				}
				
			}
		}

		public override void ParseInputLinesEnd(StreamReader sr)
		{
			m_tail.Next = m_head;
			m_head.Prev = m_tail;
			MaxCup = m_cups.Values.Max(x => x.Id);
		}

		Cup m_curr = null;

		public void RunMove()
		{
			//Console.WriteLine($"Starting move at {m_curr.Id}");
			Cup ptr = m_curr.Next;

			List<Cup> pickedUp = new List<Cup>();

			// pick up 3 cups
			for(int i=0; i < 3; i++)
			{
				//Console.WriteLine($"  Picked up {ptr.Id}");
				pickedUp.Add(ptr);
				ptr = ptr.Next;
			}

			// close the circle
			m_curr.Next = ptr;
			ptr.Prev = m_curr;

			// Choose a dest cup
			var destId = m_curr.Id - 1;
			if (destId < 1)
				destId = MaxCup;
			while (pickedUp.Any(x => x.Id == destId))
			{
				destId--;
				if (destId < 1)
					destId = MaxCup;
			}
			//Console.WriteLine($"  Destination is {destId}");

			// Drop the picked up cups back in
			Cup dest = m_cups[destId];
			Cup destNext = dest.Next;

			dest.Next = pickedUp.First();
			pickedUp.First().Prev = dest;

			pickedUp.Last().Next = destNext;
			destNext.Prev = pickedUp.Last();

			m_curr = m_curr.Next;
			//Console.WriteLine($"  Curr is now {m_curr.Id}");
		}

		public string GetOrder()
		{
			string ret = "";
			var start = m_cups[1];
			var curr = start.Next;
			while(curr != start)
			{
				ret = ret + curr.Id;
				curr = curr.Next;
			}

			return ret;
		}

		public override string Part1()
		{
			m_curr = m_head;

			for(int i=0; i < 100; i++)
			{
				RunMove();
			}

			return GetOrder();
		}

		public override string Part2()
		{
			ParseInputLine("137826495");
			//ParseInputLine("389125467");

			for(int i=10; i <= 1000000; i++)
			{
				AddCup(i);
			}

			ParseInputLinesEnd(null);
			m_curr = m_head;

			for(int i=0; i < 10000000; i++)
			{
				RunMove();
				//Console.ReadKey();
			}

			Cup one = m_cups[1];

			Console.WriteLine($"Next two cups are {one.Next.Id} and {one.Next.Next.Id}!");
			BigInteger prod = BigInteger.Multiply(one.Next.Id, one.Next.Next.Id);
			return prod.ToString();
		}
	}
}
