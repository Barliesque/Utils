using UnityEngine;


namespace Barliesque.Utils
{

	static public class ComponentExtensions
	{

		static public T GetComponentInParent<T>(this Component component, bool includeInactive) where T : Component
		{
			var here = component.transform;
			T result = null;
			while (here && !result)
			{
				if (includeInactive || here.gameObject.activeSelf)
				{
					result = here.GetComponent<T>();
				}
				here = here.parent;
			}
			return result;
		}
		
		/// <summary>
		/// Can any of the specified component types be found in this or a parent GameObject?
		/// </summary>
		static public bool IsComponentInParent(this Component here, params System.Type[] types)
		{
			foreach (var type in types)
			{
				if (here.GetComponentInParent(type, true)) return true;
			}
			return false;
		}
		
		
		/// <summary>
		/// Can any of the specified component types be found on this GameObject?
		/// </summary>
		static public bool IsComponentPresent(this Component here, params System.Type[] types)
		{
			foreach (var type in types)
			{
				if (here.GetComponent(type)) return true;
			}
			return false;
		}

		

	}

}