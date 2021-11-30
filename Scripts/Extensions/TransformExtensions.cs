using System.Collections.Generic;
using UnityEngine;

namespace Barliesque.Utils
{
	static public class TransformExtensions
	{
		/// <summary>
		/// Transform a local rotation to world space. 
		/// </summary>
		/// <param name="xform"></param>
		/// <param name="local">A rotation in the local space of this transform.</param>
		/// <returns></returns>
		static public Quaternion TransformRotation(this Transform xform, Quaternion local)
		{
			return xform.rotation * local;
		}

		/// <summary>
		/// Transform a local rotation to world space. 
		/// </summary>
		/// <param name="xform"></param>
		/// <param name="local">A rotation in the local space of this transform.</param>
		/// <returns></returns>
		static public Vector3 TransformRotation(this Transform xform, Vector3 local)
		{
			return (xform.rotation * Quaternion.Euler(local)).eulerAngles;
		}


		/// <summary>
		/// Transform a world space rotation to local space.
		/// </summary>
		/// <param name="xform"></param>
		/// <param name="world">A rotation in world space.</param>
		/// <returns></returns>
		static public Quaternion InverseTransformRotation(this Transform xform, Quaternion world)
		{
			return Quaternion.Inverse(xform.rotation) * world;
		}

		/// <summary>
		/// Transform a world space rotation to local space.
		/// </summary>
		/// <param name="xform"></param>
		/// <param name="world">A rotation in world space.</param>
		/// <returns></returns>
		static public Vector3 InverseTransformRotation(this Transform xform, Vector3 world)
		{
			return (Quaternion.Inverse(xform.rotation) * Quaternion.Euler(world)).eulerAngles;
		}

		static public void Reset(this Transform xform)
		{
			xform.localPosition = Vector3.zero;
			xform.localRotation = Quaternion.identity;
			xform.localScale = Vector3.one;
		}

		static public Ray Ray(this Transform xform) => new Ray(xform.position, xform.forward);

		static public Transform[] GetHierarchy(this Transform root)
		{
			var skeleton = new List<Transform>();
			var stack = new Stack<Transform>();
			stack.Push(root);

			while (stack.Count > 0)
			{
				var joint = stack.Pop();
				skeleton.Add(joint);
				for (int i = 0; i < joint.childCount; i++) stack.Push(joint.GetChild(i));
			}

			return skeleton.ToArray();
		}

		/// <summary>
		/// Turn (on the world-space Y-axis only) so that the Z-forward axis faces another Transform.
		/// </summary>
		/// <param name="xform"></param>
		/// <param name="subject"></param>
		/// <param name="inverse"></param>
		static public void TurnToFace(this Transform xform, Transform subject, bool inverse = false)
		{
			var subjectPos = subject.position;
			var thisPos = xform.position;
			var delta = inverse ? (thisPos - subjectPos) : (subjectPos - thisPos);
			delta.y = 0f;
			var norm = delta.normalized;
			var angle = 90f - Mathf.Atan2(norm.z, norm.x) * Mathf.Rad2Deg;
			xform.eulerAngles = new Vector3(0f, angle, 0f);
		}
	}
}