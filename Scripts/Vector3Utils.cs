using UnityEngine;

namespace Barliesque.Utils
{

	static public class Vector3Utils
	{

		/// <summary>
		/// Interpolate between two euler angle rotations
		/// </summary>
		static public Vector3 LerpAngle(Vector3 a, Vector3 b, float t)
		{
			return new Vector3(
				Mathf.LerpAngle(a.x, b.x, t),
				Mathf.LerpAngle(a.y, b.y, t),
				Mathf.LerpAngle(a.z, b.z, t)
			);
		}

		static public Vector3 SmoothDampAngle(Vector3 a, Vector3 b, ref Vector3 velocity, float smoothTime)
		{
			var smoothed = new Vector3(
				Mathf.SmoothDampAngle(a.x, b.x, ref velocity.x, smoothTime),
				Mathf.SmoothDampAngle(a.y, b.y, ref velocity.y, smoothTime),
				Mathf.SmoothDampAngle(a.z, b.z, ref velocity.z, smoothTime)
			);
			return smoothed;
		}

		/// <summary>
		/// Returns a random vector with specified magnitude.
		/// </summary>
		/// <returns></returns>
		static public Vector3 Random(float magnitude = 1f)
		{
			return UnityEngine.Random.onUnitSphere * magnitude;
			// return new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * magnitude;
		}

		/// <summary>
		/// Normalize euler angles to the range -180 to +180
		/// </summary>
		/// <param name="eulers"></param>
		/// <returns></returns>
		static public Vector3 FixEulers(Vector3 eulers)
		{
			return new Vector3(
				Mathf.Repeat(eulers.x + 180f, 360f) - 180f,
				Mathf.Repeat(eulers.y + 180f, 360f) - 180f,
				Mathf.Repeat(eulers.z + 180f, 360f) - 180f
			);
		}

		/// <summary>
		/// Compare two direction vectors.
		/// </summary>
		/// <param name="a">The first direction vector</param>
		/// <param name="b">The second direction vector</param>
		/// <returns>Returns a value from 0 to 1, where 1 is equality and 0 is an angle of 90 degrees or more.</returns>
		static public float Compare(Vector3 a, Vector3 b)
		{
			return Mathf.Clamp01(Vector3.Dot(a, b));
		}

		/// <summary>
		/// Get a normalized direction vector from one position to another, as well as the distance between the points.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="distance"></param>
		/// <returns></returns>
		static public Vector3 Direction(Vector3 from, Vector3 to, out float distance)
		{
			return (to - from).Direction(out distance);
		}

	}

}