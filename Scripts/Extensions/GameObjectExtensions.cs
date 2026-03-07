using System.Collections.Generic;
using UnityEngine;

namespace Barliesque.Utils
{


	static public class GameObjectExtensions
	{

		static public T GetOrAddComponent<T>(this GameObject go) where T : Component
		{
			var component = go.GetComponent<T>();
			if (!component) component = go.AddComponent<T>();
			return component;
		}

		static public T GetOrAddComponent<T>(this GameObject go, out bool added) where T : Component
		{
			var component = go.GetComponent<T>();
			if (!component)
			{
				component = go.AddComponent<T>();
				added = true;
			}
			else
			{
				added = false;
			}
			return component;
		}
		

		static public GameObject GetOrAddChild(this GameObject go, string name, Vector3 position = default, Quaternion rotation = default)
		{
			var child = go.transform.Find(name);
			if (child) return child.gameObject;
			
			var childGO = new GameObject(name);
			child = childGO.transform;
			child.SetParent(go.transform, false);
			child.localPosition = position;
			child.localRotation = rotation;
			return childGO;
		}
		

		static public string GetPath(this GameObject go, char separator='/')
		{
			return go.transform.GetPath(separator);
		}

		/// <summary>
		/// Sets the layer of this and all child GameObjects.
		/// </summary>
		/// <param name="go"></param>
		/// <param name="layer"></param>
		static public void SetLayer(this GameObject go, int layer)
		{
			var xforms = new Stack<Transform>();
			xforms.Push(go.transform);
			while (xforms.Count > 0)
			{
				var xform = xforms.Pop();
				xform.gameObject.layer = layer;
				for (int i = 0, len = xform.childCount; i < len; i++)
				{
					xforms.Push(xform.GetChild(i));
				}
			}
		}

		/// <summary>
		/// Sets the layer of this and all child GameObjects.
		/// </summary>
		/// <param name="go"></param>
		/// <param name="layer"></param>
		static public void SetLayer(this GameObject go, string layer)
		{
			SetLayer(go, LayerMask.NameToLayer(layer));
		}

		
		/// <summary>
		/// Activate or deactivate each immediate child of this GameObject. 
		/// </summary>
		/// <param name="go"></param>
		/// <param name="active">The active state to set, where true sets the GameObject active and false sets it to inactive.</param>
		static public void SetChildrenActive(this GameObject go, bool active)
		{
			var xform = go.transform;
			for (int i = 0, len = xform.childCount; i < len; i++)
			{
				xform.GetChild(i).gameObject.SetActive(active);
			}
		}

	}

}