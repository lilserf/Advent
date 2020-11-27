using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2019
{
	class Day21Strategy : DayIntCodeStrategy
	{
		public Day21Strategy(string inputFile) : base(inputFile)
		{
		}

		long m_result = 0;

		public override string Part1()
		{
			string program = "NOT C J\nAND D J\nNOT A T\nOR T J\nWALK\n";

			if (program == null)
			{
				Console.WriteLine("Enter ASCIIScript program for springdroid:");
				program = "";
				string command = "";
				while (command != "WALK")
				{
					command = Console.ReadLine();
					program += command + '\n';
				}
			}

			Queue<long> script = new Queue<long>(program.Select(c => (long)c));

			var computer = CreateProgram(() => true, () => script.Dequeue(), Output);
			computer.Run();

			return m_result.ToString();
		}

		private void Output(long obj)
		{
			if (obj > 255)
				m_result = obj;
			else
				Console.Write((char)obj);
		}


		private string MapFromNum(int num)
		{
			string s = "";

			for(int i=0; i < 9; i++)
			{
				char c = ((num >> i) & 1) > 0 ? '#' : '.';
				s = c + s;
			}

			return s;
		}

		public override string Part2()
		{

			//int count = 0;
			//for(int i=0; i < 512; i++)
			//{
			//	if ((i & 0xF) == 0 || (i & 0x1E) == 0 || (i & 0x3C) == 0 || (i & 0x78) == 0 || (i & 0xF0) == 0)
			//		continue;

			//	string s = MapFromNum(i);

			//	if(s[0] == '.')
			//	{
			//		continue;
			//	}
			//	else
			//	{
			//		Console.WriteLine(s);
			//		count++;
			//	}
			//}
			//Console.WriteLine($"Found {count} valid cases.");

			// Jump if:
			// E OR H is safe
			// D is safe
			// ABC is not safe
			string program = "OR A J\nAND B J\nAND C J\nNOT J J\nAND D J\nOR E T\nOR H T\nAND T J\nRUN\n";

			if (program == null)
			{
				Console.WriteLine("Enter ASCIIScript program for springdroid:");
				program = "";
				string command = "";
				while (command != "RUN")
				{
					command = Console.ReadLine();
					program += command + '\n';
				}
			}

			Queue<long> script = new Queue<long>(program.Select(c => (long)c));

			var computer = CreateProgram(() => true, () => script.Dequeue(), Output);
			computer.Run();

			return m_result.ToString();
		}
	}
}
