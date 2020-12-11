namespace Advent
{
    public static class DayRunner
    {
        public static IDay Create<T>(string fileName, T runner)
            where T : IDayPartRunner, IDayLineLoader
        {
            return new Day(new DayRunnerLineLoaderStrategy<T>(fileName, runner));
        }
    }
}
