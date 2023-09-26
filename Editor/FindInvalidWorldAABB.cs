using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Text;

// Based on a concept found here:
// https://forum.unity.com/threads/assertion-failed-invalid-worldaabb-object-is-too-large-or-too-far-away-from-the-origin.486290/

namespace Barliesque.Utils.Editor
{
    static public class FindInvalidWorldAABB
    {
        [MenuItem("Tools/Find Invalid WorldAABB")]
        static public void FindInvalidAABB()
        {
            var invalidObjects = GetInvalidAABB();
            if (invalidObjects > 0)
                Debug.LogError($"Found invalid worldAABB!  Object count:  {invalidObjects}");
            else
                Debug.Log("No invalid worldAABB objects found.");
        }
 
        /// Returns objects with invalid scale or position.
        static private int GetInvalidAABB()
        {
            var result = 0;
            var allObjects = Object.FindObjectsOfType<GameObject>(true);
         
            foreach (var obj in allObjects)
            {
                var rectTransform = obj.GetComponent<RectTransform>();
                if (rectTransform != null) continue;
 
                var position = obj.transform.position;
                var scale = obj.transform.localScale;
             
                if (!float.IsFinite(position.x) || !float.IsFinite(position.y) || !float.IsFinite(position.z)
                    || !float.IsFinite(scale.x) || !float.IsFinite(scale.y) || !float.IsFinite(scale.z))
                {
                    ++result;
                    Debug.LogError($"Found invalid worldAABB object {GetObjectPath(obj.transform)}", obj);
                }
            }
 
            return result;
        }
 
 
        static private string GetObjectPath(Transform transform)
        {
            var result = new StringBuilder();
            while (transform != null)
            {
                result.Append(transform.gameObject.name);
                result.Append('/');
                transform = transform.parent;
            }
            return result.ToString();
        }
        
    }
}
