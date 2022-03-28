using UnityEngine;

namespace Barliesque.Utils
{
	static public class Vector3Extensions
	{
		static public float GetAxis(this Vector3 vec3, Axis axis)
		{
			return axis == Axis.X ? vec3.x : (axis == Axis.Y ? vec3.y : vec3.z);
		}

		static public Vector3 SetAxis(this Vector3 vec3, Axis axis, float value)
		{
			if (axis == Axis.X) vec3.x = value;
			else if (axis == Axis.Y) vec3.y = value;
			else vec3.z = value;
			return vec3;
		}

		/// <summary>
		/// Divides each component of this vector by the corresponding component of the scale vector.
		/// </summary>
		static public Vector3 InverseScale(this Vector3 vec3, Vector3 scale)
		{
			return new Vector3(vec3.x / scale.x, vec3.y / scale.y, vec3.z / scale.z);
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
		
		
	}
}