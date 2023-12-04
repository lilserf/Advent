using System;
using System.Linq;

namespace Advent.Year2019
{
	public abstract class DayIntCodeStrategy : DayInputFileStrategy
	{
		private long[] m_input;
		public DayIntCodeStrategy(string inputFile)
			: base(inputFile)
		{ }

		protected IntCode CreateProgram(Func<bool> inputReady, Func<long> input, Action<long> output) => new IntCode(m_input, inputReady, input, output);

		protected sealed override void ParseInputLine(string line, int lineNum)
		{
			m_input = line.Split(',').Select(long.Parse).ToArray();
		}
	}
}