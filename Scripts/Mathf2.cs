using System;
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

		
	}
}