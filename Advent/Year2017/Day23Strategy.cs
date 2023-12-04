using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2017
{
	class Day23Strategy : DayLineLoaderBasic
	{
		List<string> m_data = new List<string>();
		public override void ParseInputLine(string line, int lineNum)
		{
			m_data.Add(line);
		}

		public override string Part1()
		{
			Duet d = new Duet(m_data, true, 0);

			Duet.State done = Duet.State.Running;
			do
			{
				done = d.Tick(null, null);
			}
			while (done != Duet.State.Done);

			return d.MulCount.ToString();
		}

		public override string Part2()
		{
			Duet d = new Duet(m_data, true, 0);
			d.SetRegValue('a', 1);

			Duet.State done = Duet.State.Running;
			do
			{
				done = d.Tick(null, null, false);
			}
			while (done != Duet.State.Done);

			return d.GetRegValue('h').ToString();
		}
	}
}
