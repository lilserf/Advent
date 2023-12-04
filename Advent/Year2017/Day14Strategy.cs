using Advent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2017
{

	class Day14Strategy : DayLineLoaderBasic
	{
		static void DebugPrint(LLNode start)
		{
			LLNode head = LinkedList.GetHead(start);

			if (head == start)
				Console.Write($"[{head.Id}]");
			else
				Console.Write($"{head.Id}");

			LLNode curr = head.Next;
			while (curr != head) 
			{
				if (curr == start)
					Console.Write($",[{curr.Id}]");
				else
					Console.Write($",{curr.Id}");
				curr = curr.Next;
			}

			Console.WriteLine();
		}

		static LLNode KnotHashRound(IEnumerable<byte> bytes, LLNode curr, ref int skip, bool debug = false)
		{
			foreach (var len in bytes)
			{
				if(debug) Console.Write($"Reversed segment of length {len} starting at {curr.Id}:,");
				curr = LinkedList.Reverse(curr, len);
				if (debug) DebugPrint(curr);
				if (debug) Console.Write($"Skipped {len+skip} items:,");
				curr = LinkedList.Next(curr, len + skip);
				if (debug) DebugPrint(curr);
				skip++;
			}

			return curr;
		}

		static LLNode CreateNodeList(int numNodes)
		{
			LLNode root = new LLNode(0);
			root.IsHead = true;
			LLNode curr = root;
			for (int i = 1; i < numNodes; i++)
			{
				curr = curr.AddNext(i);
			}
			curr.SetNext(root);
			return root;
		}

		static string KnotHash(string key)
		{
			List<byte> bytes = Encoding.ASCII.GetBytes(key).ToList();

			bytes.Add(17);
			bytes.Add(31);
			bytes.Add(73);
			bytes.Add(47);
			bytes.Add(23);

			LLNode curr = CreateNodeList(256);
			int skip = 0;

			for (int round = 0; round < 64; round++)
			{
				curr = KnotHashRound(bytes, curr, ref skip);
			}

			curr = LinkedList.GetHead(curr);

			List<byte> dense = new List<byte>();
			byte xor = 0;
			for(int i=0; i < 16; i++)
			{
				//Console.WriteLine($"Block {i}:");
				for(int j=0; j < 16; j++)
				{
					if (j == 0)
					{
						//Console.Write($"  {curr.Id}");
						xor = (byte)curr.Id;
					}
					else
					{
						//Console.Write($" ^ {curr.Id}");
						xor = (byte)(xor ^ curr.Id);
					}
					curr = curr.Next;
				}
				//Console.WriteLine($" = {xor}");
				dense.Add(xor);
			}

			var strRep = BitConverter.ToString(dense.ToArray()).Replace("-", "").ToLower(); ;
			if(strRep.Length != 32)
			{
				Debugger.Break();
			}
			return strRep;
		}


		public override void ParseInputLine(string line, int lineNum)
		{
			
		}


		public void Day10Stuff()
		{
			byte[] input2 = new byte[] { 3, 4, 1, 5 };
			LLNode curr = CreateNodeList(5);

			int skip = 0;
			curr = KnotHashRound(input2, curr, ref skip);

			curr = LinkedList.GetHead(curr);
			long prod = curr.Id * curr.Next.Id;
			Console.WriteLine($"{curr.Id} * {curr.Next.Id} = {prod}");

			byte[] input = new byte[] { 206, 63, 255, 131, 65, 80, 238, 157, 254, 24, 133, 2, 16, 0, 1, 3 };
			LLNode root = CreateNodeList(256);

			skip = 0;
			root = KnotHashRound(input, root, ref skip);
			root = LinkedList.GetHead(root);

			prod = root.Id * root.Next.Id;
			Console.WriteLine($"{root.Id} * {root.Next.Id} = {prod}");

			Console.WriteLine($"Empty string: {KnotHash("")}");
			Console.WriteLine($"AoC 2017: {KnotHash("AoC 2017")}");
			Console.WriteLine($"1,2,3: {KnotHash("1,2,3")}");
			Console.WriteLine($"1,2,4: {KnotHash("1,2,4")}");
			Console.WriteLine($"206,63,255,131,65,80,238,157,254,24,133,2,16,0,1,3: {KnotHash("206,63,255,131,65,80,238,157,254,24,133,2,16,0,1,3")}");

		}

		static string key = "hwlqcszp";

		int NumBits(char c)
		{
			switch(c)
			{
				case '0': return 0;
				case '1': return 1;
				case '2': return 1;
				case '3': return 2;
				case '4': return 1;
				case '5': return 2;
				case '6': return 2;
				case '7': return 3;
				case '8': return 1;
				case '9': return 2;
				case 'a': return 2;
				case 'b': return 3;
				case 'c': return 2;
				case 'd': return 3;
				case 'e': return 3;
				case 'f': return 4;
			}

			throw new Exception("What");
		}

		int Num(char c)
		{
			switch (c)
			{
				case '0': return 0;
				case '1': return 1;
				case '2': return 2;
				case '3': return 3;
				case '4': return 4;
				case '5': return 5;
				case '6': return 6;
				case '7': return 7;
				case '8': return 8;
				case '9': return 9;
				case 'a': return 10;
				case 'b': return 11;
				case 'c': return 12;
				case 'd': return 13;
				case 'e': return 14;
				case 'f': return 15;
			}

			throw new Exception("What");
		}

		char[][] m_map;

		public override string Part1()
		{
			ArrayUtil.Build2DArray(ref m_map, 128, 128, ' ');
			
			//key = "flqrgnkx";

			int sumRows = 0;
			for(int row=0; row < 128; row++)
			{
				string rowKey = $"{key}-{row}";

				var hash = KnotHash(rowKey);

				int sumBits = 0;

				int x = 0;
				foreach(var c in hash)
				{
					int numBits = NumBits(c);
					sumBits += numBits;

					int num = Num(c);
					for(int bit=0; bit < 4; bit++)
					{
						if((num & (1 << bit)) > 0)
						{
							m_map[x + 3 - bit][row] = '#';
						}
						else
						{
							m_map[x + 3 - bit][row] = '.';
						}
					}

					x+=4;
				}

				sumRows += sumBits;
			}

			ArrayUtil.PrintArray(m_map, 8, 8);

			return sumRows.ToString();
		}

		public void PaintRegion(int col, int row)
		{
			Vec2 start = new Vec2(col, row);
			Queue<Vec2> toProcess = new Queue<Vec2>();
			toProcess.Enqueue(start);

			while(toProcess.Count > 0)
			{
				Vec2 curr = toProcess.Dequeue();

				m_map[curr.X][curr.Y] = '@';

				foreach(var offset in Vec2.CardinalAdjacent)
				{
					Vec2 neighbor = curr + offset;
					if (neighbor.X >= 0 && neighbor.X < 128 && neighbor.Y >= 0 && neighbor.Y < 128)
					{
						if (m_map[neighbor.X][neighbor.Y] == '#')
						{
							toProcess.Enqueue(neighbor);
						}
					}
				}
			}
		}

		public override string Part2()
		{
			int regions = 0;
			for(int row = 0; row < 128; row++)
			{
				for(int col = 0; col < 128; col++)
				{
					char c = m_map[col][row];

					if (c == '#')
					{
						PaintRegion(col, row);
						regions++;
					}
				}
			}
			return regions.ToString();
		}
	}
}
