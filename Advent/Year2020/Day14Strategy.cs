using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent2020.Year2020
{
	class Day14Strategy : DayLineLoaderBasic
	{
		List<string> m_data = new List<string>();

		public override void ParseInputLine(string line)
		{
			m_data.Add(line);
		}

		Dictionary<int, long> m_memory = new Dictionary<int, long>();

		public override string Part1()
		{
			long posMask = 0;
			long negMask = 0;

			foreach (var line in m_data)
			{

				var match = Regex.Match(line, @"mask = (.*)");
				if(match.Success)
				{
					posMask = 0;
					negMask = 0;
					foreach(char c in match.Groups[1].Value)
					{
						if(c == '1')
						{
							posMask = (posMask << 1) | 1;
							negMask = (negMask << 1);
						}
						else if(c == '0')
						{
							posMask = posMask << 1;
							negMask = (negMask << 1) | 1;
						}
						else
						{
							posMask = posMask << 1;
							negMask = negMask << 1;
						}
					}
				}
				else
				{
					var memMatch = Regex.Match(line, @"mem\[(\d+)\] = (\d+)");
					if(memMatch.Success)
					{
						long value = long.Parse(memMatch.Groups[2].Value);
						value |= posMask;
						value &= ~negMask;
						int index = int.Parse(memMatch.Groups[1].Value);
						m_memory[index] = value;
					}
				}

			}

			return m_memory.Values.Sum().ToString();
		}

		Dictionary<long, long> m_mem2 = new Dictionary<long, long>();

		private void WriteToIndex(long index, string mask, long value)
		{
			List<char[]> addrs = new List<char[]>();
			var init = (Convert.ToString(index, 2).ToCharArray());

			char[] full = new char[36];
			for(int i=0; i < 36; i++)
			{
				full[i] = '0';
			}

			for(int i = 0; i < init.Length; i++)
			{
				full[36-init.Length+i] = init[i];
			}
			Console.WriteLine($"Told to write to address {new string(full)} (decimal {index})");
			Console.WriteLine($"Mask is [{mask}]");
			addrs.Add(full);

			for(int i=0; i < mask.Length; i++)
			{
				char c = mask[i];
				if(c == '1')
				{
					foreach(var addr in addrs)
					{
						addr[i] = c;
					}
				}
				else if(c == 'X')
				{
					List<char[]> additions = new List<char[]>();
					
					foreach (var addr in addrs)
					{
						char[] copy = (char[])addr.Clone();
						addr[i] = '0';
						copy[i] = '1';
						additions.Add(copy);
					}

					addrs.AddRange(additions);
				}
			}

			foreach(var addr in addrs)
			{
				long iAddr = Convert.ToInt64(new string(addr), 2);
				Console.WriteLine($"Writing {value} to address {new string(addr)} (decimal {iAddr})");
				m_mem2[iAddr] = value;
			}
		}

		public override string Part2()
		{
			string currMask = "";
			foreach (var line in m_data)
			{
				var match = Regex.Match(line, @"mask = (.*)");
				if (match.Success)
				{
					currMask = match.Groups[1].Value;
				}
				else
				{
					var memMatch = Regex.Match(line, @"mem\[(\d+)\] = (\d+)");
					if (memMatch.Success)
					{
						long value = long.Parse(memMatch.Groups[2].Value);
						long index = long.Parse(memMatch.Groups[1].Value);
						WriteToIndex(index, currMask, value);
					}
				}

			}

			return m_mem2.Values.Sum().ToString();
		}
	}
}
