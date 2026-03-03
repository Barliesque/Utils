using UnityEngine;

namespace Barliesque.Utils
{
	static public class Vector3Extensions
	{
		/// <summary>
		/// Get a normalized direction vector and the distance of the original vector.
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="distance"></param>
		/// <returns></returns>
		static public Vector3 Direction(this Vector3 vector, out float distance)
		{
			distance = vector.magnitude;
			return vector / distance;
		}

		/// <summary>
		/// Gets the value of a specified axis from this Vector3.
		/// </summary>
		/// <param name="vec3"></param>
		/// <param name="axis"></param>
		/// <returns></returns>
		static public float GetAxis(this Vector3 vec3, Axis axis)
		{
			return axis == Axis.X ? vec3.x : (axis == Axis.Y ? vec3.y : vec3.z);
		}

		/// <summary>
		/// Returns the Vector3 with a specified axis value changed.  Note that this cannot modify the original Vector3 and must be reassigned.
		/// </summary>
		/// <param name="vec3"></param>
		/// <param name="axis"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		static public Vector3 SetAxis(this Vector3 vec3, Axis axis, float value)
		{
			if (axis == Axis.X) vec3.x = value;
			else if (axis == Axis.Y) vec3.y = value;
			else vec3.z = value;
			return vec3;
		}

		/// <summary>
		/// Copies the values in selected axes from another Vector3.  Note that this cannot modify the original Vector3 and must be reassigned.
		/// </summary>
		/// <param name="vec3"></param>
		/// <param name="axes"></param>
		/// <param name="copyFrom"></param>
		/// <returns></returns>
		static public Vector3 CopyAxes(this Vector3 vec3, Axis axes, Vector3 copyFrom)
		{
			if ((axes & Axis.X) == Axis.X) vec3.x = copyFrom.x;
			if ((axes & Axis.Y) == Axis.Y) vec3.y = copyFrom.y;
			if ((axes & Axis.Z) == Axis.Z) vec3.z = copyFrom.z;
			return vec3;
		}

		/// <summary>
		/// Divides each component of this vector by the corresponding component of the scale vector.
		/// </summary>
		static public Vector3 Divide(this Vector3 vec3, Vector3 divisor)
		{
			return new Vector3(vec3.x / divisor.x, vec3.y / divisor.y, vec3.z / divisor.z);
		}

		/// <summary>
		/// Returns false if any component of the vector is infinite or NaN.
		/// </summary>
		static public bool IsValid(this Vector3 vec3)
		{
//			return float.IsFinite(vec3.x) && float.IsFinite(vec3.y) && float.IsFinite(vec3.z);
			return !(float.IsNaN(vec3.x) || float.IsNaN(vec3.y) || float.IsNaN(vec3.z) ||
			         float.IsInfinity(vec3.x) || float.IsInfinity(vec3.y) || float.IsInfinity(vec3.z));
		}

		/// <summary>
		/// Calculate the distance to a line segment.  See Mathf2 for additional variations of this function.
		/// </summary>
		/// <param name="pos">Position to measure from</param>
		/// <param name="segA">Point A of the segment</param>
		/// <param name="segB">Point B of the segment</param>
		static public float DistToSeg(this Vector3 pos, Vector3 segA, Vector3 segB)
		{
			var delta = segB - segA;
			var segLenSqr = delta.sqrMagnitude;
			var distA = pos - segA;

			var t = Mathf.Clamp01((distA.x * delta.x + distA.y * delta.y + distA.z * delta.z) / segLenSqr);
			var seg = new Vector3(segA.x + t * delta.x, segA.y + t * delta.y, segA.z + t * delta.z);

			return (pos - seg).magnitude;
		}

		/// <summary>
		/// Calculates the cubic volume, where this Vector3 represents the half-dimensions
		/// </summary>
		/// <param name="size">Dimensions of the volume</param>
		/// <param name="isHalf">True if dimensions are given as half the width, height, depth.</param>
		/// <returns></returns>
		static public float Volume(this Vector3 size, bool isHalf = false)
		{
			if (isHalf) return size.x * size.y * size.z * 8f;
			return size.x * size.y * size.z;
		}

		/// <summary>
		/// Extract the XZ components as a Vector2
		/// </summary>
		/// <param name="vec3"></param>
		/// <returns></returns>
		static public Vector2 xz(this Vector3 vec3) => new(vec3.x, vec3.z);

	}
}