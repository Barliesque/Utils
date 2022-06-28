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
		

		static public string GetPath(this GameObject go, char separator='/')
		{
			return go.transform.GetPath(separator);
		}

	}

}