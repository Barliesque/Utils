using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Barliesque.Utils
{
	static public class Mathf2
	{
		/// <summary>
		/// Returns the value that is furthest from zero.
		/// </summary>
		static public float AbsMax(float a, float b) => (Math.Abs(a) > Math.Abs(b) ? a : b);

		/// <summary>
		/// Returns the value that is furthest from zero.
		/// </summary>
		static public float AbsMax(params float[] values)
		{
			var len = values.Length;
			if (len == 0) return 0f;
			
			var num = values[0];
			var abs = Math.Abs(num);
			for (int i = 1; i < len; ++i)
			{
				if (!(Math.Abs(values[i]) > abs)) continue;
				
				num = values[i];
				abs = Math.Abs(num);
			}

			return num;
		}


		/// <summary>
		/// Returns the value that is closest to zero.
		/// </summary>
		static public float AbsMin(float a, float b) => (Math.Abs(a) < Math.Abs(b) ? a : b);

		/// <summary>
		/// Returns the value that is closest to zero.
		/// </summary>
		static public float AbsMin(params float[] values)
		{
			int len = values.Length;
			if (len == 0) return 0f;
			
			var num = values[0];
			var abs = Math.Abs(num);
			for (int i = 1; i < len; ++i)
			{
				if (!(Math.Abs(values[i]) < abs)) continue;
				
				num = values[i];
				abs = Math.Abs(num);
			}

			return num;
		}

		/// <summary>
		/// Returns the fractional portion of a floating point value, with the sign of the original value.
		/// </summary>
		static public float Fractional(float value)
		{
			if (value >= 0f)
				return (float) (value - Math.Floor(value));
			else
				return (float) (value - Math.Ceiling(value));
		}


		static public float InverseLerpUnclamped(float start, float end, float value)
		{
			return (value - start) / (end - start);
		}

		static public Vector3 LerpEulerAngles(Vector3 a, Vector3 b, float t)
		{
			return new Vector3(Mathf.LerpAngle(a.x, b.x, t), Mathf.LerpAngle(a.y, b.y, t), Mathf.LerpAngle(a.z, b.z, t));
		}
		
		/// <summary>
		/// Remap a value from one range to another
		/// </summary>
		/// <param name="value">Value to convert</param>
		/// <param name="originalStart">Start of original range</param>
		/// <param name="originalEnd">End of original range</param>
		/// <param name="newStart">Start of output range</param>
		/// <param name="newEnd">End of output range</param>
		static public float RemapUnclamped(
			float value,
			float originalStart, float originalEnd,
			float newStart, float newEnd)
		{
			float t = (value - originalStart) / (originalEnd - originalStart);
			return newStart + (newEnd - newStart) * t;
		}
		
		/// <summary>
		/// Remap a value from one range to another
		/// </summary>
		/// <param name="value">Value to convert</param>
		/// <param name="originalStart">Start of original range</param>
		/// <param name="originalEnd">End of original range</param>
		/// <param name="newStart">Start of output range</param>
		/// <param name="newEnd">End of output range</param>
		static public float Remap(
			float value,
			float originalStart, float originalEnd,
			float newStart, float newEnd)
		{
			var t = Mathf.Clamp01((value - originalStart) / (originalEnd - originalStart));
			return newStart + (newEnd - newStart) * t;
		}

		
		/// <summary>
		/// Calculate the distance to a line segment, squared.
		/// </summary>
		/// <param name="pos">Position to measure from</param>
		/// <param name="segA">Point A of the segment</param>
		/// <param name="segB">Point B of the segment</param>
		/// <param name="nearest">The nearest point on the segment</param>
		/// <returns>Returns the distance (squared) to the nearest point on the segment.</returns>
		static public float DistToSegSqr(Vector3 pos, Vector3 segA, Vector3 segB, out Vector3 nearest)
		{
			var delta = segB - segA;
			var segLenSqr = delta.sqrMagnitude;
			var distA = pos - segA;

			var t = Mathf.Clamp01((distA.x * delta.x + distA.y * delta.y + distA.z * delta.z) / segLenSqr);
			var seg = new Vector3(segA.x + t * delta.x, segA.y + t * delta.y, segA.z + t * delta.z);

			nearest = seg;
			return (pos - seg).sqrMagnitude;
		}

		
		/// <summary>
		/// Calculate the distance to a line segment.
		/// </summary>
		/// <param name="pos">Position to measure from</param>
		/// <param name="segA">Point A of the segment</param>
		/// <param name="segB">Point B of the segment</param>
		/// <param name="nearest">The nearest point on the segment</param>
		/// <returns>Returns the distance (squared) to the nearest point on the segment.</returns>
		static public float DistToSeg(Vector3 pos, Vector3 segA, Vector3 segB, out Vector3 nearest)
		{
			var delta = segB - segA;
			var segLenSqr = delta.sqrMagnitude;
			var distA = pos - segA;

			var t = Mathf.Clamp01((distA.x * delta.x + distA.y * delta.y + distA.z * delta.z) / segLenSqr);
			var seg = new Vector3(segA.x + t * delta.x, segA.y + t * delta.y, segA.z + t * delta.z);

			nearest = seg;
			return (pos - seg).magnitude;
		}

		
		/// <summary>
		/// Calculate the distance to a line segment, squared.
		/// </summary>
		/// <param name="pos">Position to measure from</param>
		/// <param name="segA">Point A of the segment</param>
		/// <param name="segB">Point B of the segment</param>
		/// <returns>Returns the distance (squared) to the nearest point on the segment.</returns>
		static public float DistToSegSqr(Vector3 pos, Vector3 segA, Vector3 segB)
		{
			var delta = segB - segA;
			var segLenSqr = delta.sqrMagnitude;
			var distA = pos - segA;

			var t = Mathf.Clamp01((distA.x * delta.x + distA.y * delta.y + distA.z * delta.z) / segLenSqr);
			var seg = new Vector3(segA.x + t * delta.x, segA.y + t * delta.y, segA.z + t * delta.z);

			return (pos - seg).sqrMagnitude;
		}

		
		/// <summary>
		/// Calculate the distance to a line segment.
		/// </summary>
		/// <param name="pos">Position to measure from</param>
		/// <param name="segA">Point A of the segment</param>
		/// <param name="segB">Point B of the segment</param>
		static public float DistToSeg(Vector3 pos, Vector3 segA, Vector3 segB)
		{
			var delta = segB - segA;
			var segLenSqr = delta.sqrMagnitude;
			var distA = pos - segA;

			var t = Mathf.Clamp01((distA.x * delta.x + distA.y * delta.y + distA.z * delta.z) / segLenSqr);
			var seg = new Vector3(segA.x + t * delta.x, segA.y + t * delta.y, segA.z + t * delta.z);

			return (pos - seg).magnitude;
		}

		/// <summary> Calculates a smooth union between two values, rather than the hard limit of a traditional min() function. </summary>
		/// <param name="a">First value</param>
		/// <param name="b">Second value</param>
		/// <param name="smoothing">0 = no smoothing, 1 = fully smoothed</param>
		/// <seealso cref="https://www.shadertoy.com/view/Ml3Gz8"/>
		static public float SmoothMin(float a, float b, float smoothing = 1f)
		{
			var h = Mathf.Clamp01(0.5f + 0.5f * (a - b) / smoothing);
			return Mathf.Lerp(a, b, h) - smoothing * h * (1f - h);
		}
		
		/// <summary> Calculates a smooth union between two values, rather than the hard limit of a traditional max() function. </summary>
		/// <param name="a">First value</param>
		/// <param name="b">Second value</param>
		/// <param name="smoothing">0 = no smoothing, 1 = fully smoothed</param>
		/// <seealso cref="https://www.shadertoy.com/view/Ml3Gz8"/>
		static public float SmoothMax(float a, float b, float smoothing = 1f)
		{
			var h = Mathf.Clamp01(0.5f + 0.5f * (a - b) / -smoothing);
			return Mathf.Lerp(a, b, h) + smoothing * h * (1f - h);
		}

		/// <summary> Calculates a smooth union between two values, rather than the hard limit of a traditional min() function. </summary>
		/// <param name="a">First value</param>
		/// <param name="b">Second value</param>
		/// <param name="smoothing">0 = no smoothing, 1 = fully smoothed</param>
		/// <seealso cref="https://www.shadertoy.com/view/Ml3Gz8"/>
		static public Vector2 SmoothMin(Vector2 a, Vector2 b, float smoothing = 1f)
		{
			return new Vector2(SmoothMin(a.x, b.x, smoothing), SmoothMin(a.y, b.y, smoothing));
		}

		/// <summary> Calculates a smooth union between two values, rather than the hard limit of a traditional max() function. </summary>
		/// <param name="a">First value</param>
		/// <param name="b">Second value</param>
		/// <param name="smoothing">0 = no smoothing, 1 = fully smoothed</param>
		/// <seealso cref="https://www.shadertoy.com/view/Ml3Gz8"/>
		static public Vector2 SmoothMax(Vector2 a, Vector2 b, float smoothing = 1f)
		{
			return new Vector2(SmoothMax(a.x, b.x, smoothing), SmoothMax(a.y, b.y, smoothing));
		}

		/// <summary> Calculates a smooth union between two values, rather than the hard limit of a traditional min() function. </summary>
		/// <param name="a">First value</param>
		/// <param name="b">Second value</param>
		/// <param name="smoothing">0 = no smoothing, 1 = fully smoothed</param>
		/// <seealso cref="https://www.shadertoy.com/view/Ml3Gz8"/>
		static public Vector3 SmoothMin(Vector3 a, Vector3 b, float smoothing = 1f)
		{
			return new Vector3(SmoothMin(a.x, b.x, smoothing), SmoothMin(a.y, b.y, smoothing), SmoothMin(a.z, b.z, smoothing));
		}

		/// <summary> Calculates a smooth union between two values, rather than the hard limit of a traditional max() function. </summary>
		/// <param name="a">First value</param>
		/// <param name="b">Second value</param>
		/// <param name="smoothing">0 = no smoothing, 1 = fully smoothed</param>
		/// <seealso cref="https://www.shadertoy.com/view/Ml3Gz8"/>
		static public Vector3 SmoothMax(Vector3 a, Vector3 b, float smoothing = 1f)
		{
			return new Vector3(SmoothMax(a.x, b.x, smoothing), SmoothMax(a.y, b.y, smoothing), SmoothMax(a.z, b.z, smoothing));
		}

		/// <summary>
		/// Exponential interpolation between two values, using t.
		/// </summary>
		/// <param name="fromValue">The value returned when t is 0.</param>
		/// <param name="toValue">The value returned when t is 1.</param>
		/// <param name="t">A value from 0 to 1, though this is not clamped.</param>
		/// <returns></returns>
		static public float Eerp(float fromValue, float toValue, float t)
		{
			return fromValue * Mathf.Pow(toValue / fromValue, t);
			//return Mathf.Pow(fromValue, 1f - t) * Mathf.Pow(toValue, t);
		}

		/// <summary>
		/// Interpolate a value towards a target, in a manner that's consistent regardless of framerate.
		/// </summary>
		/// <param name="current">The current value.</param>
		/// <param name="target">The target value.</param>
		/// <param name="halfLife">Rate of change;  the time it should take to reach half-way to the target.</param>
		/// <param name="deltaTime">Elapsed time since the last update.</param>
		/// <seealso cref="https://theorangeduck.com/page/spring-roll-call"/>
		static public float Damper(float current, float target, float halfLife, float deltaTime)
		{
			return Mathf.Lerp(current, target, 1f - FastNegExp((0.69314718056f * deltaTime) / (halfLife + 1e-5f)));
		}
		
		/// <summary>
		/// Interpolate a value towards a target, in a manner that's consistent regardless of framerate.
		/// </summary>
		/// <param name="current">The current value.</param>
		/// <param name="target">The target value.</param>
		/// <param name="halfLife">Rate of change;  the time it should take to reach half-way to the target.</param>
		/// <seealso cref="https://theorangeduck.com/page/spring-roll-call"/>
		static public float Damper(float current, float target, float halfLife)
		{
			return Mathf.Lerp(current, target, 1f - FastNegExp((0.69314718056f * Time.deltaTime) / (halfLife + 1e-5f)));
		}
		
		/// <summary>
		/// Interpolate a value towards a target, in a manner that's consistent regardless of framerate.
		/// </summary>
		/// <param name="current">The current value.</param>
		/// <param name="target">The target value.</param>
		/// <param name="halfLife">Rate of change;  the time it should take to reach half-way to the target.</param>
		/// <param name="deltaTime">Elapsed time since the last update.</param>
		/// <seealso cref="https://theorangeduck.com/page/spring-roll-call"/>
		static public Vector2 Damper(Vector2 current, Vector2 target, float halfLife, float deltaTime)
		{
			return Vector2.Lerp(current, target, 1f - FastNegExp((0.69314718056f * deltaTime) / (halfLife + 1e-5f)));
		}
		
		/// <summary>
		/// Interpolate a value towards a target, in a manner that's consistent regardless of framerate.
		/// </summary>
		/// <param name="current">The current value.</param>
		/// <param name="target">The target value.</param>
		/// <param name="halfLife">Rate of change;  the time it should take to reach half-way to the target.</param>
		/// <seealso cref="https://theorangeduck.com/page/spring-roll-call"/>
		static public Vector2 Damper(Vector2 current, Vector2 target, float halfLife)
		{
			return Vector2.Lerp(current, target, 1f - FastNegExp((0.69314718056f * Time.deltaTime) / (halfLife + 1e-5f)));
		}
		
		/// <summary>
		/// Interpolate a value towards a target, in a manner that's consistent regardless of framerate.
		/// </summary>
		/// <param name="current">The current value.</param>
		/// <param name="target">The target value.</param>
		/// <param name="halfLife">Rate of change;  the time it should take to reach half-way to the target.</param>
		/// <param name="deltaTime">Elapsed time since the last update.</param>
		/// <seealso cref="https://theorangeduck.com/page/spring-roll-call"/>
		static public Vector3 Damper(Vector3 current, Vector3 target, float halfLife, float deltaTime)
		{
			return Vector3.Lerp(current, target, 1f - FastNegExp((0.69314718056f * deltaTime) / (halfLife + 1e-5f)));
		}
		
		/// <summary>
		/// Interpolate a value towards a target, in a manner that's consistent regardless of framerate.
		/// </summary>
		/// <param name="current">The current value.</param>
		/// <param name="target">The target value.</param>
		/// <param name="halfLife">Rate of change;  the time it should take to reach half-way to the target.</param>
		/// <seealso cref="https://theorangeduck.com/page/spring-roll-call"/>
		static public Vector3 Damper(Vector3 current, Vector3 target, float halfLife)
		{
			return Vector3.Lerp(current, target, 1f - FastNegExp((0.69314718056f * Time.deltaTime) / (halfLife + 1e-5f)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static private float FastNegExp(float x) => 1f / (1f + x + 0.48f * x * x + 0.235f * x * x * x);
		
	}
}