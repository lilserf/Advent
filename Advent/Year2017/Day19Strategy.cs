using Advent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2017
{
	class Day19Strategy : DayLineLoaderBasic
	{
		char[][] m_map;

		int MAP_WIDTH = 202;

		public override void ParseInputLinesBegin(StreamReader sr)
		{
			ArrayUtil.Build2DArray(ref m_map, MAP_WIDTH, MAP_WIDTH, ' ');
		}

		int m_currLine = 0;
		public override void ParseInputLine(string line, int lineNum)
		{
			for(int i=0; i < line.Length; i++)
			{
				m_map[m_currLine][i] = line[i];
			}
			m_currLine++;
		}

		Vec2 FindNewDir(Vec2 curr, Vec2 currDir)
		{
			foreach(var offset in Vec2.CardinalAdjacent)
			{
				Vec2 fromDir = currDir * -1;
				if(offset != fromDir)
				{
					Vec2 check = curr + offset;
					if (m_map[check.Y][check.X] != ' ')
					{
						return offset;
					}
				}
			}

			return Vec2.Zero;
		}

		int m_steps = 1;

		public override string Part1()
		{
			Vec2 startPos = new Vec2(0, 0);

			for (int i = 0; i < MAP_WIDTH; i++)
			{
				if(m_map[0][i] == '|')
				{
					startPos.X = i;
					break;
				}
			}

			Queue<char> letters = new Queue<char>();
			Vec2 curr = startPos;
			Vec2 currDir = Vec2.Down;
			while (true)
			{
				curr += currDir;
				m_steps++;
				char c = m_map[curr.Y][curr.X];

				if(c == '+')
				{
					currDir = FindNewDir(curr, currDir);
				}
				else if(c == '|' || c == '-')
				{
					// do nothing
				}
				else if(c == ' ')
				{
					return letters.Aggregate("", (s, x) => s + x);
				}
				else
				{
					letters.Enqueue(c);
				}
			}

			return "";
		}

		public override string Part2()
		{
			return (m_steps-1).ToString();
		}
	}
}
