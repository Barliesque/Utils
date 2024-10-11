using System;
using System.Collections.Generic;
using System.Linq;

static public class EnumerableExtensions
{
	/// <summary>
	/// Returns a new IList containing the same elements in random order. 
	/// </summary>
	/// <param name="sequence"></param>
	/// <param name="seed">Random number generator seed.</param>
	/// <typeparam name="T"></typeparam>
	/// <exception cref="ArgumentNullException"></exception>
	static public IList<T> Shuffle<T>(this IEnumerable<T> sequence, int seed)
	{
		return sequence.Shuffle(new Random(seed));
	}
    
	/// <summary>
	/// Returns a new IList containing the same elements in random order. 
	/// </summary>
	/// <param name="sequence"></param>
	/// <typeparam name="T"></typeparam>
	/// <exception cref="ArgumentNullException"></exception>
	static public IList<T> Shuffle<T>(this IEnumerable<T> sequence)
	{
		return sequence.Shuffle(new Random(DateTime.Now.Millisecond));
	}

	/// <summary>
	/// Returns a new IList containing the same elements in random order. 
	/// </summary>
	/// <param name="sequence"></param>
	/// <param name="randomNumberGenerator"></param>
	/// <typeparam name="T"></typeparam>
	/// <exception cref="ArgumentNullException"></exception>
	static public IList<T> Shuffle<T>(this IEnumerable<T> sequence, Random randomNumberGenerator)
	{
		if (sequence == null)
		{
			throw new ArgumentNullException(nameof(sequence));
		}

		if (randomNumberGenerator == null)
		{
			throw new ArgumentNullException(nameof(randomNumberGenerator));
		}

		var values = sequence.ToList();
		int currentlySelecting = values.Count;
		while (currentlySelecting > 1)
		{
			int selectedElement = randomNumberGenerator.Next(currentlySelecting);
			--currentlySelecting;
			if (currentlySelecting != selectedElement)
			{
				(values[currentlySelecting], values[selectedElement]) = (values[selectedElement], values[currentlySelecting]);
			}
		}

		return values;
	}
}