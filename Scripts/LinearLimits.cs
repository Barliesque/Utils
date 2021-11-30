using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Barliesque.Utils
{

	[Serializable]
	public struct LinearLimits
	{
		[FormerlySerializedAs("Low")] public float Start;
		[FormerlySerializedAs("High")] public float End;
		[Obsolete("Use LinearLimits.Start instead.")] public float Low => Start;
		[Obsolete("Use LinearLimits.End instead.")] public float High => End;

		public LinearLimits(float start, float end)
		{
			Start = start;
			End = end;
		}

		public bool IsInside(float value) => (Start < End) ? (value >= Start && value <= End) : (value <= Start && value >= End);
		public float Clamp(float value) => (Start < End) ? Mathf.Clamp(value, Start, End) : Mathf.Clamp(value, End, Start);
		public float Range => End - Start;
		public float Lerp(float t) => Mathf.Lerp(Start, End, t);
		public float InverseLerp(float value) => Mathf.InverseLerp(Start, End, value);
		
		/// <summary>
		/// Get a random value within the specified limits.
		/// </summary>
		public float Random() => Mathf.Lerp(Start, End, UnityEngine.Random.value);

		override public string ToString() => $"[LinearLimits: Low={Start} High={End}]";

	}
}