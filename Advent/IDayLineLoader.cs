using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Advent
{
    public interface IDayLineLoader
    {
        void ParseInputLinesBegin(StreamReader sr);
        void ParseInputLine(string line, int lineNum);
        void ParseInputLinesEnd(StreamReader sr);
    }

    public abstract class DayLineLoaderBasic : IDayLineLoader, IDayPartRunner
    {
        public abstract void ParseInputLine(string line, int lineNum);
        public virtual void ParseInputLinesBegin(StreamReader sr) {}
        public virtual void ParseInputLinesEnd(StreamReader sr) { }

        protected static Match MatchLine(string line, Regex regex)
        {
            var match = regex.Match(line);
            if (!match.Success) throw new NotSupportedException($"Could not parse line \"{line}\"");
            return match;
        }

        public abstract string Part1();
        public abstract string Part2();
    }
}
