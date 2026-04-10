using System.ComponentModel;
using UnityEngine;

namespace Barliesque.Utils
{
	static public class PhysicsUtils
	{
		static public int OverlapCapsuleNonAlloc(CapsuleCollider capsule, Collider[] results,
			[DefaultValue("AllLayers")] int layerMask,
			[DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			var xform = capsule.transform;
			var center = xform.TransformPoint(capsule.center);
			var direction = capsule.direction;
			var halfHeight =
				xform.TransformPoint((direction == 0 ? Vector3.right : (direction == 1 ? Vector3.up : Vector3.forward)) * (capsule.height * 0.5f))
				- xform.position;
			var radius = capsule.radius * xform.lossyScale.x; // Note: Assumes uniform scaling
			return Physics.OverlapCapsuleNonAlloc(center - halfHeight, center + halfHeight, radius, results);
		}

		static public int OverlapNonAlloc(this CapsuleCollider capsule, Collider[] results,
			[DefaultValue("AllLayers")] int layerMask,
			[DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)

		{
			return OverlapCapsuleNonAlloc(capsule, results, layerMask, queryTriggerInteraction);
		}

		/// <summary>
		/// Cast a conical shape and store results in buffer.  This version will collect all hits within the cone.
		/// Omit hitCollect[] to stop casting as soon as a result has been found.
		/// </summary>
		/// <param name="origin">The start position in world space.</param>
		/// <param name="direction">The direction of the cone cast.</param>
		/// <param name="minRadius">The radius at the origin of the cone.</param>
		/// <param name="spread">The angle of the cone, in degrees.</param>
		/// <param name="maxDistance">The maximum depth of the cone.</param>
		/// <param name="hitCollect">A workspace array.  If omitted, casting will stop as soon as results have been collected.</param>
		/// <param name="results">An array of RaycastHits containing the result of the cast.</param>
		/// <param name="layers">Layers to be included in the cast.</param>
		/// <param name="segments">Break up the depth of the cone into segments to refine the conical shape.  Casting starts with the minimum radius across the full depth of the cone, and expands with successive sphere casts to approximate a conical shape.</param>
		/// <param name="queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>Returns a count of the Raycast hits stored in the results buffer.</returns>
		static public int ConeCastNonAlloc(Vector3 origin, Vector3 direction, float minRadius, float spread, float maxDistance,
			RaycastHit[] hitCollect, RaycastHit[] results, [DefaultValue("AllLayers")] LayerMask layers, int segments = 8,
			QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
		{
			var there = origin + direction * maxDistance;
			float maxRadius = Mathf.Tan(spread * Mathf.Deg2Rad) * maxDistance;

			var t = 0f;
			var tStep = 1f / segments;
			var t2 = 0f;
			var t2Step = 1f / (segments - 1);
			int hitCount = 0;

			for (int i = 0; i < segments; i++)
			{
				var radius = Mathf.Lerp(minRadius, maxRadius, t2);
				var start = Vector3.Lerp(origin, there, t);
				var distance = maxDistance * (1f - tStep * i);

				var count = Physics.SphereCastNonAlloc(start - direction * radius, radius, direction, hitCollect, distance, layers,
					queryTriggerInteraction);

				for (int c = 0; c < count; c++)
				{
					if (hitCollect[c].distance == 0f) continue;
					
					var dupe = false;
					for (int d = 0; d < c; d++)
						if ((hitCollect[d].point - hitCollect[c].point).sqrMagnitude < 0.01f)
						{
							dupe = true;
							break;
						}
					if (dupe) continue;
						
					results[hitCount++] = hitCollect[c];
					if (hitCount == results.Length) break;
				}

				if (hitCount == results.Length) break;

				t += tStep;
				t2 += t2Step;
			}

			return hitCount;
		}
		

		/// <summary>
		/// Cast a conical shape and store results in buffer.  This version will collect all hits within the cone.
		/// Omit hitCollect[] to stop casting as soon as a result has been found.
		/// </summary>
		/// <param name="origin">The start position in world space.</param>
		/// <param name="direction">The direction of the cone cast.</param>
		/// <param name="minRadius">The radius at the origin of the cone.</param>
		/// <param name="spread">The angle of the cone, in degrees.</param>
		/// <param name="maxDistance">The maximum depth of the cone.</param>
		/// <param name="results">An array of RaycastHits containing the result of the cast.</param>
		/// <param name="layers">Layers to be included in the cast.</param>
		/// <param name="segments">Break up the depth of the cone into segments to refine the conical shape.  Casting starts with the minimum radius across the full depth of the cone, and expands with successive sphere casts to approximate a conical shape.</param>
		/// <param name="queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>Returns a count of the Raycast hits stored in the results buffer.</returns>
		static public int ConeCastNonAlloc(Vector3 origin, Vector3 direction, float minRadius, float spread, float maxDistance,
			RaycastHit[] results, [DefaultValue("AllLayers")] LayerMask layers, int segments = 8,
			QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
		{
			var there = origin + direction * maxDistance;
			float maxRadius = Mathf.Tan(spread * Mathf.Deg2Rad) * maxDistance;

			var t = 0f;
			var tStep = 1f / segments;
			var t2 = 0f;
			var t2Step = 1f / (segments - 1);
			int hitCount = 0;

			for (int i = 0; i < segments; i++)
			{
				var radius = Mathf.Lerp(minRadius, maxRadius, t2);
				var start = Vector3.Lerp(origin, there, t);
				var distance = maxDistance * (1f - tStep * i);

				var count = Physics.SphereCastNonAlloc(start - direction * radius, radius, direction, results, distance, layers,
					queryTriggerInteraction);

				for (int c = 0; c < count; c++)
				{
					if (results[c].distance == 0f) continue;
						
					results[hitCount++] = results[c];
					if (hitCount == results.Length) break;
				}

				if (hitCount == results.Length) break;
				if (hitCount > 0) break;

				t += tStep;
				t2 += t2Step;
			}

			return hitCount;
		}
		
	}
}