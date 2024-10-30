using System;
using UnityEngine;

namespace Barliesque.Utils
{
	static public class int2Utils
	{
		/// <summary>
		/// Find the optimal dimension of a 2D array to fit a given number of elements.
		/// </summary>
		/// <param name="count">The number of elements to fit into a 2D array.</param>
		/// <param name="maxWidth">Maximum number of columns.</param>
		/// <param name="maxHeight">Maximum number of rows.</param>
		/// <returns>Returns an int2 with the optimal dimensions.</returns>
		/// <exception cref="Exception">An exception is thrown if the count exceeds the maximum possible elements.</exception>
		static public int2 SizeToFit(int count, int maxWidth = 16, int maxHeight = 16)
		{
			var c = Mathf.FloorToInt(Mathf.Sqrt(count));
			var r = Mathf.CeilToInt(count / (float)c);
			var w = count - c * r;
			if (w < 0) throw new Exception($"{count} count exceeds maximum possible within {maxWidth}x{maxHeight}");

			var best = new int2(c, r);
			for (int cc = c + 1; cc <= maxWidth; cc++)
			{
				r = Mathf.CeilToInt(count / (float)cc);
				var ww = count - cc * r;
				if (ww >= w) continue;
				w = ww;
				best = new int2(cc, r);
			}

			return best;
		}
	}
}