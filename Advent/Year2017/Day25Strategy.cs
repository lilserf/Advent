using Advent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent.Year2017
{

	class State
	{
		public int[] Write { get; set; }
		public int[] Move { get; set; }
		public char[] NextState { get; set; }

		private int m_parsingVal = -1;
		public char Name { get; set; }
		public State(char name)
		{
			Name = name;
			Write = new int[2];
			Move = new int[2];
			NextState = new char[2];
		}

		public void NextVal()
		{
			m_parsingVal++;
		}

		public void NextInstr(string line)
		{
			var matches = Regex.Match(line, @"Write the value (\d)");
			if(matches.Success)
			{
				Write[m_parsingVal] = int.Parse(matches.Groups[1].Value);
				return;
			}

			matches = Regex.Match(line, @"Move one slot to the (\w+)");
			if(matches.Success)
			{
				Move[m_parsingVal] = (matches.Groups[1].Value == "right" ? 1 : -1);
				return;
			}

			matches = Regex.Match(line, @"Continue with state (\w)");
			if(matches.Success)
			{
				NextState[m_parsingVal] = (matches.Groups[1].Value[0]);
				return;
			}

			throw new Exception("Bad parse");
		}

		public (int, char) Process(Dictionary<int, int> tape, int currPos)
		{
			if(!tape.ContainsKey(currPos))
			{
				tape[currPos] = 0;
			}
			int val = tape[currPos];

			tape[currPos] = Write[val];
			currPos += Move[val];
			return (currPos, NextState[val]);
		}
	}

	class Day25Strategy : DayLineLoaderBasic
	{
		State m_curr;

		Dictionary<char, State> m_states = new Dictionary<char, State>();

		public override void ParseInputLine(string line, int lineNum)
		{
			if(string.IsNullOrWhiteSpace(line))
			{
				m_states.Add(m_curr.Name, m_curr);
			}
			else if(line.StartsWith("In state"))
			{
				var stateName = line[9];
				m_curr = new State(stateName);
			}
			else if(line.StartsWith("  If"))
			{
				m_curr.NextVal();
			}
			else if(line.StartsWith("    -"))
			{
				m_curr.NextInstr(line);
			}

		}

		public override void ParseInputLinesEnd(StreamReader sr)
		{
			m_states.Add(m_curr.Name, m_curr);
		}

		// Begin in state A.
		//Perform a diagnostic checksum after 12173597 steps.

		Dictionary<int, int> m_tape = new Dictionary<int, int>();
		int m_currPos = 0;

		public override string Part1()
		{
			char newState = 'A';

			for (long i = 0; i < 12173597; i++)
			{
				State curr = m_states[newState];
				(m_currPos, newState) = curr.Process(m_tape, m_currPos);
			}

			return m_tape.Values.Sum().ToString();
		}

		public override string Part2()
		{
			return "";
		}
	}
}
