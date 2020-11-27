using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent
{
	class Permuter
	{
		public static IEnumerable<Tuple<T, T>> Pairs<T>(IEnumerable<T> original)
		{
			for(int i=0; i < original.Count(); i++)
			{
				for(int j = i+1; j < original.Count(); j++)
				{
					yield return new Tuple<T, T>(original.ElementAt(i), original.ElementAt(j));
				}
			}
		}

		public static IEnumerable<IReadOnlyCollection<T>> Permute<T>(IReadOnlyCollection<T> original)
		{
			List<List<T>> inputs = new List<List<T>>();
			inputs.Add(new List<T>());

			while (inputs.Any(x => x.Count < original.Count))
			{
				var nextList = inputs.First();
				inputs.RemoveAt(0);

				foreach(T item in original)
				{
					if (!nextList.Contains(item))
					{
						List<T> newList = new List<T>();

						newList.AddRange(nextList);
						newList.Add(item);

						inputs.Add(newList);
					}
				}
			}

			return inputs;
		}
	}
}
