using System.IO;

namespace Advent
{
	// Strat to read a file with some sort of character map, like a 2D grid of characters
	// Override ReadMapCharacter to do what you want with each character read
    public abstract class DayMapInputFileStrategy : IDayStrategy
    {
        private readonly string m_inputFile;

        public DayMapInputFileStrategy(string inputFile)
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

        protected abstract void ReadMapCharacter(Vec2 position, char c);

		protected virtual void ReadMapEnd() { }

        private void ParseInputStream(StreamReader sr)
        {
            int x = 0;
            int y = 0;
            while (!sr.EndOfStream)
            {
                char c = (char)sr.Read();
                switch (c)
                {
                    case '\n':
                        ++y;
                        x = -1;
                        break;

                    case '\r':
                        --x; // We don't want to increment this one
                        break;

                    default:
                        ReadMapCharacter(new Vec2(x, y), c);
                        break;
                }
                ++x;
            }

			ReadMapEnd();
        }
    }
}
