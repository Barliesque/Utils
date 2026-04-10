using System;
using UnityEngine;

namespace Barliesque.Utils
{

	[Serializable]
	public struct IntRange
	{
		public int Start;
		public int End;

		public IntRange(int start, int end)
		{
			Start = start;
			End = end;
		}

		/// <summary>
		/// Is the specified value within this range?  (Range is inclusive)
		/// </summary>
		public bool Contains(int value) => (Start < End) ? (value >= Start && value <= End) : (value <= Start && value >= End);
		
		/// <summary>
		/// Clamp the specified value to this range  (Range is inclusive)
		/// </summary>
		public int Clamp(int value) => (Start < End) ? Mathf.Clamp(value, Start, End) : Mathf.Clamp(value, End, Start);

		/// <summary>
		/// Wrap the specified value to this range  (Range is inclusive)
		/// </summary>
		public int Wrap(int value) => value < Start ? (End + 1 - (Start - value) % (End + 1 - Start)) : (Start + (value - Start) % (End + 1 - Start));
			
		/// <summary>
		/// How many values are encompassed by this range?  If Start and End are equal, this will be 1.
		/// </summary>
		public int Size => Mathf.Abs(End - Start) + 1;
		
		public int Lerp(float t) => Mathf.RoundToInt(Mathf.Lerp(Start, End, t));
		public int LerpUnclamped(float t) => Mathf.RoundToInt(Mathf.LerpUnclamped(Start, End, t));
		public float InverseLerp(int value) => Mathf.InverseLerp(Start, End, value);

		/// <summary>
		/// Get a random value within this range  (Range is inclusive)
		/// </summary>
		public int Random() => Mathf.Min(Mathf.FloorToInt(Mathf.Lerp(Start, End + 1, UnityEngine.Random.value)), End);

		override public string ToString() => $"[IntRange: Start={Start} End={End}]";

	}
}