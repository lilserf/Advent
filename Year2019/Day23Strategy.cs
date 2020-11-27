using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2019
{
	class Day23Strategy : DayIntCodeStrategy
	{
		IntCode[] m_computers;
		Queue<long>[] m_inputs;
		Queue<long>[] m_outputs;
		bool[] m_waiting;
		bool m_natTriggered = false;
		long m_natX = 0;
		long m_natY = 0;

		public Day23Strategy(string inputFile) : base(inputFile)
		{
			m_computers = new IntCode[NUM_NODES];
			m_inputs = new Queue<long>[NUM_NODES];
			m_outputs = new Queue<long>[NUM_NODES];
			m_waiting = new bool[NUM_NODES];
		}

		public static int NUM_NODES = 50;

		public override string Part1()
		{
			for(int i=0; i < NUM_NODES; i++)
			{
				int addr = i;
				m_computers[i] = CreateProgram(() => true, () => GetInput(addr), (x) => SendOutput(addr, x));
				m_inputs[i] = new Queue<long>();
				// Queue up the address
				m_inputs[i].Enqueue(i);
				m_outputs[i] = new Queue<long>();
				m_waiting[i] = false;
			}

			while(!m_natTriggered)
			{
				foreach(var p in m_computers)
				{
					p.Step();
				}
			}

			return m_natY.ToString();
		}

		private void SendOutput(int i, long output)
		{
			m_outputs[i].Enqueue(output);

			if(m_outputs[i].Count == 3)
			{
				int dest = (int)m_outputs[i].Dequeue();

				long x = m_outputs[i].Dequeue();
				long y = m_outputs[i].Dequeue();

				if (dest == 255)
				{
					//Console.WriteLine($"Got a packet {x}, {y} to 255!");
					m_natX = x;
					m_natY = y;
					m_natTriggered = true;
				}
				else
				{
					m_inputs[dest].Enqueue(x);
					m_inputs[dest].Enqueue(y);
				}
			}
		}

		private long GetInput(int i)
		{
			if(m_inputs[i].Count > 0)
			{
				m_waiting[i] = false;
				return m_inputs[i].Dequeue();
			}
			else
			{
				m_waiting[i] = true;
				return -1;
			}
		}

		HashSet<long> m_seenYValues;

		public override string Part2()
		{
			m_seenYValues = new HashSet<long>();

			for (int i = 0; i < NUM_NODES; i++)
			{
				int addr = i;
				m_computers[i] = CreateProgram(() => true, () => GetInput(addr), (x) => SendOutput(addr, x));
				m_inputs[i] = new Queue<long>();
				// Queue up the address
				m_inputs[i].Enqueue(i);
				m_outputs[i] = new Queue<long>();
				m_waiting[i] = false;
			}

			while (true)
			{
				for(int i=0; i < NUM_NODES; i++)
				{
					m_computers[i].Step();
				}

				if(m_inputs.All(q => q.Count == 0) && m_waiting.All(x => x == true))
				{
					//Console.WriteLine("Network is idle!");
					if(m_seenYValues.Contains(m_natY))
					{
						break;
					}
					else
					{
						m_seenYValues.Add(m_natY);
					}

					// Send the NAT stuff on
					m_inputs[0].Enqueue(m_natX);
					m_inputs[0].Enqueue(m_natY);
				}
			}

			return m_natY.ToString();
		}
	}
}
