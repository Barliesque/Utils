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

		public bool IsInside(int value) => (Start < End) ? (value >= Start && value <= End) : (value <= Start && value >= End);
		public int Clamp(int value) => (Start < End) ? Mathf.Clamp(value, Start, End) : Mathf.Clamp(value, End, Start);
		public int Wrap(int value) => ((value + Range - Start) % Range) + Start;
		public int Range => End - Start;
		public int Lerp(float t) => Mathf.RoundToInt(Mathf.Lerp(Start, End, t));
		public int LerpUnclamped(float t) => Mathf.RoundToInt(Mathf.LerpUnclamped(Start, End, t));
		public float InverseLerp(int value) => Mathf.InverseLerp(Start, End, value);

		/// <summary>
		/// Get a random value within the specified limits.
		/// </summary>
		public int Random() => Mathf.RoundToInt(Mathf.Lerp(Start, End, UnityEngine.Random.value));

		override public string ToString() => $"[IntRange: Start={Start} End={End}]";

	}
}