using System.IO;

namespace Advent
{
    public class DayRunnerLineLoaderStrategy<T> : DayInputFileStrategy
        where T : IDayPartRunner, IDayLineLoader
    {
        private readonly T m_loaderRunner;

        public DayRunnerLineLoaderStrategy(string inputFile, T loaderRunner)
            : base(inputFile)
        {
            m_loaderRunner = loaderRunner;
        }

        protected override void ParseInputLinesBegin(StreamReader sr) => m_loaderRunner.ParseInputLinesBegin(sr);
        protected override void ParseInputLine(string line) => m_loaderRunner.ParseInputLine(line);
        protected override void ParseInputLinesEnd(StreamReader sr) => m_loaderRunner.ParseInputLinesEnd(sr);

        public override string Part1() => m_loaderRunner.Part1();
        public override string Part2() => m_loaderRunner.Part2();
    }
}
