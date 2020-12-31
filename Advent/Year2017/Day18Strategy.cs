using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent2020.Year2017
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

		public State Tick(Queue<long> inQueue, Queue<long> outQueue)
		{
			string cmd = m_data[m_pc];
			(State done, int offset) = Process(cmd, inQueue, outQueue);
			m_pc += offset;

			if(done == State.Done || m_pc < 0 || m_pc > m_data.Count)
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
				case "mul":
					SetRegValue(reg, GetRegValue(reg) * y.Evaluate(m_registers));
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
			}

			return (state, nextInstruction);
		}
	}

	class Day18Strategy : DayLineLoaderBasic
	{
		List<string> m_data = new List<string>();

		public override void ParseInputLine(string line)
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
