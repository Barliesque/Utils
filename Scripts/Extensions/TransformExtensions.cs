using System.Collections.Generic;
using System.Text;
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
		/// Turn (on the world-space Y-axis, and optionally the x-axis) so that the Z-forward axis faces another Transform.
		/// </summary>
		/// <param name="xform">This transform</param>
		/// <param name="subject">The transform to face</param>
		/// <param name="pitch">True to enable pitch rotation</param>
		/// <param name="inverse">If true, then rotation will be to face away from the subject</param>
		static public void TurnToFace(this Transform xform, Transform subject, bool pitch, bool inverse = false)
		{
			var subjectPos = subject.position;
			var thisPos = xform.position;
			var delta = inverse ? (thisPos - subjectPos) : (subjectPos - thisPos);
			
			var angleY = 90f - Mathf.Atan2(delta.z, delta.x) * Mathf.Rad2Deg;
			var angleX = 0f;
			if (pitch)
			{
				angleX = Mathf.Atan2(-delta.y, delta.magnitude) * Mathf.Rad2Deg;
			}

			xform.rotation = Quaternion.Euler(0f, angleY, 0f) * Quaternion.Euler(angleX, 0f, 0f);
		}

		static public string GetPath(this Transform xform, char separator='/')
		{
			var path = new StringBuilder(xform.name);
			while (xform.parent != null)
			{
				xform = xform.parent;
				path.Insert(0, separator);
				path.Insert(0, xform.name);
			}
			return path.ToString();
		}
		
		static public Transform GetOrAddChild(this Transform xform, string name, Vector3 position = default, Quaternion rotation = default)
		{
			var child = xform.Find(name);
			if (child) return child;
			
			var childGO = new GameObject(name);
			child = childGO.transform;
			child.SetParent(xform, false);
			child.localPosition = position;
			child.localRotation = rotation;
			return child;
		}

		
	}
}