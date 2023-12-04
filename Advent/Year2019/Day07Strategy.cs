using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2019
{
	class Day07Strategy : DayInputFileStrategy
	{
		long[] m_program;


		public Day07Strategy(string file) : base(file)
		{
		}

		public override string Part1()
		{
			Dictionary<string, long> inputs = new Dictionary<string, long>();

			inputs.Add("", 0);

			for(int amp=0; amp < 5; amp++)
			{
				Dictionary<string, long> newInputs = new Dictionary<string, long>();

				while (inputs.Count > 0)
				{
					var nextInput = inputs.First();
					inputs.Remove(nextInput.Key);

					for (int i = 0; i < 5; i++)
					{
						if (!nextInput.Key.Contains(i.ToString()))
						{
							List<long> output = new List<long>();
							List<long> progInputs = new long[] { i, nextInput.Value }.ToList();

							IntCode program = new IntCode(m_program, () => true, () =>
							{
								var x = progInputs.First();
								progInputs.RemoveAt(0);
								return x;
							},
							(x) => output.Add(x));

							program.Run();
							newInputs.Add(nextInput.Key + i.ToString(), output.Last());
						}
					}
				}

				//Console.WriteLine($"Amp {amp}:");
				foreach(var k in newInputs)
				{
					//Console.WriteLine($"  {k.Key} : {k.Value}");
					inputs.Add(k.Key, k.Value);
				}
			}

			return inputs.Values.Max().ToString();
		}

		public override string Part2()
		{
			var inputs = Permuter.Permute(new long[] { 5, 6, 7, 8, 9 });

			//foreach(var x in inputs)
			//	Console.WriteLine(x.Aggregate("", (s, i) => s += i.ToString()));

			long highestSignal = 0;

			foreach (var phases in inputs)
			{
				IntCode[] amps = new IntCode[5];
				Queue<long>[] outputs = new Queue<long>[5];

				for (int i = 0; i < 5; i++)
				{
					outputs[i] = new Queue<long>();
					outputs[i].Enqueue(phases.ElementAt(i));
				}

				// Starting input for first amp
				outputs[4].Enqueue(0);

				amps[0] = InitAmp(outputs, 4, 1);
				amps[1] = InitAmp(outputs, 0, 2);
				amps[2] = InitAmp(outputs, 1, 3);
				amps[3] = InitAmp(outputs, 2, 4);
				amps[4] = InitAmp(outputs, 3, 0);

				while (amps.Any(x => x.State != IntCode.ExecutionState.Ended))
				{
					Array.ForEach(amps, x => x.Step());
				}

				long finalOutput = outputs[4].Dequeue();
				if(finalOutput > highestSignal)
				{
					highestSignal = finalOutput;
				}
			}

			return highestSignal.ToString();
		}

		private IntCode InitAmp(Queue<long>[] queues, int inputFromIndex, int outputToIndex)
		{
			return new IntCode(m_program,
				() => queues[inputFromIndex].Count > 0,
				() => queues[inputFromIndex].Dequeue(),
				(x) => queues[outputToIndex].Enqueue(x));
		}

		protected override void ParseInputLine(string line, int lineNum)
		{
			m_program = line.Split(',').Select(s => long.Parse(s)).ToArray();
		}
	}
}
