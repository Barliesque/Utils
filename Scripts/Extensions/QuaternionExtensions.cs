using UnityEngine;

namespace Barliesque.Utils
{

   static  public class QuaternionExtensions
   {
	   static public Vector4 ToVector4(this Quaternion q) => new Vector4(q.x, q.y, q.z, q.w);
   }

}