using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Advent
{
	// Strat to read an input file and parse individual lines. Override the virtual protected functions to parse lines
    public abstract class DayInputFileStrategy : IDayStrategy
    {
        private readonly string m_inputFile;

        public DayInputFileStrategy(string inputFile)
        {
            m_inputFile = inputFile;
        }

        public void Initialize()
        {
            using (var sr = new StreamReader(m_inputFile))
            {
                ParseInputStream(sr);
            }
        }

        public virtual void Reset() { }
        public abstract string Part1();
        public abstract string Part2();

        public override string ToString()
        {
            string typsString = GetType().ToString();
            int dot = typsString.LastIndexOf('.', typsString.LastIndexOf('.') - 1);
            return typsString.Substring(dot + 1).TrimStart('_');
        }

        protected virtual void ParseInputStream(StreamReader sr)
        {
            string line;
            ParseInputLinesBegin(sr);
            while (null != (line = sr.ReadLine()))
            {
                ParseInputLine(line);
            }
            ParseInputLinesEnd(sr);
        }

        protected virtual void ParseInputLinesBegin(StreamReader sr) { }
        protected virtual void ParseInputLine(string line) { }
        protected virtual void ParseInputLinesEnd(StreamReader sr) { }

        protected static Match MatchLine(string line, Regex regex)
        {
            var match = regex.Match(line);
            if (!match.Success) throw new NotSupportedException($"Could not parse line \"{line}\"");
            return match;
        }
    }
}
