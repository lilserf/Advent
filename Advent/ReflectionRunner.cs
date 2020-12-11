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

		public bool UseDayRunner => m_useDayRunner;
		private bool m_useDayRunner;

		private static Record CreateShared(Type t)
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

			for (int i = 0; i < 10; i++)
			{
				var check = Path.Combine(filePath, $"day{day}-test{i}.txt");
				if (File.Exists(check))
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

		public static Record CreateFromRunner(Type t)
		{
			var r = CreateShared(t);
			if (r != null)
			{
				r.m_useDayRunner = true;
			}
			return r;
		}

		public static Record CreateFromStrategyType(Type t)
		{
			return CreateShared(t);
		}


		private void CreateAndRun(string f)
		{
			IDay day = null;
			if (m_useDayRunner)
			{
				var instance = (DayLineLoaderBasic)Activator.CreateInstance(m_strategyType);
				day = DayRunner.Create(f, instance);
			}
			else
			{
				day = new Day((IDayStrategy)Activator.CreateInstance(m_strategyType, f));
			}
			day.Initialize();
			Program.RunAndProfile(day);
		}

		public void Run()
		{
			foreach(var tf in m_testFiles)
			{
				Console.WriteLine($"################ TEST {tf} #################");
				CreateAndRun(tf);
				Console.ReadKey();
				Console.WriteLine("#############################################");
			}

			CreateAndRun(m_inputFileName);
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

			target = typeof(DayLineLoaderBasic);
			types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => target.IsAssignableFrom(p));

			foreach(var t in types)
			{
				Record r = Record.CreateFromRunner(t);
				if (r != null)
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
