using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2020
{
	enum Operation
	{
		Acc,
		Jmp,
		Nop
	}

	class Instruction
	{
		public Operation Operation;
		public int Argument;

		public override string ToString()
		{
			return $"{Operation} {Argument}";
		}
	}

	class Handheld
	{
		List<Instruction> m_data = new List<Instruction>();

		int m_pc = 0;
		long m_accumulator = 0;

		public Handheld(IEnumerable<string> instructions)
		{
			foreach(var s in instructions)
			{
				var splits = s.Split(' ');

				Instruction newIn = new Instruction();
				newIn.Argument = int.Parse(splits[1]);
				switch(splits[0])
				{
					case "acc":
						newIn.Operation = Operation.Acc;
						break;
					case "nop":
						newIn.Operation = Operation.Nop;
						break;
					case "jmp":
						newIn.Operation = Operation.Jmp;
						break;

				}
				m_data.Add(newIn);
			}
		}

		public (int, long) Run(int timeoutMs)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			while(m_pc < m_data.Count())
			{
				Step(false);
				if (sw.ElapsedMilliseconds > timeoutMs)
					return (1, m_accumulator);
			}

			return (0, m_accumulator);
		}

		public (int, long) Step(bool print = true)
		{
			var inst = m_data.ElementAt(m_pc);
			if(print) Console.WriteLine($"Executing instruction {m_pc} - Accum is {m_accumulator}\n  {inst}");

			if(inst.Operation == Operation.Acc)
			{
				m_accumulator += inst.Argument;
				m_pc++;
			}
			else if(inst.Operation == Operation.Jmp)
			{
				m_pc += inst.Argument;
			}
			else if(inst.Operation == Operation.Nop)
			{
				m_pc++;
			}

			return (m_pc, m_accumulator);
		}

	}
}
