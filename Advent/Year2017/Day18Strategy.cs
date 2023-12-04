using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2017
{
	class Operand
	{
		string m_rep;

		public Operand(string rep)
		{
			m_rep = rep;
		}

		public char GetRegisterName()
		{
			return m_rep[0];
		}

		public long Evaluate(Dictionary<char, long> registers)
		{
			long result = 0;
			if(long.TryParse(m_rep, out result))
			{
				return result;
			}
			else
			{
				char c = GetRegisterName();
				if(!registers.ContainsKey(c))
				{
					registers[c] = 0;
				}
				return registers[c];
			}
		}
	}

	class Duet
	{
		Dictionary<char, long> m_registers = new Dictionary<char, long>();
		bool m_partB = false;

		List<string> m_data;
		int m_pc;

		public Duet(IEnumerable<string> data, bool partB, int id)
		{
			m_data = new List<string>(data);
			m_pc = 0;
			m_partB = partB;
			if(partB)
			{
				m_registers['p'] = id;
			}
		}

		public long GetRegValue(char c)
		{
			if(!m_registers.ContainsKey(c))
			{
				m_registers[c] = 0;
			}
			return m_registers[c];
		}

		long m_lastSoundFreq = 0;

		public long SendCount => m_sendCount;
		long m_sendCount = 0;

		public long MulCount => m_mulCount;
		long m_mulCount = 0;

		public void SetRegValue(char c, long val)
		{
			m_registers[c] = val;
		}

		private (string, Operand, Operand) MakeCmd(string s)
		{
			var splits = s.Split(' ');

			string cmd = splits[0];
			Operand a = new Operand(splits[1]);
			Operand b = null;
			if(splits.Length > 2)
			{
				b = new Operand(splits[2]);
			}

			return (cmd, a, b);
		}

		public enum State
		{
			Running,
			Done,
			Waiting
		}

		long m_ticks = 0;


		class Jump
		{
			public int Source { get; set; }
			public int FrequentDest { get; set; }
			public bool FoundFrequent { get; set; }
			public int RareDest { get; set; }
			public bool FoundRare { get; set; }

			public Dictionary<char, long> InitialState;
			public Dictionary<char, long> RareState;

			public Jump()
			{
				FoundFrequent = false;
				FoundRare = false;
			}

			public void Print()
			{
				Console.WriteLine($"Jump at {Source} went to {RareDest} instead of {FrequentDest}!");
				Console.Write($"\tInitial state: ");
				foreach (var kvp in InitialState)
				{
					Console.Write($"[{kvp.Key}: {kvp.Value}] ");
				}
				Console.WriteLine();
				Console.Write($"\tRare state:    ");
				foreach (var kvp in RareState)
				{
					Console.Write($"[{kvp.Key}: {kvp.Value}] ");
				}
				Console.WriteLine();
				FoundFrequent = false;
				FoundRare = false;
			}
		}

		Dictionary<int, Jump> m_jumps = new Dictionary<int, Jump>();

		public void PrintState(string prefix, string cmd)
		{
			Console.Write($"{prefix} Tick {m_ticks}: {m_pc}\t{cmd}\t");
			foreach (var kvp in m_registers)
			{
				Console.Write($"[{kvp.Key}: {kvp.Value}] ");
			}
			Console.WriteLine();
			Console.ReadKey();
		}

		public State Tick(Queue<long> inQueue, Queue<long> outQueue, bool debug = false)
		{
			m_ticks++;
			string cmd = m_data[m_pc];

			//Jump j = null;
			//if(cmd.StartsWith("jnz"))
			//{
			//	if (!m_jumps.ContainsKey(m_pc))
			//	{
			//		j = new Jump();
			//		j.Source = m_pc;
			//		j.InitialState = new Dictionary<char, long>(m_registers);
			//		m_jumps[m_pc] = j;
			//	}
			//	else
			//	{
			//		j = m_jumps[m_pc];
			//	}
			//}
			(State done, int offset) = Process(cmd, inQueue, outQueue);
			//if(j != null)
			//{
			//	if(!j.FoundFrequent)
			//	{
			//		j.FrequentDest = m_pc + offset;
			//		j.FoundFrequent = true;
			//	}
			//	else if(!j.FoundRare && m_pc + offset != j.FrequentDest)
			//	{
			//		j.RareDest = m_pc + offset;
			//		j.FoundRare = true;
			//		j.RareState = new Dictionary<char, long>(m_registers);
			//	}
			//}


			//if (debug && (j?.FoundRare ?? false))
			//{
			//	Console.WriteLine($"### TICK {m_ticks}");
			//	j.Print();
			//	Console.ReadKey();
			//}

			if (debug)
			{
				PrintState("", cmd);
			}

			m_pc += offset;

			if (done == State.Done || m_pc < 0 || m_pc >= m_data.Count)
			{
				return State.Done;
			}
			else
			{
				return done;
			}
		}

		private (State, int) Process(string s, Queue<long> inQueue, Queue<long> outQueue)
		{
			(string cmd, Operand x, Operand y) = MakeCmd(s);

			int nextInstruction = 1;

			char reg = x.GetRegisterName();
			State state = State.Running;

			switch(cmd)
			{
				case "snd":
					if (m_partB)
					{
						m_sendCount++;
						outQueue.Enqueue(x.Evaluate(m_registers));
					}
					else
					{
						m_lastSoundFreq = x.Evaluate(m_registers);
					}
					break;
				case "set":
					SetRegValue(reg, y.Evaluate(m_registers));
					break;
				case "add":
					SetRegValue(reg, GetRegValue(reg) + y.Evaluate(m_registers));
					break;
				case "sub":
					SetRegValue(reg, GetRegValue(reg) - y.Evaluate(m_registers));
					break;
				case "mul":
					SetRegValue(reg, GetRegValue(reg) * y.Evaluate(m_registers));
					m_mulCount++;
					break;
				case "mod":
					SetRegValue(reg, GetRegValue(reg) % y.Evaluate(m_registers));
					break;
				case "rcv":
					if (m_partB)
					{
						if(inQueue.Count > 0)
						{
							SetRegValue(reg, inQueue.Dequeue());
						}
						else
						{
							nextInstruction = 0;
							state = State.Waiting;
						}
					}
					else
					{
						if (x.Evaluate(m_registers) != 0)
						{
							Console.WriteLine($"Recovered {m_lastSoundFreq}");
							state = State.Done;
						}
					}
					break;
				case "jgz":
					if (x.Evaluate(m_registers) > 0)
						nextInstruction = (int)y.Evaluate(m_registers);
					break;
				case "jnz":
					if (x.Evaluate(m_registers) != 0)
						nextInstruction = (int)y.Evaluate(m_registers);
					break;
				case "pri":
					{
						var num = y.Evaluate(m_registers);
						SetRegValue(reg, IsPrime(num) ? 1 : 0);
						break;
					}
			}

			return (state, nextInstruction);
		}

		bool IsPrime(long num)
		{
			if (num <= 1) return false;
			if (num == 2) return true;
			if (num % 2 == 0) return false;

			var boundary = (long)Math.Floor(Math.Sqrt(num));

			for (int i = 3; i < boundary; i += 2)
			{
				if(num % i == 0)
				{
					return false;
				}
			}

			return true;
		}
	}

	class Day18Strategy : DayLineLoaderBasic
	{
		List<string> m_data = new List<string>();

		public override void ParseInputLine(string line, int lineNum)
		{
			m_data.Add(line);
		}

		public override string Part1()
		{
			Duet d = new Duet(m_data, false, 0);

			Duet.State done = Duet.State.Running;
			do
			{
				done = d.Tick(null, null);
			}
			while (done != Duet.State.Done);

			return "";
		}

		public override string Part2()
		{
			Duet a = new Duet(m_data, true, 0);
			Duet b = new Duet(m_data, true, 1);

			Queue<long> aToB = new Queue<long>();
			Queue<long> bToA = new Queue<long>();

			Duet.State stateA = Duet.State.Running;
			Duet.State stateB = Duet.State.Running;
			do
			{
				stateA = a.Tick(bToA, aToB);
				stateB = b.Tick(aToB, bToA);
			}
			while (stateA == Duet.State.Running || stateB == Duet.State.Running);

			return b.SendCount.ToString();
		}
	}
}
