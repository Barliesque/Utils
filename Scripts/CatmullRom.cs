using UnityEngine;

namespace Barliesque.Utils
{
	
	static public class CatmullRom
	{
		static public Vector2 Position(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
		{
			var t2 = t * t;
			Vector2 q = 0.5f * ((2f * p1) +
			                    (p2 - p0) * t +
			                    (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
			                    (3f * p1 - p0 - 3f * p2 + p3) * t2 * t);
			return q;
		}
	
		static public Vector3 Position(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			var t2 = t * t;
			Vector3 q = 0.5f * ((2f * p1) +
			                    (p2 - p0) * t +
			                    (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
			                    (3f * p1 - p0 - 3f * p2 + p3) * t2 * t);
			return q;
		}
	
		static public Vector4 Position(Vector4 p0, Vector4 p1, Vector4 p2, Vector4 p3, float t)
		{
			var t2 = t * t;
			Vector4 q = 0.5f * ((2f * p1) +
			                    (p2 - p0) * t +
			                    (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
			                    (3f * p1 - p0 - 3f * p2 + p3) * t2 * t);
			return q;
		}
	
		static public Quaternion Rotation(Quaternion r0, Quaternion r1, Quaternion r2, Quaternion r3, float t)
		{
//		var p0 = r0 * Vector3.forward;
//		var p1 = r1 * Vector3.forward;
//		var p2 = r2 * Vector3.forward;
//		var p3 = r3 * Vector3.forward;
//		var p = Position (p0, p1, p2, p3, t);
//		p.Normalize();
//		var q = new Quaternion();
//		q.SetLookRotation(p);
//		return q;
		
			var p0 = new Vector4(r0.x,r0.y,r0.z,r0.w);
			var p1 = new Vector4(r1.x,r1.y,r1.z,r1.w);
			var p2 = new Vector4(r2.x,r2.y,r2.z,r2.w);
			var p3 = new Vector4(r3.x,r3.y,r3.z,r3.w);
			var p = Position (p0, p1, p2, p3, t);
			return new Quaternion(p.x,p.y,p.z,p.w);
		}
	
		static public Vector3 Euler(Quaternion r0, Quaternion r1, Quaternion r2, Quaternion r3, float t)
		{
			var p0 = r0.eulerAngles;
			var p1 = r1.eulerAngles;
			var p2 = r2.eulerAngles;
			var p3 = r3.eulerAngles;
			return Position (p0, p1, p2, p3, t);
		}
	
	}
	
}