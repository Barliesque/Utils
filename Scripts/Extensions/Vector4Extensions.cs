using UnityEngine;

namespace Barliesque.Utils
{
	static public class Vector4Extensions
	{
		static public Quaternion ToQuaternion(this Vector4 v) => new Quaternion() { x = v.x, y = v.y, z = v.z, w = v.w };
	}
}