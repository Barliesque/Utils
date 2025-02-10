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
	
	/// <summary>
	/// Retrieves and removes the first item of the list.
	/// </summary>
	static public T PopFirst<T>(this IList<T> list)
	{
		if (list.Count == 0) return default;
		var first = list[0];
		list.RemoveAt(0);
		return first;
	}
	
	/// <summary>
	/// Retrieves and removes the first item of the list.  True is returned if successful.
	/// </summary>
	static public bool PopFirst<T>(this IList<T> list, out T first)
	{
		if (list.Count == 0)
		{
			first = default;
			return false;
		}
		first = list[0];
		list.RemoveAt(0);
		return true;
	}
	
	/// <summary>
	/// Retrieves and removes the last item of the list.
	/// </summary>
	static public T PopLast<T>(this IList<T> list)
	{
		var index = list.Count - 1;
		if (index < 0) return default;
		var last = list[index];
		list.RemoveAt(index);
		return last;
	}
	
	/// <summary>
	/// Retrieves and removes the last item of the list.  True is returned if successful.
	/// </summary>
	static public bool PopLast<T>(this IList<T> list, out T last)
	{
		var index = list.Count - 1;
		if (index < 0)
		{
			last = default;
			return false;
		}
		last = list[index];
		list.RemoveAt(index);
		return true;
	}

}