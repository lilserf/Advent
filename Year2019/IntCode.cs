using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2019
{
	public class IntCode
	{
		const int DATA_SIZE = 10000;

		long[] m_data;
		long[] m_backup;
		long m_pc;
		long m_relativeBase;

		static bool s_debug = false;

		Func<bool> m_inputReady;
		Func<long> m_input;

		Action<long> m_output;

		public enum ExecutionState
		{
			Init,
			Running,
			StalledForInput,
			Ended,
		}

		ExecutionState m_state;
		public ExecutionState State => m_state;

		enum OpCode
		{
			Add = 1,
			Multiply = 2,
			Input = 3,
			Output = 4,
			JumpIfTrue = 5,
			JumpIfFalse = 6,
			LessThan = 7,
			Equals = 8,
			RelativeBase = 9,
			End = 99
		}

		public IntCode(long[] data, Func<bool> inputReady, Func<long> input, Action<long> output)
		{
			m_data = new long[DATA_SIZE];
			data.CopyTo(m_data, 0);
			m_backup = (long[])m_data.Clone();
			m_state = ExecutionState.Init;

			m_inputReady = inputReady;
			m_input = input;

			m_output = output;
		}

		public void WriteMemory(long addr, long value)
		{
			m_data[addr] = value;
			m_backup[addr] = value;
		}

		public void Reset()
		{
			m_data = new long[DATA_SIZE];
			m_backup.CopyTo(m_data, 0);
		}

		private ref long Resolve(ref long param, int flag)
		{
			if(flag == 1)
			{
				if(s_debug) Console.WriteLine($" ! IMMEDIATE: {param}");
				// Immediate mode
				return ref param;
			}
			else if(flag == 2)
			{
				if (s_debug) Console.WriteLine($" + RELATIVE: {m_relativeBase} + {param} => data[{m_relativeBase + param}] = {m_data[m_relativeBase + param]}");
				// Relative mode
				return ref m_data[m_relativeBase + param];
			}
			else
			{
				if (s_debug) Console.WriteLine($" = POSITION: data[{param}] = {m_data[param]}");
				// Position mode
				return ref m_data[param];
			}
		}

		private ref long GetParam(int index, int flag)
		{
			ref long resolved = ref Resolve(ref m_data[m_pc + index], flag);
			if(s_debug)
				Console.WriteLine($" > Resolved param {index}({flag}) as {resolved}");
			return ref resolved;
		}

		public void Step()
		{
			m_state = ExecutionState.Running;
			OpCode opcode = (OpCode)(m_data[m_pc] % 100);
			// End of program
			if (opcode == OpCode.End)
			{
				m_state = ExecutionState.Ended;
				return;
			}


			long paramFlags = (m_data[m_pc] / 100);

			int flag1 = (int)(paramFlags % 10);
			int flag2 = (int)(paramFlags / 10 % 10);
			int flag3 = (int)(paramFlags / 100);

			if(s_debug)
				Console.WriteLine($"Executing {m_data[m_pc]} - {opcode}...");

			switch (opcode)
			{
				// 1 is Add
				case OpCode.Add:
					{
						ref long dest = ref GetParam(3, flag3);
						dest = GetParam(1, flag1) + GetParam(2, flag2);
						if (s_debug) Console.WriteLine($"     Stored result ({dest}).");
						m_pc += 4;
						break;
					}
				// 2 is Multiply
				case OpCode.Multiply:
					{
						ref long dest = ref GetParam(3, flag3);
						dest = GetParam(1, flag1) * GetParam(2, flag2);
						if (s_debug) Console.WriteLine($"     Stored result ({dest}).");
						m_pc += 4;
						break;
					}
				case OpCode.Input:
					{
						if (m_inputReady())
						{
							ref long dest = ref GetParam(1, flag1);
							dest = m_input();
							if (s_debug)
								Console.WriteLine($"  Got input of {dest}");
							m_pc += 2;
						}
						else
						{
							m_state = ExecutionState.StalledForInput;
						}
						break;
					}
				case OpCode.Output:
					{
						m_output(GetParam(1, flag1));
						m_pc += 2;
						break;
					}
				case OpCode.JumpIfTrue:
					{
						long param = GetParam(1, flag1);
						if (param != 0)
						{
							m_pc = GetParam(2, flag2);
							if (s_debug) Console.WriteLine("     Jumping to m_pc!");
						}
						else
						{
							m_pc += 3;
						}
						break;
					}
				case OpCode.JumpIfFalse:
					{
						long param = GetParam(1, flag1);
						if (param == 0)
						{
							m_pc = GetParam(2, flag2);
							if (s_debug) Console.WriteLine("     Jumping to m_pc!");
						}
						else
						{
							m_pc += 3;
						}
						break;
					}
				case OpCode.LessThan:
					{
						ref long dest = ref GetParam(3, flag3);
						dest = (GetParam(1, flag1) < GetParam(2, flag2)) ? 1 : 0;
						if (s_debug) Console.WriteLine($"     Stored result ({dest}).");
						m_pc += 4;
						break;
					}
				case OpCode.Equals:
					{
						ref long dest = ref GetParam(3, flag3);
						dest = (GetParam(1, flag1) == GetParam(2, flag2)) ? 1 : 0;
						if (s_debug) Console.WriteLine($"     Stored result ({dest}).");
						m_pc += 4;
						break;
					}
				case OpCode.RelativeBase:
					{
						m_relativeBase += GetParam(1, flag1);
						if (s_debug) Console.WriteLine($"     Relative base is now {m_relativeBase}");
						m_pc += 2;
						break;
					}
			}
		}

		public void Run()
		{
			while(m_state != ExecutionState.Ended)
			{
				Step();
			}
		}
	}
}
