using System;
using System.Collections.Generic;

static public class IListExtensions
{
	static private int _calls;

	/// <summary>
	/// Performs an in-place shuffle of all elements.
	/// </summary>
	/// <param name="list"></param>
	/// <param name="seed">Random number generator seed.</param>
	/// <typeparam name="T"></typeparam>
	/// <exception cref="ArgumentNullException"></exception>
	static public void Shuffle<T>(this IList<T> list, int seed)
	{
		UnityEngine.Random.InitState(seed);

		if (list == null) throw new ArgumentNullException(nameof(list));

		for (int i = 0, len = list.Count; i < len; i++)
		{
			var swap = UnityEngine.Random.Range(0, len);
			if (i == swap) continue;
			(list[i], list[swap]) = (list[swap], list[i]);
		}
	}

	/// <summary>
	/// Performs an in-place shuffle of all elements.  Random number seed is automatically initialized to be different for each call.
	/// </summary>
	/// <param name="list"></param>
	/// <typeparam name="T"></typeparam>
	/// <exception cref="ArgumentNullException"></exception>
	static public void Shuffle<T>(this IList<T> list)
	{
		list.Shuffle(DateTime.Now.Millisecond + _calls++);
	}

	/// <summary>
	/// Performs an in-place shuffle of all elements.
	/// </summary>
	/// <param name="list"></param>
	/// <param name="randomNumberGenerator">Instance of System.Random</param>
	/// <typeparam name="T"></typeparam>
	/// <exception cref="ArgumentNullException"></exception>
	static public void Shuffle<T>(this IList<T> list, Random randomNumberGenerator)
	{
		if (list == null) throw new ArgumentNullException(nameof(list));
		if (randomNumberGenerator == null) throw new ArgumentNullException(nameof(randomNumberGenerator));

		for (int i = 0, len = list.Count; i < len; i++)
		{
			var swap = randomNumberGenerator.Next(i);
			if (i == swap) continue;
			(list[i], list[swap]) = (list[swap], list[i]);
		}
	}
}