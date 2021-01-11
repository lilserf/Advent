using Advent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent2020.Year2017
{

	class Day22Strategy : DayLineLoaderBasic
	{
		enum NodeState
		{
			Clean,
			Infected,
			Weakened,
			Flagged
		}

		Dictionary<Vec2, NodeState> m_map = new Dictionary<Vec2, NodeState>();
		int m_count = 0;
		public override void ParseInputLine(string line)
		{
			for(int i=0; i < line.Length; i++)
			{
				Vec2 pos = new Vec2(i, m_count);
				m_map[pos] = line[i] == '#' ? NodeState.Infected : NodeState.Clean;
			}
			m_count++;
		}

		Dictionary<Vec2, NodeState> m_init;

		public override void ParseInputLinesEnd(StreamReader sr)
		{
			m_init = new Dictionary<Vec2, NodeState>(m_map);
		}

		NodeState GetNodeState(Vec2 pos)
		{
			if (!m_map.ContainsKey(pos))
				return NodeState.Clean;
			else
				return m_map[pos];
		}


		int m_infectCount = 0;
		void Infect(Vec2 pos)
		{
			m_map[pos] = NodeState.Infected;
			m_infectCount++;
		}

		void Clean(Vec2 pos)
		{
			m_map[pos] = NodeState.Clean;
		}

		void Flag(Vec2 pos)
		{
			m_map[pos] = NodeState.Flagged;
		}

		void Weaken(Vec2 pos)
		{
			m_map[pos] = NodeState.Weakened;
		}

		Vec2 TurnRight(Vec2 curr)
		{
			if (curr == Vec2.Up) return Vec2.Right;
			if (curr == Vec2.Right) return Vec2.Down;
			if (curr == Vec2.Down) return Vec2.Left;
			if (curr == Vec2.Left) return Vec2.Up;

			throw new Exception("what");
		}

		Vec2 TurnLeft(Vec2 curr)
		{
			if (curr == Vec2.Up) return Vec2.Left;
			if (curr == Vec2.Right) return Vec2.Up;
			if (curr == Vec2.Down) return Vec2.Right;
			if (curr == Vec2.Left) return Vec2.Down;

			throw new Exception("what");
		}

		Vec2 Reverse(Vec2 curr)
		{
			if (curr == Vec2.Up) return Vec2.Down;
			if (curr == Vec2.Right) return Vec2.Left;
			if (curr == Vec2.Down) return Vec2.Up;
			if (curr == Vec2.Left) return Vec2.Right;

			throw new Exception("what");
		}

		public override string Part1()
		{
			Vec2 heading = Vec2.Up;
			int xMax = m_map.Keys.Max(foo => foo.X);
			int yMax = m_map.Keys.Max(foo => foo.Y);

			Vec2 pos = new Vec2(xMax / 2, yMax / 2);

			for (int i = 0; i < 10000; i++)
			{
				if (GetNodeState(pos) == NodeState.Infected)
				{
					heading = TurnRight(heading);
				}
				else
				{
					heading = TurnLeft(heading);
				}

				if (GetNodeState(pos) == NodeState.Infected)
				{
					Clean(pos);
				}
				else
				{
					Infect(pos);
				}

				pos = pos + heading;
			}

			return m_infectCount.ToString();
		}

		public override string Part2()
		{
			m_map = new Dictionary<Vec2, NodeState>(m_init);
			m_infectCount = 0;

			Vec2 heading = Vec2.Up;
			int xMax = m_map.Keys.Max(foo => foo.X);
			int yMax = m_map.Keys.Max(foo => foo.Y);

			Vec2 pos = new Vec2(xMax / 2, yMax / 2);

			for (int i = 0; i < 10000000; i++)
			{
				var nodeState = GetNodeState(pos);

				if (nodeState == NodeState.Clean)
					heading = TurnLeft(heading);
				else if (nodeState == NodeState.Infected)
					heading = TurnRight(heading);
				else if (nodeState == NodeState.Flagged)
					heading = Reverse(heading);

				if (nodeState == NodeState.Clean)
					Weaken(pos);
				else if (nodeState == NodeState.Weakened)
					Infect(pos);
				else if (nodeState == NodeState.Infected)
					Flag(pos);
				else if (nodeState == NodeState.Flagged)
					Clean(pos);

				pos = pos + heading;
			}

			return m_infectCount.ToString();
		}
	}
}
