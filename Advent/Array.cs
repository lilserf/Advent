using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent
{
	public static class ArrayUtil
	{
		public static void Build2DArray<T>(ref T[][] array, int width, int height, T value) where T : struct
		{
			array = new T[width][];
			for (int i = 0; i < width; i++)
			{
				array[i] = new T[height];
				for(int j=0; j < height; j++)
				{
					array[i][j] = value;
				}
			}
		}

		public static void Build2DArray<T>(ref T[][] array, int width, int height, Func<T> create) where T : class
		{
			array = new T[width][];
			for (int i = 0; i < width; i++)
			{
				array[i] = new T[height];
				for (int j = 0; j < height; j++)
				{
					array[i][j] = create();
				}
			}
		}
	}
}
