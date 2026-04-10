using System;
using UnityEngine;

namespace Barliesque.Utils
{

	[Serializable]
	public struct LinearLimits : IEquatable<LinearLimits>
	{
		public float Start;
		public float End;

		public LinearLimits(float start, float end)
		{
			Start = start;
			End = end;
		}

		/// <summary>
		/// Returns true if the specified value is within the range.
		/// </summary>
		public bool IsInside(float value) => (Start < End) ? (value >= Start && value <= End) : (value <= Start && value >= End);
		
		/// <summary>
		/// Clamps the specified value to the range, and returns the result.
		/// </summary>
		public float Clamp(float value) => (Start < End) ? Mathf.Clamp(value, Start, End) : Mathf.Clamp(value, End, Start);
		
		/// <summary>
		/// Wraps the specified value to the range, and returns the result.
		/// </summary>
		public float Wrap(float value) => ((value + Range - Start) % Range) + Start;
		
		/// <summary>
		/// A getter that returns the difference between the Start and the End of the range.
		/// </summary>
		public float Range => Mathf.Abs(End - Start);
		
		/// <summary>
		/// Linearly interpolates between the start and end of the range, by t.  The result is clamped to the range.
		/// </summary>
		public float Lerp(float t) => Mathf.Lerp(Start, End, t);
		
		/// <summary>
		/// Linearly interpolates between the start and end of the range, by t.  The result is not clamped to the range.
		/// </summary>
		public float LerpUnclamped(float t) => Mathf.LerpUnclamped(Start, End, t);
		
		/// <summary>
		/// Normalizes the specified value to the range, returning a value from 0.0 to 1.0
		/// </summary>
		public float InverseLerp(float value) => Mathf.Clamp01((value - Start) / (End - Start));
		
		/// <summary>
		/// Normalizes the specified value to the range, returning a value from 0.0 to 1.0 — the result is not clamped.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public float InverseLerpUnclamped(float value) => (value - Start) / (End - Start);
		
		/// <summary>
		/// A getter that returns the value at the center of the range.
		/// </summary>
		public float Center => (Start + End) * 0.5f;

		/// <summary>
		/// Remap a given value from this range to another.
		/// If the input value is at the center of this range, the output value will be at the center of the target range.
		/// </summary>
		/// <param name="value">The input value</param>
		/// <param name="targetRange">The range to remap to</param>
		/// <returns></returns>
		public float Remap(float value, LinearLimits targetRange) => targetRange.Lerp(InverseLerp(value));
		
		/// <summary>
		/// Remap a given value from this range to another, with no limits.
		/// So if the input value is 15% beyond the end of this range, the result will be 15% beyond the end of the target range.
		/// </summary>
		/// <param name="value">The input value</param>
		/// <param name="targetRange">The range to remap to</param>
		/// <returns></returns>
		public float RemapUnclamped(float value, LinearLimits targetRange) => targetRange.LerpUnclamped(InverseLerpUnclamped(value));
		
		/// <summary> Smoothly clamps within the range, avoiding hard stops at the range limits. </summary>
		/// <param name="value"> The original value to be clamped. </param>
		public float SoftClamp(float value) => Mathf.SmoothStep(Start, End, (value - Start) / (End - Start));

		/// <summary>
		/// Get a random value within the specified limits.
		/// </summary>
		public float Random() => Mathf.Lerp(Start, End, UnityEngine.Random.value);

		override public string ToString() => $"[LinearLimits: Start={Start} End={End}]";

		/// <summary>
		/// Returns true if the specified LinearLimits specifies the exact same range.
		/// </summary>
		public bool Equals(LinearLimits other)
		{
			return Start.Equals(other.Start) && End.Equals(other.End);
		}

		/// <summary>
		/// Returns true if the specified LinearLimits specifies the exact same range.
		/// </summary>
		override public bool Equals(object obj)
		{
			return obj is LinearLimits other && Start.Equals(other.Start) && End.Equals(other.End);
		}

		/// <summary>
		/// Creates a hash code integer from the start and end of the range.
		/// Two instances of LinearLimits defining the exact same range, would thus return the same hash code.
		/// </summary>
		override public int GetHashCode()
		{
			return HashCode.Combine(Start, End);
		}

		static public LinearLimits operator *(LinearLimits limits, float multiplier)
		{
			return new LinearLimits(limits.Start * multiplier, limits.End * multiplier);
		}

		static public LinearLimits operator /(LinearLimits limits, float divisor)
		{
			return new LinearLimits(limits.Start / divisor, limits.End / divisor);
		}

		static public LinearLimits operator +(LinearLimits limits, float offset)
		{
			return new LinearLimits(limits.Start + offset, limits.End + offset);
		}

		static public LinearLimits operator -(LinearLimits limits, float offset)
		{
			return new LinearLimits(limits.Start - offset, limits.End - offset);
		}
	}
}