using Advent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent.Year2020
{

	enum Edge
	{
		Top = 0,
		Right = 90,
		Bottom = 180,
		Left = 270
	}

	public class Constants
	{
		public static int WIDTH = 10;
		public static int WIDTH_MINUS_1 = 9;
	}

	abstract class Transform
	{

		public static Transform Rotate(int deg)
		{
			return new RotateTransform(deg);
		}

		public static Transform FlipX()
		{
			return new FlipXTransform();
		}

		public static Transform FlipY()
		{
			return new FlipYTransform();
		}

		public abstract (Edge, string) Apply(Edge e, string s);

		public abstract (Edge, string) Unapply(Edge e, string s);

		public abstract (int, int) ApplyPoint(int x, int y);

		public abstract (int, int) UnapplyPoint(int x, int y);
	}


	class RotateTransform : Transform
	{
		int m_degrees;
		public RotateTransform(int degrees)
		{
			m_degrees = degrees;
		}

		public override (Edge, string) Apply(Edge e, string s)
		{
			int startDeg = (int)e;
			int endDeg = (360 + startDeg + m_degrees) % 360;
			return ((Edge)endDeg, s);
		}

		public override (int, int) ApplyPoint(int x, int y)
		{
			int outX = 0;
			int outY = 0;

			if (m_degrees == 0)
			{
				outX = x;
				outY = y;
			}
			if (m_degrees == 90)
			{
				outX = Constants.WIDTH_MINUS_1 - y;
				outY = x;
			}
			if (m_degrees == 180)
			{
				outX = Constants.WIDTH_MINUS_1 - x;
				outY = Constants.WIDTH_MINUS_1 - y;
			}
			if (m_degrees == 270)
			{
				outX = y;
				outY = Constants.WIDTH_MINUS_1 - x;
			}

			return (outX, outY);
		}

		public override (Edge, string) Unapply(Edge e, string s)
		{
			int endDeg = (int)e;
			int startDeg = (360 + endDeg - m_degrees) % 360;
			return ((Edge)startDeg, s);
		}

		public override (int, int) UnapplyPoint(int x, int y)
		{
			int outX = 0;
			int outY = 0;

			if (m_degrees == 0)
			{
				outX = x;
				outY = y;
			}
			if (m_degrees == 90)
			{
				outX = y;
				outY = Constants.WIDTH_MINUS_1 - x;
			}
			if (m_degrees == 180)
			{
				outX = Constants.WIDTH_MINUS_1 - x;
				outY = Constants.WIDTH_MINUS_1 - y;
			}
			if (m_degrees == 270)
			{
				outX = Constants.WIDTH_MINUS_1 - y;
				outY = x;
			}

			return (outX, outY);
		}
	}

	class FlipXTransform : Transform
	{
		public override (Edge, string) Apply(Edge e, string s)
		{
			string reversed = new string(s.Reverse().ToArray());
			switch (e)
			{
				case Edge.Top:
				case Edge.Bottom:
					return (e, reversed);
				case Edge.Left:
					return (Edge.Right, reversed);
				case Edge.Right:
					return (Edge.Left, reversed);
				default:
					throw new Exception("What?");
			}
		}

		public override (int, int) ApplyPoint(int x, int y)
		{
			return (Constants.WIDTH_MINUS_1 - x, y);
		}

		public override (Edge, string) Unapply(Edge e, string s)
		{
			return Apply(e, s);
		}

		public override (int, int) UnapplyPoint(int x, int y)
		{
			return ApplyPoint(x, y);
		}
	}

	class FlipYTransform : Transform
	{
		public override (Edge, string) Apply(Edge e, string s)
		{
			string reversed = new string(s.Reverse().ToArray());
			switch (e)
			{
				case Edge.Left:
				case Edge.Right:
					return (e, reversed);
				case Edge.Top:
					return (Edge.Bottom, reversed);
				case Edge.Bottom:
					return (Edge.Top, reversed);
				default:
					throw new Exception("What?");
			}
		}

		public override (int, int) ApplyPoint(int x, int y)
		{
			return (x, Constants.WIDTH_MINUS_1 - y);
		}

		public override (Edge, string) Unapply(Edge e, string s)
		{
			return Apply(e, s);
		}

		public override (int, int) UnapplyPoint(int x, int y)
		{
			return ApplyPoint(x, y);
		}
	}

	class Tile
	{
		public int Id { get; set; }
		public Dictionary<string, int> EdgeMatches { get; set; }

		char[][] m_data;
		int m_currLine = 0;

		List<Transform> m_transforms;

		public Tile(int id)
		{
			Id = id;
			ArrayUtil.Build2DArray(ref m_data, 10, 10, ' ');
			EdgeMatches = new Dictionary<string, int>();
			m_transforms = new List<Transform>();
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
				top += m_data[0][Constants.WIDTH_MINUS_1 - i];
				bottom += m_data[Constants.WIDTH_MINUS_1][i];
				left += m_data[i][0];
				right += m_data[Constants.WIDTH_MINUS_1 - i][Constants.WIDTH_MINUS_1];
			}

			m_edges[Edge.Top] = top;
			m_edges[Edge.Right] = right;
			m_edges[Edge.Bottom] = bottom;
			m_edges[Edge.Left] = left;

			return new string[] { top, right, bottom, left };
		}

		public (string, string) GetCurrentSide(Edge e)
		{
			string unused = "";
			(e, unused) = UnapplyTransforms(e, unused);

			string orig = m_edges[e];
			string final = orig;
			(e, final) = ApplyTransforms(e, orig);

			return (orig, final);
		}

		public (Edge, string) FindCurrentStateOfEdge(string s)
		{
			Edge edge = m_edges.Where(k => k.Value == s).Select(k => k.Key).First();
			return ApplyTransforms(edge, s);
		}

		public (Edge, string) ApplyTransforms(Edge edge, string s)
		{
			foreach (var t in m_transforms)
			{
				(edge, s) = t.Apply(edge, s);
			}
			return (edge, s);
		}

		public (Edge, string) UnapplyTransforms(Edge edge, string s)
		{
			for(int i = m_transforms.Count-1; i >= 0; i--)
			{
				var t = m_transforms.ElementAt(i);
				(edge, s) = t.Unapply(edge, s);
			}
			return (edge, s);
		}

		public void Rotate(int deg)
		{
			m_transforms.Add(Transform.Rotate(deg));
		}

		public void FlipMove(Edge e)
		{
			if (e == Edge.Top || e == Edge.Bottom)
				m_transforms.Add(Transform.FlipY());
			else
				m_transforms.Add(Transform.FlipX());
		}

		public void FlipNoMove(Edge e)
		{
			if (e == Edge.Top || e == Edge.Bottom)
				m_transforms.Add(Transform.FlipX());
			else
				m_transforms.Add(Transform.FlipY());
		}


		public void RotateFromTo(Edge start, Edge end)
		{
			int rotDeg = (360 + end - start) % 360;
			Rotate(rotDeg);
		}

		public void Position(string e1, Edge dir1, string e2, Edge dir2)
		{
			(Edge curr1, string s) = FindCurrentStateOfEdge(e1);
			RotateFromTo(curr1, dir1);

			(Edge curr2, string s2) = FindCurrentStateOfEdge(e2);
			if (curr2 != dir2)
			{
				// Flip this edge across to the other side of the tile
				FlipMove(curr2);
			}
		}

		public void PositionMatching(string edge, Edge dirToFace, string matching)
		{
			(Edge curr, string currVal) = FindCurrentStateOfEdge(edge);
			RotateFromTo(curr, dirToFace);

			(curr, currVal) = FindCurrentStateOfEdge(edge);
			if (currVal != new string(matching.Reverse().ToArray()))
			{
				// Flip this edge in place so it's reversed
				FlipNoMove(dirToFace);
			}
		}

		public (int, int) UnapplyTransforms(int x, int y)
		{
			for (int i = m_transforms.Count - 1; i >= 0; i--)
			{
				var t = m_transforms.ElementAt(i);
				(x, y) = t.UnapplyPoint(x, y);
			}
			return (x, y);
		}

		public char GetOuputChar(int outX, int outY)
		{
			(int x, int y) = UnapplyTransforms(outX, outY);
			return m_data[y][x];
		}

		public void CopyInto(ref char[][] map, int x, int y)
		{
			for(int i = 1; i < Constants.WIDTH_MINUS_1; i++)
			{
				for(int j = 1; j < Constants.WIDTH_MINUS_1; j++)
				{
					char c = GetOuputChar(i, j);
					map[x+i-1][y+j-1] = c;
				}
			}
		}

		public void Print()
		{
			for (int j = 0; j < Constants.WIDTH; j++)
			{
				for (int i = 0; i < Constants.WIDTH; i++)
				{
					char c = GetOuputChar(i, j);
					Console.Write(c);
				}
				Console.WriteLine();
			}
		}

	}

	class Day20Strategy : DayLineLoaderBasic
	{
		Dictionary<int, Tile> m_tiles = new Dictionary<int, Tile>();

		Tile m_curr = null;
		public override void ParseInputLine(string line, int lineNum)
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


		string r1 = "                  # ";
		string r2 = "#    ##    ##    ###";
		string r3 = " #  #  #  #  #  #   ";

		List<Vec2> seaMonsterParts = new List<Vec2>(new Vec2[]
		{
			new Vec2(18,0),
			new Vec2(0,1),
			new Vec2(5,1),
			new Vec2(6,1),
			new Vec2(11,1),
			new Vec2(12,1),
			new Vec2(17,1),
			new Vec2(18,1),
			new Vec2(19,1),
			new Vec2(1,2),
			new Vec2(4,2),
			new Vec2(7,2),
			new Vec2(10,2),
			new Vec2(13,2),
			new Vec2(16,2)
		});

		int SEA_MONSTER_HEIGHT = 3;
		int SEA_MONSTER_WIDTH = 20;
		public int NumSeaMonsters(ref char[][] map, int width, int height)
		{
			int sum = 0;
			for(int row = 0; row < height-SEA_MONSTER_HEIGHT; row++)
			{
				for(int col = 0; col < width - SEA_MONSTER_WIDTH; col++)
				{
					if(IsSeaMonster(map, row, col))
					{
						MarkSeaMonster(ref map, row, col);
						sum++;
					}
				}
			}

			return sum;
		}

		public void MarkSeaMonster(ref char[][] map, int y, int x)
		{
			foreach(var p in seaMonsterParts)
			{
				map[x + p.X][y + p.Y] = 'O';
			}
		}

		public bool IsSeaMonster(char[][]map, int y, int x)
		{
			return seaMonsterParts.All(offset =>
			{
				return map[x + offset.X][y + offset.Y] == '#';
			});
		}

		public override string Part2()
		{
			//var t = Transform.Rotate(90);
			//var f = Transform.FlipX();

			//Edge e = Edge.Top;
			//string s = "0123456789";
			//Console.WriteLine($"Started at {e}, {s}");

			//(e, s) = t.Apply(e, s);
			//Console.WriteLine($"Rotated to {e}, {s}");

			//(e, s) = f.Apply(e, s);
			//Console.WriteLine($"Flipped to {e}, {s}");

			//int x1 = 1;
			//int y = 4;
			//Console.WriteLine($"Started at ({x1}, {y})");

			//(x1, y) = t.ApplyPoint(x1, y);
			//Console.WriteLine($"Rotated to ({x1}, {y})");

			//(x1, y) = f.ApplyPoint(x1, y);
			//Console.WriteLine($"Flipped to ({x1}, {y})");

			int SIDE = (int)Math.Sqrt(m_tiles.Count);
			const int TILE_WIDTH = 8;
			ArrayUtil.Build2DArray(ref m_map, SIDE * TILE_WIDTH, SIDE * TILE_WIDTH, ' ');

			var corners = m_tiles.Values.Where(x => x.EdgeMatches.Count == 2);
			Tile start = corners.First();
			var edges = start.EdgeMatches.Keys;
			var edge1 = edges.ElementAt(0);
			var edge2 = edges.ElementAt(1);
			start.Position(edge1, Edge.Bottom, edge2, Edge.Right);

			Console.WriteLine($"Positioning Tile {start.Id} with {edge1} down and {edge2} right:");
			start.Print();

			Dictionary<Vec2, Tile> positions = new Dictionary<Vec2, Tile>();
			positions[new Vec2(0, 0)] = start;

			// Finish row 0
			for(int col = 1; col < SIDE; col++)
			{
				Tile left = positions[new Vec2(0, col - 1)];
				(string rightEdgeStart, string rightEdgeNow) = left.GetCurrentSide(Edge.Right);

				Tile me = m_tiles[left.EdgeMatches[rightEdgeStart]];
				string edge = me.EdgeMatches.Where(k => k.Value == left.Id).Select(k => k.Key).First();
				me.PositionMatching(edge, Edge.Left, rightEdgeNow);

				positions[new Vec2(0, col)] = me;
				Console.WriteLine($"Positioning Tile {me.Id} with {edge} left:");
				me.Print();
			}

			for (int row = 1; row < SIDE; row++)
			{
				for(int col = 0; col < SIDE; col++)
				{
					Tile up = positions[new Vec2(row - 1, col)];
					(string botEdgeStart, string botEdgeNow) = up.GetCurrentSide(Edge.Bottom);

					Tile me = m_tiles[up.EdgeMatches[botEdgeStart]];
					string edge = me.EdgeMatches.Where(k => k.Value == up.Id).Select(k => k.Key).First();
					me.PositionMatching(edge, Edge.Top, botEdgeNow);

					positions[new Vec2(row, col)] = me;
					Console.WriteLine($"Positioning Tile {me.Id} with {edge} up:");
					me.Print();
				}
			}

			for(int row = 0; row < SIDE; row++)
			{
				for(int col = 0; col < SIDE; col++)
				{
					Tile t = positions[new Vec2(row, col)];

					t.CopyInto(ref m_map, col * TILE_WIDTH, row * TILE_WIDTH);
				}
			}
			Console.WriteLine("Built map:");
			ArrayUtil.PrintArray(m_map, SIDE * TILE_WIDTH, SIDE * TILE_WIDTH);

			int monsters = NumSeaMonsters(ref m_map, SIDE * TILE_WIDTH, SIDE * TILE_WIDTH);

			Console.WriteLine($"Found {monsters}!");
			ArrayUtil.PrintArray(m_map, SIDE * TILE_WIDTH, SIDE * TILE_WIDTH);

			return m_map.Sum(row => row.Sum(c => c == '#' ? 1 : 0)).ToString();

		}
	}
}
