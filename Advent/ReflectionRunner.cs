using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent
{
	class Record
	{
		public int Year => m_year;
		private int m_year;
		public int Day => m_day;
		private int m_day;
		public Type StrategyType => m_strategyType;
		private Type m_strategyType;
		string InputFileName => m_inputFileName;
		private string m_inputFileName;

		IEnumerable<string> TestFiles => m_testFiles;
		private List<string> m_testFiles;

		public static Record CreateFromStrategyType(Type t)
		{
			var matches = Regex.Match(t.Namespace, @"Year(\d+)");
			if (!matches.Success)
			{
				return null;
			}
			int year = int.Parse(matches.Groups[1].Value);

			matches = Regex.Match(t.Name, @"Day(\d+)Strategy");
			if (!matches.Success)
			{
				return null;
			}
			int day = int.Parse(matches.Groups[1].Value);

			string inputFilePath = null;
			string filePath = $"inputs\\{year}";
			List<string> testFiles = new List<string>();
			
			for(int i = 0; i < 10; i++)
			{
				var check = Path.Combine(filePath, $"day{day}-test{i}.txt");
				if(File.Exists(check))
				{
					testFiles.Add(check);
				}
			}

			if (inputFilePath == null)
			{
				inputFilePath = Path.Combine(filePath, $"day{day}.txt");
			}

			Record r = new Record();
			r.m_year = year;
			r.m_day = day;
			r.m_strategyType = t;
			r.m_inputFileName = inputFilePath;
			r.m_testFiles = testFiles;
			return r;
		}

		public void Run()
		{
			foreach(var tf in m_testFiles)
			{
				Console.WriteLine($"################ TEST {tf} #################");
				Day test = new Day((IDayStrategy)Activator.CreateInstance(m_strategyType, tf));
				test.Initialize();
				Program.RunAndProfile(test);
				Console.ReadKey();
				Console.WriteLine("#############################################");
			}

			Day d = new Day((IDayStrategy)Activator.CreateInstance(m_strategyType, m_inputFileName));
			d.Initialize();
			Program.RunAndProfile(d);
			Console.ReadKey();
		}
	}

	class ReflectionRunner
	{
		List<Record> m_records;

		public ReflectionRunner()
		{
			m_records = new List<Record>();

			var target = typeof(IDayStrategy);
			var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => target.IsAssignableFrom(p));

			foreach(var t in types)
			{
				Record r = Record.CreateFromStrategyType(t);
				if(r != null)
					m_records.Add(r);
			}
		}

		public void RunLatest()
		{
			var latest = m_records.OrderBy(x => x.Day).OrderBy(x => x.Year).Last();
			latest.Run();
		}
	}
}
