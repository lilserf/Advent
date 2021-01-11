using Advent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent2020.Year2018
{
	class Day22Strategy : DayLineLoaderBasic
	{
		public override void ParseInputLine(string line)
		{
			throw new NotImplementedException();
		}

		Dictionary<Vec2, long> m_erosionCache = new Dictionary<Vec2, long>();

		long GeologicIndex(Vec2 pos)
		{
			if (pos == Vec2.Zero)
				return 0;
			else if (pos == TARGET)
				return 0;
			else if (pos.Y == 0)
				return pos.X * 16807;
			else if (pos.X == 0)
				return pos.Y * 48271;
			else
				return ErosionLevel(new Vec2(pos.X - 1, pos.Y)) * ErosionLevel(new Vec2(pos.X, pos.Y - 1));
		}

		long ErosionLevel(Vec2 pos)
		{
			if (!m_erosionCache.ContainsKey(pos))
				m_erosionCache[pos] = (GeologicIndex(pos) + CAVE_DEPTH) % 20183;
			
			return m_erosionCache[pos];
		}

		enum RegionType
		{
			Rocky = 0,
			Wet = 1,
			Narrow = 2,
		}

		char CharForRegionType(RegionType t)
		{
			switch(t)
			{
				case Day22Strategy.RegionType.Rocky: return '.';
				case Day22Strategy.RegionType.Wet: return '=';
				case Day22Strategy.RegionType.Narrow: return '|';
			}
			return '?';
		}

		RegionType ComputeRegionType(long erosionLevel)
		{
			return (RegionType)(erosionLevel % 3);
		}

		RegionType ComputeRegionType(Vec2 pos)
		{
			return ComputeRegionType(ErosionLevel(pos));
		}

		int RiskLevel(RegionType t)
		{
			switch(t)
			{
				case RegionType.Rocky: return 0;
				case RegionType.Wet: return 1;
				case RegionType.Narrow: return 2;
			}
			return 0;
		}

		int RiskLevel(Vec2 pos)
		{
			return RiskLevel(ComputeRegionType(pos));
		}

		// depth: 10689
		// target: 11,722
		static Vec2 TARGET = new Vec2(11, 722);
		long CAVE_DEPTH = 10689;

		// test data
		//static Vec2 TARGET = new Vec2(10,10);
		//long CAVE_DEPTH = 510;

		public override string Part1()
		{
			int riskSum = 0;
			for(int row = 0; row <= TARGET.Y; row++)
			{
				for(int col = 0; col <= TARGET.X; col++)
				{
					Vec2 pos = new Vec2(col, row);
					var regionType = ComputeRegionType(pos);
					riskSum += RiskLevel(regionType);

					//Console.Write(CharForRegionType(regionType));
				}
				//Console.WriteLine();
			}


			return riskSum.ToString();
		}

		public void PrintMap()
		{
			for (int row = 0; row <= TARGET.Y; row++)
			{
				for (int col = 0; col <= TARGET.X; col++)
				{
					var regionType = ComputeRegionType(new Vec2(col, row));
					Console.Write(CharForRegionType(regionType));
				}
				Console.WriteLine();
			}
		}

		enum Tool
		{
			ClimbingGear,
			Torch,
			None
		}

		class RouteState : IComparable<RouteState>
		{
			public Vec2 Pos { get; private set; }
			public Tool CurrTool { get; private set; }
			public int Minutes { get; private set; }

			public HashSet<int> Visited { get; private set; }

			public RouteState(Vec2 pos, Tool tool, IEnumerable<int> visited = null)
			{
				Pos = pos;
				CurrTool = tool;
				Minutes = 0;
				if (visited == null)
				{
					Visited = new HashSet<int>();
				}
				else
				{
					Visited = new HashSet<int>(visited);
				}
			}

			private int NumForVec2(Vec2 pos)
			{
				return (pos.Y << 8) + pos.X;
			}

			public bool HasVisited(Vec2 pos)
			{
				int num = NumForVec2(pos);
				return Visited.Contains(num);
			}

			public RouteState SwitchTool(Tool tool)
			{
				RouteState r = new RouteState(Pos, tool, Visited);
				r.Minutes = Minutes + 7;
				return r;
			}

			public RouteState Move(Vec2 offset)
			{
				Visited.Add(NumForVec2(Pos));
				RouteState r = new RouteState(Pos + offset, CurrTool, Visited);
				r.Minutes = Minutes + 1;
				return r;
			}

			public bool CanEnter(RegionType r)
			{
				switch(CurrTool)
				{
					case Tool.ClimbingGear: return r != RegionType.Narrow;
					case Tool.Torch: return r != RegionType.Wet;
					case Tool.None: return r != RegionType.Rocky;
				}

				throw new Exception("wat");
			}

			public override string ToString()
			{
				return $"{Minutes} minutes: At {Pos} with {CurrTool}";
			}

			static int DISTANCE_CLOSE = 20;

			public static int Score(RouteState r)
			{
				int dist = Vec2.ManhattanDistance(r.Pos, TARGET);
				int time = r.Minutes;

				return time;
				//if(dist < DISTANCE_CLOSE)
				//{
				//	return 1200 - time;
				//}
				//else
				//{
				//	return dist;
				//}
			}

			float DISTANCE_FACTOR = 1.0f;
			float TIME_FACTOR = 1.0f;
			public int CompareTo(RouteState other)
			{
				return Score(this) - Score(other);
				//int distanceDiff = Vec2.ManhattanDistance(Pos, TARGET) - Vec2.ManhattanDistance(other.Pos, TARGET);
				//int minDiff = Minutes - other.Minutes;

				//return (int)(DISTANCE_FACTOR * distanceDiff) + (int)(TIME_FACTOR * minDiff);
			}
		}

		IEnumerable<Tool> GetValidTools(RegionType r)
		{
			switch(r)
			{
				case RegionType.Rocky: return new Tool[] { Tool.ClimbingGear, Tool.Torch };
				case RegionType.Wet: return new Tool[] { Tool.ClimbingGear, Tool.None };
				case RegionType.Narrow: return new Tool[] { Tool.None, Tool.Torch };
			}

			throw new Exception("wat");
		}

		Tool GetToolForRegions(RegionType a, RegionType b)
		{
			return m_transitionTools[(int)a][(int)b];
		}

		Tool[][] m_transitionTools = new Tool[3][];

		bool ShouldAbandon(RouteState curr, int shortest)
		{
			// We know from SO MANY ITERATIONS that it's lower than this
			if(curr.Minutes > 1200)
			{
				return true;
			}
			// Abandon this longer route
			if (curr.Minutes > shortest)
			{
				return true;
			}
			// Abandon a route that's too far away to beat the best at top speed
			if (curr.Minutes + Vec2.ManhattanDistance(curr.Pos, TARGET) > shortest)
			{
				return true;
			}
			// Abandon a route that's too far away to beat while also switching to torch
			if (curr.CurrTool != Tool.Torch && curr.Minutes + Vec2.ManhattanDistance(curr.Pos, TARGET) + 7 > shortest)
			{
				return true;
			}
			return false;
		}

		private void PrintSnapshot(int width, int height)
		{
			for (int row = 0; row < height; row++)
			{
				for (int col = 0; col < width; col++)
				{
					Vec2 v = new Vec2(col, row);
					int numBest = 0;

					bool torch = (HasBest(v, Tool.Torch));
					bool gear = (HasBest(v, Tool.ClimbingGear));
					bool none = (HasBest(v, Tool.None));

					char c = ' ';

					if (torch) c = 'T';
					if (gear) c = 'G';
					if (none) c = 'N';
					if (torch && gear) c = 'a';
					if (torch && none) c = 'l';
					if (gear && none) c = 'c';

					if (m_reason.ContainsKey(v))
						c = m_reason[v];

					if (v == TARGET)
						c = '*';
					Console.Write(c);
				}
				Console.Write("\t\t");
				for(int col = 0; col < width; col++)
				{
					var regionType = ComputeRegionType(new Vec2(col, row));
					Console.Write(CharForRegionType(regionType));
				}

				Console.WriteLine();
			}
		}

		private bool HasBest(Vec2 p, Tool t)
		{
			var key = (p, t);
			return m_bestTime.ContainsKey(key);
		}


		Dictionary<(Vec2, Tool), int> m_bestTime = new Dictionary<(Vec2, Tool), int>();
		Dictionary<Vec2, char> m_reason = new Dictionary<Vec2, char>();

		public override string Part2()
		{
			m_transitionTools[0] = new Tool[3] { Tool.None,			Tool.ClimbingGear,	Tool.Torch };
			m_transitionTools[1] = new Tool[3] { Tool.ClimbingGear, Tool.None,			Tool.None};
			m_transitionTools[2] = new Tool[3] { Tool.Torch,		Tool.None,			Tool.None};


			RouteState init = new RouteState(Vec2.Zero, Tool.Torch);

			MinHeap<RouteState> pending = new MinHeap<RouteState>(10000000);

			
			//MinHeap<RouteState> finished = new MinHeap<RouteState>(100000);
			//int shortest = int.MaxValue;
			int finishedCount = 0;

			pending.Add(init);

			bool debug = false;
			int ticks = 0;
			int processed = 0;
			int abandoned = 0;
			int skipped = 0;
			int outclassed = 0;
			int improved = 0;
			bool printAndStop = false;
			while(pending.Count() > 0)
			{
				ticks++;
				//var shortest = finished.IsEmpty() ? int.MaxValue : finished.Peek().Minutes;

				var currBestKey = (TARGET, Tool.Torch);
				var currBestTime = m_bestTime.ContainsKey(currBestKey) ? m_bestTime[currBestKey] : 1200;
				
				if (ticks % 1000 == 0)
				{
					if (printAndStop)
					{
						PrintSnapshot(50, 100);
						Console.ReadKey();
					}
					Console.WriteLine($"{pending.Count()} pending\t{m_bestTime.Count()} cells\t{improved} improved\t{abandoned} abandoned\tBest so far is {currBestTime}");
					improved = 0;
					abandoned = 0;
				}

				//if (ticks % 100000 == 0)
				//{
				//	Console.WriteLine($"{ticks/100000}: {pending.Count()} routes pending, {finishedCount} finished. Best so far: {shortest} minutes. {skipped} skipped, {abandoned} abandoned, and {outclassed} outclassed since last report.");
				//	abandoned = 0;
				//	processed = 0;
				//	skipped = 0;
				//	outclassed = 0;
				//}

				RouteState curr = pending.Pop();
				processed++;
				if(debug) Console.WriteLine($"Processing [{curr}]...");

				if(ShouldAbandon(curr, currBestTime))
				{
					abandoned++;
					m_reason[curr.Pos] = 'x';
					if (debug) Console.WriteLine($"  Abandoned for being too long already!");
					continue;
				}

				foreach (Vec2 offset in Vec2.CardinalAdjacent)
				{
					Vec2 newPos = curr.Pos + offset;

					if (newPos.X < 0 || newPos.Y < 0)// || newPos.X > (TARGET.X * 1.4) || newPos.Y > (TARGET.Y * 1.4))
					{
						continue;
					}
					else if(curr.HasVisited(newPos))
					{
						continue;
					}

					if (debug) Console.Write($"  Checking new position {newPos}...");
					RegionType currRegion = ComputeRegionType(curr.Pos);
					RegionType newRegion = ComputeRegionType(newPos);
					if (curr.CanEnter(newRegion))
					{
						if (debug) Console.WriteLine("can enter with this tool.");
						var key = (newPos, curr.CurrTool);
						var best = m_bestTime.ContainsKey(key) ? m_bestTime[key] : int.MaxValue;
						var newRoute = curr.Move(offset);

						if (newRoute.Minutes < best)
						{
							if (best != int.MaxValue)
							{
								if(debug) Console.WriteLine($"    New time of {newRoute.Minutes} is better than old time of {best}.");
								improved++;
							}
							else
							{
								if (debug) Console.WriteLine($"    First time reaching here - took {newRoute.Minutes}.");
							}
							m_bestTime[key] = newRoute.Minutes;
							pending.Add(newRoute);
						}
						else
						{
							if (debug) Console.WriteLine($"    Time of {newRoute.Minutes} didn't beat old time of {best}.");
						}
					}
					else
					{
						var tool = GetToolForRegions(currRegion, newRegion);
						if (debug) Console.WriteLine($"need to switch to {tool}!");
						var key = (newPos, tool);
						var best = m_bestTime.ContainsKey(key) ? m_bestTime[key] : int.MaxValue;
						var newRoute = curr.SwitchTool(tool).Move(offset);

						if(newRoute.Minutes < best)
						{
							if (best != int.MaxValue)
							{
								if (debug) Console.WriteLine($"    New time of {newRoute.Minutes} is better than old time of {best}.");
								improved++;
							}
							else
							{
								if (debug) Console.WriteLine($"    First time reaching here - took {newRoute.Minutes}.");
							}
							m_bestTime[key] = newRoute.Minutes;
							pending.Add(newRoute);
						}
						else
						{
							if (debug) Console.WriteLine($"    Time of {newRoute.Minutes} didn't beat old time of {best}.");
						}
					}
				}


				//if(curr.Pos == Vec2.Zero)
				//{
				//	Debugger.Break();
				//}

				//if(ShouldAbandon(curr, shortest))
				//{
				//	abandoned++;
				//	continue;
				//}

				//if (curr.Pos == TARGET && curr.CurrTool == Tool.Torch)
				//{
				//	finishedCount++;
				//	if(curr.Minutes < shortest)
				//	{
				//		shortest = curr.Minutes;
				//	}
				//}
				//else if(curr.Pos == TARGET)
				//{
				//	pending.Add(curr.SwitchTool(Tool.Torch));
				//}
				//else
				//{
				//	foreach(Vec2 offset in Vec2.CardinalAdjacent)
				//	{
				//		Vec2 newPos = curr.Pos + offset;

				//		if(newPos.X < 0 || newPos.Y < 0)// || newPos.X > (TARGET.X * 1.4) || newPos.Y > (TARGET.Y * 1.4))
				//		{
				//			continue;
				//		}
				//		else if(curr.HasVisited(newPos))
				//		{
				//			continue;
				//		}

				//		RegionType currRegion = ComputeRegionType(curr.Pos);
				//		RegionType newRegion = ComputeRegionType(newPos);
				//		if (curr.CanEnter(newRegion))
				//		{
				//			var newRoute = curr.Move(offset);
				//			if (!ShouldAbandon(newRoute, shortest))
				//			{
				//				pending.Add(newRoute);
				//			}
				//			else
				//			{
				//				skipped++;
				//			}
				//		}
				//		else
				//		{
				//			var tool = GetToolForRegions(currRegion, newRegion);
				//			var newRoute = curr.SwitchTool(tool).Move(offset);
				//			if (!ShouldAbandon(newRoute, shortest))
				//			{
				//				pending.Add(newRoute);
				//			}
				//			else
				//			{
				//				skipped++;
				//			}
				//		}
				//	}
				//}
			}

			PrintSnapshot(50, 750);
			Console.WriteLine($"{pending.Count()} pending\t{m_bestTime.Count()} cells\t{improved} improved\t{abandoned} abandoned\t");

			var shortest = m_bestTime[(TARGET, Tool.Torch)];

			return shortest.ToString();
		}
	}
}
