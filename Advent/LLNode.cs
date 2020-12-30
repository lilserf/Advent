using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent2020
{
	class LinkedList
	{
		private static void AdjustHead(LLNode start, long num, long headIndex)
		{
			LLNode curr = start;
			for(int i=0; i < num; i++)
			{
				if(i == headIndex)
				{
					curr.IsHead = true;
				}
				else
				{
					curr.IsHead = false;
				}
				curr = curr.Next;
			}
		}

		// Reverse a chunk of list including this starting node
		public static LLNode Reverse(LLNode start, long num)
		{
			if (num == 0 || num == 1) return start;

			long len = Length(start);
			long headIndex = FindHeadIndex(start, num);

			if (num > len)
			{
				throw new InvalidOperationException("Can't reverse a portion longer than the list");
			}
			else if(len == num)
			{
				LLNode curr = start;
				bool first = true;
				while(first || (curr != null && curr != start))
				{
					first = false;
					LLNode next = curr.Next;
					curr.Next = curr.Prev;
					curr.Prev = next;
					
					curr = next;
				}
				AdjustHead(curr.Next, num, headIndex);
				return curr.Next;
			}
			else
			{

				LLNode left = start.Prev;

				LLNode curr = start;
				for (long i = 0; i < num; i++)
				{
					LLNode next = curr.Next;
					curr.Next = curr.Prev;
					curr.Prev = next;

					curr = next;
				}

				LLNode right = curr;
				LLNode last = curr.Prev;

				left.Next = last;
				last.Prev = left;

				start.Next = right;
				right.Prev = start;

				AdjustHead(last, num, headIndex);
				return last;
			}
		}

		public static long Length(LLNode start)
		{
			long len = 1;
			LLNode curr = start.Next;
			while (curr != null && curr != start)
			{
				len++;
				curr = curr.Next;
			}

			return len;
		}

		public static LLNode Next(LLNode start, long num)
		{
			LLNode curr = start;
			for(long i=0; i < num;i++)
			{
				curr = curr.Next;
			}
			return curr;
		}

		public static LLNode GetHead(LLNode node)
		{
			LLNode curr = node;
			while(!curr.IsHead)
			{
				curr = curr.Next;
			}
			return curr;
		}

		private static long FindHeadIndex(LLNode start, long num)
		{
			LLNode curr = start;
			for(long i=0; i < num; i++)
			{
				if (curr.IsHead)
					return i;
				curr = curr.Next;
			}

			return -1;
		}

		public static LLNode InsertAfter(LLNode after, long id)
		{
			LLNode next = after.Next;

			after.Next = new LLNode(id);
			next.Prev = after.Next;

			after.Next.Next = next;
			after.Next.Prev = after;

			return after.Next;
		}
	}

	class LLNode
	{
		public bool IsHead { get; set; }
		public long Id { get; set; }
		public LLNode Next { get; set; }
		public LLNode Prev { get; set; }

		public LLNode(long id)
		{
			Id = id;
			Next = null;
			Prev = null;
		}

		public void SetNext(LLNode node)
		{
			Next = node;
			node.Prev = this;
		}
		public LLNode AddNext(long id)
		{
			Next = new LLNode(id);
			Next.Prev = this;
			return Next;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(Id);
			LLNode curr = Next;
			while(curr != this)
			{
				sb.Append($", {curr.Id}");
				curr = curr.Next;
			}
			return sb.ToString();
		}
	}
}
