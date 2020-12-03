using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent2020.Year2020
{
	class Day03Strategy : DayMapInputFileStrategy
	{
		char[][] m_map;
		static int MAP_WIDTH = 31;

		public Day03Strategy(string file)
			: base(file)
		{
			ArrayUtil.Build2DArray<char>(ref m_map, MAP_WIDTH, 400, ' ');
		}

		protected override void ReadMapCharacter(Vec2 position, char c)
		{
			m_map[position.X][position.Y] = c;
		}

		int CheckSlope(Vec2 offset)
		{
			Vec2 pos = new Vec2(0, 0);
			int trees = 0;
			while (true)
			{
				pos += offset;
				if (pos.X >= MAP_WIDTH)
					pos.X = pos.X % MAP_WIDTH;
				if (m_map[pos.X][pos.Y] == ' ')
					break;
				if (m_map[pos.X][pos.Y] == '#')
					trees++;
			}
			return trees;
		}

		public override string Part1()
		{
			//ArrayUtil.PrintArray(m_map, 31, 400);


			return CheckSlope(new Vec2(3,1)).ToString();
		}

		public override string Part2()
		{
			long num = (long)CheckSlope(new Vec2(1, 1)) * (long)CheckSlope(new Vec2(3, 1)) * (long)CheckSlope(new Vec2(5, 1)) * (long)CheckSlope(new Vec2(7, 1)) * (long)CheckSlope(new Vec2(1, 2));
			return num.ToString() ;
		}
	}
}
