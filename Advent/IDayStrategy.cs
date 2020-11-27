namespace Advent
{
    public interface IDayStrategy
    {
        void Initialize();
        void Reset();
        string Part1();
        string Part2();
    }

    public abstract class DayStrategyBasic : IDayStrategy
    {
        public virtual void Initialize() { }
        public virtual void Reset() { }
        public abstract string Part1();
        public abstract string Part2();
    }
}
