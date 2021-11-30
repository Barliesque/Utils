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
	}
}