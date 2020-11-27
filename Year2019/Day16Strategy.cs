using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2019
{
	class Day16Strategy : DayInputFileStrategy
	{

		byte[] m_input;

		byte[] m_working1;
		byte[] m_working2;

		int m_length = 0;
		int m_offset = 0;

		public Day16Strategy(string file) : base(file)
		{
			m_input = new byte[7000000];
			m_working1 = new byte[7000000];
			m_working2 = new byte[7000000];
		}

		private int Sum(byte[] input, int begin, int end)
		{
			int sum = 0;
			for(int i=begin; i < end; i++)
			{
				sum += input[i];
			}

			return sum;
		}

		private void Transform(byte[] input, byte[] output)
		{
			for(int gen = 0; gen <= m_length; gen++)
			{
				int sum = 0;

				int ptr = gen;
				int skip = 2 * (gen+1);
				while(ptr < m_length)
				{
					sum += Sum(input, ptr, ptr + gen + 1);
					sum -= Sum(input, ptr + skip, ptr + skip + gen + 1);
					ptr += 2 * skip;
				}

				output[gen] = (byte)Math.Abs(sum % 10);
			}
		}

		private string GetAnswer(byte[] arr, int offset)
		{
			string answer = "";
			for (int i = offset; i < offset + 8; i++)
			{
				answer += arr[i].ToString();
			}
			return answer;
		}

		private void Print(byte[] arr, int offset)
		{
			Console.WriteLine(GetAnswer(arr, offset));
		}

		public override string Part1()
		{
			//m_input[0] = (byte)1;
			//m_input[1] = (byte)2;
			//m_input[2] = (byte)3;
			//m_input[3] = (byte)4;
			//m_input[4] = (byte)5;
			//m_input[5] = (byte)6;
			//m_input[6] = (byte)7;
			//m_input[7] = (byte)8;
			//m_length = 8;

			Transform(m_input, m_working1);
			//Print(m_working1, 0);
			Transform(m_working1, m_working2);
			//Print(m_working2, 0);

			for (int i=0; i < 49; i++)
			{
				Transform(m_working2, m_working1);
				//Print(m_working1, 0);
				Transform(m_working1, m_working2);
				//Print(m_working2, 0);
			}

			string answer = "";
			for(int i=0; i < 8; i++)
			{
				answer += m_working2[i].ToString();
			}

			return answer;
		}

		static int MULT = 10000;

		public void ComputeDigitFromEnd(byte[] curr, byte[] prev, int pos)
		{
			if (pos == m_length - 1)
			{
				curr[pos] = prev[pos];
			}
			else
			{
				curr[pos] = (byte)((prev[pos] + curr[pos + 1])%10);
			}
			//Console.WriteLine($"Computed index {pos} as {curr[pos]}");
		}

		public override string Part2()
		{
			//byte[] input = "03036732577212944063491565474664".Select(x => (byte)(x - '0')).ToArray();
			//m_offset = 303673;

			//byte[] input = "02935109699940807407585447034323".Select(x => (byte)(x - '0')).ToArray();
			//m_offset = 293510;

			//byte[] input = "03081770884921959731165446850517".Select(x => (byte)(x - '0')).ToArray();
			//m_offset = 308177;

			//byte[] input = "12345678".Select(x => (byte)(x - '0')).ToArray();
			//m_offset = 4;

			byte[] input = m_input;

			Array.Clear(m_working1, 0, m_working1.Count());
			Array.Clear(m_working2, 0, m_working2.Count());
			// byte[] input = m_input;

			for (int i = 0; i < MULT; i++)
			{
				for (int j = 0; j < m_length; j++)
				{
					m_working1[i * m_length + j] = input[j];
				}
			}
			m_length *= MULT;

			for (int count = 0; count < 50; count++)
			{
				for (int i = m_length-1; i >= m_offset; i--)
				{
					ComputeDigitFromEnd(m_working2, m_working1, i);
				}
				//Print(m_working2, m_length-8);
				for (int i = m_length-1; i >= m_offset; i--)
				{
					ComputeDigitFromEnd(m_working1, m_working2, i);
				}
				//Print(m_working1, m_length - 8);
			}

			return GetAnswer(m_working1, m_offset);
		}

		protected override void ParseInputLine(string line)
		{
			m_offset = int.Parse(line.Substring(0, 7));
			foreach(char c in line)
			{
				m_input[m_length] = (byte)(c - '0');
				m_length++;
			}
		}
	}
}
