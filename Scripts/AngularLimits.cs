using System;
using UnityEngine;

namespace Barliesque.Utils
{

	[Serializable]
	public struct AngularLimits
	{
		/// <summary>
		/// Start point of the angular range.  Must be -180 to +180 degrees.
		/// </summary>
		[Tooltip("Start point of the angular range.  Must be -180 to +180 degrees.")]
		[SerializeField] private float _start;
		public float Start
		{
			get => _start;
			set => _start = FixAngle(value);
		}
		
		/// <summary>
		/// End point of the angular range.  Must be -180 to +180 degrees.
		/// </summary>
		[Tooltip("End point of the angular range.  Must be -180 to +180 degrees.")]
		[SerializeField] private float _end;
		public float End
		{
			get => _end;
			set => _end = FixAngle(value);
		}

		/// <summary>
		/// Is the angular range specified in counter-clockwise order?  Angular values increase travelling counter-clockwise.  This option can be used to invert the angular range.
		/// </summary>
		[Tooltip("Is the angular range specified in counter-clockwise order?  Angular values increase travelling counter-clockwise.  This option can be used to invert the angular range.")]
		public bool CCW;

		public AngularLimits(float start, float end, bool ccw = false)
		{
			_start = Mathf.Repeat(start + 180f, 360f) - 180f;
			_end = Mathf.Repeat(end + 180f, 360f) - 180f;
			CCW = ccw;
		}

		public bool RangeIsWrapped => CCW ? (_start > _end) : (_end > _start);
		
		public bool IsInside(float value, bool roundedToInt = false)
		{
			value = roundedToInt ?  Mathf.RoundToInt(FixAngle(value)) : FixAngle(value);
			var start = roundedToInt ? Mathf.RoundToInt(_start) : _start;
			var end = roundedToInt ? Mathf.RoundToInt(_end) : _end;
			if (RangeIsWrapped)
			{
				return CCW ? (value >= start || value <= end) : (value >= end || value <= start);
			}
			return CCW ? (value <= end && value >= start) : (value <= start && value >= end);
		}

		public float Clamp(float value)
		{
			value = FixAngle(value);
			if (RangeIsWrapped)
			{
				return (value < 0f) ? Mathf.Clamp(value, -180f, CCW ? _start : _end) : Mathf.Clamp(value, CCW ? _end : _start, 180f);
			}
			return CCW ? Mathf.Clamp(value, _start, _end) : Mathf.Clamp(value, _end, _start);
		}

		public float Range => RangeIsWrapped ? Mathf.DeltaAngle(_start, _end) : Mathf.Abs(_end - _start);  //(360f - Mathf.Abs(_end - _start))

		public float Center => _start + Mathf.DeltaAngle(_start, _end) * 0.5f;
		

		public float Normalize(float value)
		{
			var start = _start;
			var end = _end;
			value = FixAngle(value);
			if (RangeIsWrapped)
			{
				if (CCW) end += 360f;
				else start += 360f;
				if (value < 0f) value += 360f;
			}
			return Mathf.InverseLerp(start, end, value);
		}

		public float Lerp(float t)
		{
			var start = _start;
			var end = _end;
			if (RangeIsWrapped)
			{
				if (CCW) end += 360f;
				else start += 360f;
				return FixAngle(Mathf.Lerp(start, end, t));
			}
			return Mathf.Lerp(start, end, t);
		}
		
		public float InverseLerp(float angle)
		{
			angle = FixAngle(angle);
			if (RangeIsWrapped)
			{
				if (_start < _end)
				{
					return 1f - Mathf.Clamp01(angle <= _start 
						? Mathf.InverseLerp(_end - 360f, _start, angle) 
						: Mathf.InverseLerp(_end, _start + 360f, angle));
				}
				else
				{
					return Mathf.Clamp01(angle <= _end 
						? Mathf.InverseLerp(_start - 360f, _end, angle) 
						: Mathf.InverseLerp(_start, _end + 360f, angle));
				}
			}
			return Mathf.InverseLerp(_start, _end, angle);
		}

		public float InverseLerpUnclamped(float angle)
		{
			angle = FixAngle(angle);
			if (RangeIsWrapped)
			{
				if (_start < _end)
				{
					return 1f - (angle <= _start
						? Mathf2.InverseLerpUnclamped(_end - 360f, _start, angle)
						: Mathf2.InverseLerpUnclamped(_end, _start + 360f, angle));
				}
				else
				{
					return (angle <= _end
						? Mathf2.InverseLerpUnclamped(_start - 360f, _end, angle)
						: Mathf2.InverseLerpUnclamped(_start, _end + 360f, angle));
				}
			}
			return Mathf2.InverseLerpUnclamped(_start, _end, angle);
		}

		public AngularLimits Expand(float includeAngle)
		{
			if (IsInside(includeAngle)) return this;
			includeAngle = FixAngle(includeAngle);
			var result = this;
			var delta = Mathf.Abs(Start - includeAngle);
			var deltaStart = Mathf.Min(delta, 360f - delta);
			delta = Mathf.Abs(End - includeAngle);
			var deltaEnd = Mathf.Min(delta, 360f - delta);
			if (deltaStart < deltaEnd)
			{
				result.Start = FixAngle(result.Start + deltaStart * (Start > End ? 1 : -1) * (RangeIsWrapped ? -1 : 1));
			}
			else
			{
				result.End = FixAngle(result.End + deltaEnd * (End > Start ? 1 : -1) * (RangeIsWrapped ? -1 : 1));
			}
			return result;
		}
		

		/// <summary>
		/// Wrap an angle to the range -180 to 180
		/// </summary>
		static public float FixAngle(float angle) => Mathf.Repeat(angle + 180f, 360f) - 180f;
		
		/// <summary>
		/// Wrap angle values to the range -180 to 180
		/// </summary>
		static public Vector3 FixAngles(Vector3 angles) => new Vector3(FixAngle(angles.x), FixAngle(angles.y), FixAngle(angles.z));

		/// <summary>
		/// Get a random angle within the specified limits.
		/// </summary>
		public float Random() => Lerp(UnityEngine.Random.value);

		
		override public string ToString() => $"[AngularLimits: Start={Start}° End={End}° CCW={CCW}]";

		
		/// <summary>
		/// Returns a modified version of this range.  Either the start or the end is expanded so that a specified angle is at its center.
		/// </summary>
		/// <param name="center"></param>
		/// <returns></returns>
		public AngularLimits ExpandFromCenter(float center)
		{
			var start = _start;
			var end = _end;
			if (RangeIsWrapped)
			{
				if ((Mathf.Sign(center) > 0) == (Mathf.Sign(end) > 0))
				{
					start -= 360f * Mathf.Sign(start);
				}
				else
				{
					end -= 360f * Mathf.Sign(end);
				}
			}
			var range = Mathf.Max(Mathf.Abs(center - end), Mathf.Abs(center - start));
			return new AngularLimits(center + (start > end ? range : -range), center + (end > start ? range : -range));
		}

		/// <summary>
		/// Returns a new AngularLimits with the same range, but shifted to a specified center point.
		/// </summary>
		/// <param name="center"></param>
		/// <returns></returns>
		public AngularLimits ShiftToCenter(float center)
		{
			var start = _start;
			var end = _end;
			if (RangeIsWrapped)
			{
				if ((Mathf.Sign(center) > 0) == (Mathf.Sign(end) > 0))
				{
					start -= 360f * Mathf.Sign(start);
				}
				else
				{
					end -= 360f * Mathf.Sign(end);
				}
			}
			var range = Mathf.Abs(start - end) * 0.5f;
			return new AngularLimits(center + (start > end ? range : -range), center + (end > start ? range : -range));
		}

		/// <summary>
		/// Returns a new AngularLimits with the same range, but in the reverse direction.
		/// </summary>
		/// <returns></returns>
		public AngularLimits Reverse()
		{
			return new AngularLimits(_end, _start, !CCW);
		}
		
	}
}