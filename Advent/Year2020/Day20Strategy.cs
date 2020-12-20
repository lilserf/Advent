using Advent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent2020.Year2020
{
	enum Edge
	{
		Top = 0,
		Right = 90,
		Bottom = 180,
		Left = 270
	}
	class Tile
	{
		public int Id { get; set; }
		public Dictionary<string, int> EdgeMatches { get; set; }

		char[][] m_data;
		int m_currLine = 0;

		public Tile(int id)
		{
			Id = id;
			ArrayUtil.Build2DArray(ref m_data, 10, 10, ' ');
			EdgeMatches = new Dictionary<string, int>();
		}

		public void AddLine(string line)
		{
			for(int i=0; i < line.Length; i++)
			{
				m_data[m_currLine][i] = line[i];
			}

			m_currLine++;
		}

		Dictionary<Edge, string> m_edges = new Dictionary<Edge, string>();

		public IEnumerable<string> GetEdges()
		{
			string top = "";
			string bottom = "";
			string left = "";
			string right = "";

			for (int i = 0; i < 10; i++)
			{
				top += m_data[0][9 - i];
				bottom += m_data[9][i];
				left += m_data[i][0];
				right += m_data[9 - i][9];
			}

			m_edges[Edge.Top] = top;
			m_edges[Edge.Right] = right;
			m_edges[Edge.Bottom] = bottom;
			m_edges[Edge.Left] = left;

			return new string[] { top, right, bottom, left };
		}

		private int Rotation = 0;
		private bool FlipX = false;
		private bool FlipY = false;

		public Edge WhereIsEdge(string e)
		{
			var original = m_edges.Where(k => k.Value == e).Select(k => k.Key).First();

			int deg = (int)original;
			deg += Rotation;

			return (Edge)(deg % 360);
		}

		public void Flip(Edge e)
		{
			if(e == Edge.Top || e == Edge.Bottom)
			{
				FlipY = true;
			}
			else
			{
				FlipX = true;
			}
		}

		public void Position(string e1, Edge dir1, string e2, Edge dir2)
		{
			Edge curr1 = WhereIsEdge(e1);
			Rotation = (360 + dir1 - curr1) % 360;

			Edge now2 = WhereIsEdge(e2);
			if(now2 != dir2)
			{
				Flip(now2);
			}
		}

		public char GetOuputChar(int outX, int outY)
		{
			if (FlipX)
				outX = 9 - outX;
			if (FlipY)
				outY = 9 - outY;

			int x = 0;
			int y = 0;

			if(Rotation == 0)
			{
				x = outX;
				y = outY;
			}
			if (Rotation == 90)
			{
				x = outY;
				y = 9 - outX;
			}
			if (Rotation == 180)
			{
				x = 9 - outX;
				y = 9 - outY;
			}
			if(Rotation == 270)
			{
				x = 9 - outY;
				y = outX;
			}

			return m_data[y][x];
		}

		public void CopyInto(ref char[][] map, int x, int y)
		{
			for(int i = 1; i < 9; i++)
			{
				for(int j = 1; j < 9; j++)
				{
					char c = GetOuputChar(i, j);
					map[y][x] = c;
				}
			}
		}

	}

	class Day20Strategy : DayLineLoaderBasic
	{
		Dictionary<int, Tile> m_tiles = new Dictionary<int, Tile>();

		Tile m_curr = null;
		public override void ParseInputLine(string line)
		{
			if (string.IsNullOrWhiteSpace(line))
				return;

			var match = Regex.Match(line, @"Tile (\d+):");
			if(match.Success)
			{
				if(m_curr != null)
					m_tiles[m_curr.Id] = m_curr;
				m_curr = new Tile(int.Parse(match.Groups[1].Value));
			}
			else
			{
				m_curr.AddLine(line);
			}
		}

		public override void ParseInputLinesEnd(StreamReader sr)
		{
			m_tiles[m_curr.Id] = m_curr;
		}

		//Dictionary<string, Tile> edgeList = new Dictionary<string, Tile>();

		List<(string, int)> edgeList = new List<(string, int)>();

		public override string Part1()
		{
			foreach(var t in m_tiles.Values)
			{
				foreach(var e in t.GetEdges())
				{
					edgeList.Add((e, t.Id));
				}
			}

			foreach((var edge, var id) in edgeList)
			{
				foreach((var otherEdge, var id2) in edgeList)
				{
					if (id != id2)
					{
						var reverse = new string(otherEdge.Reverse().ToArray());
						if (edge.Equals(reverse) || edge.Equals(otherEdge))
						{
							var tile1 = m_tiles[id];
							var tile2 = m_tiles[id2];

							Console.WriteLine($"Tile {tile1.Id} [{edge}] == [{reverse}] Tile {tile2.Id}");

							tile1.EdgeMatches[edge] = tile2.Id;
							tile2.EdgeMatches[otherEdge] = tile1.Id;
						}
					}
				}
			}

			var corners = m_tiles.Values.Where(x => x.EdgeMatches.Count == 2);
			if (corners.Count() != 4)
				Debugger.Break();

			long prod = 1;
			foreach(var corner in corners)
			{
				prod *= corner.Id;
			}
			return prod.ToString();
		}

		char[][] m_map;

		public override string Part2()
		{
			const int SIDE = 12;
			const int TILE_WIDTH = 8;
			ArrayUtil.Build2DArray(ref m_map, SIDE * TILE_WIDTH, SIDE * TILE_WIDTH, ' ');

			var corners = m_tiles.Values.Where(x => x.EdgeMatches.Count == 2);
			Tile start = corners.First();
			var edges = start.EdgeMatches.Keys;
			var edge1 = edges.ElementAt(0);
			var edge2 = edges.ElementAt(1);
			start.Position(edge1, Edge.Bottom, edge2, Edge.Right);

			string currEdge = edge2;
			Edge currSide = Edge.Right;
			int numPlaced = 1;
			while(numPlaced < 144)
			{

			}

			return "";
		}
	}
}
