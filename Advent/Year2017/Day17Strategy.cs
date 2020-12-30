using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent2020.Year2017
{
	class Day17Strategy : DayLineLoaderBasic
	{
		public override void ParseInputLine(string line)
		{
			
		}

		int SPINLOCK_COUNT = 304;

		public override string Part1()
		{
			LLNode root = new LLNode(0);
			root.Next = root;
			root.Prev = root;

			LLNode curr = root;
			for(int i = 1; i <= 2017; i++)
			{
				curr = LinkedList.Next(curr, SPINLOCK_COUNT);
				curr = LinkedList.InsertAfter(curr, i);
			}

			return curr.Next.Id.ToString();
		}

		public override string Part2()
		{
			// THIS IS SLOW (like 10 minutes) but works
			LLNode root = new LLNode(0);
			root.Next = root;
			root.Prev = root;

			LLNode curr = root;
			for (int i = 1; i <= 50000000; i++)
			{
				curr = LinkedList.Next(curr, SPINLOCK_COUNT);
				curr = LinkedList.InsertAfter(curr, i);
			}

			return root.Next.Id.ToString();
		}
	}
}
