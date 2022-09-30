using UnityEngine;

namespace Barliesque.Utils
{

	static public class BoundsExtensions
	{

		/// <summary>
		/// Expand the bounds to include a given point.
		/// </summary>
		/// <param name="bounds"></param>
		/// <param name="point"></param>
		/// <returns>Returns the new bounds.</returns>
		static public Bounds AddPoint(this Bounds bounds, Vector3 point)
		{
			if (bounds.Contains(point)) return bounds;
			bounds.SetMinMax(Vector3.Min(bounds.min, point), Vector3.Max(bounds.max, point));
			return bounds;
		}
		
	}
	
}