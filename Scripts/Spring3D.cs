using UnityEngine;

namespace Barliesque.Utils
{
	public class Spring3D
	{
		public Vector3 Value;
		public Vector3 Target;
		public float Springiness;

		private Vector3 _velocity;

		public void Set(Vector3 value, Vector3 target)
		{
			Value = value;
			Target = target;
		}
	
		public Vector3 Update()
		{
			var delta = Target - Value;
			_velocity = Vector3.Lerp(_velocity, delta, Mathf.Lerp(0.25f, 0.0625f, Springiness));
			Value += _velocity;
			return Value;
		}

		static public Vector3 Update(Vector3 current, Vector3 target, ref Vector3 velocity, float springiness = 1f)
		{
			var delta = target - current;
			velocity = Vector3.Lerp(velocity, delta, Mathf.Lerp(0.25f, 0.0625f, springiness));
			current += velocity;
			return current;
		}
	
	}
}