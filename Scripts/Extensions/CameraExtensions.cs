using UnityEngine;

namespace Barliesque.Utils
{
	static public class CameraExtensions
	{
		static public bool IsInFrustum(this Camera camera, Renderer renderer) {
			if (!renderer) return false;

			// Quick reject if layer is not rendered by this camera
			if ((camera.cullingMask & (1 << renderer.gameObject.layer)) == 0) return false;

			// Calculate frustum planes and test the renderer's world AABB
			var planes = GeometryUtility.CalculateFrustumPlanes(camera);
			return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
		}
		
		static public bool IsInFrustum(this Camera camera, Bounds bounds) {
			// Calculate frustum planes and test the renderer's world AABB
			var planes = GeometryUtility.CalculateFrustumPlanes(camera);
			return GeometryUtility.TestPlanesAABB(planes, bounds);
		}
		
	}
}