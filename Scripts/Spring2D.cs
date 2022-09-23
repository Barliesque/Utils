using UnityEngine;

namespace Barliesque.Utils
{
	/// TODO  This should be revamped, for consistency, using the approach found here:  https://theorangeduck.com/page/spring-roll-call
	public class Spring2D
	{
		public Vector2 Value;
		public Vector2 Target;
		public float Springiness;

		private Vector2 _velocity;

		public void Set(Vector2 value, Vector2 target, float springiness = 0.5f)
		{
			Value = value;
			Target = target;
			Springiness = springiness;
		}
	
		public Vector2 Update()
		{
			var delta = Target - Value;
			var t = Mathf.Lerp(0.25f, 0.0625f, Springiness * Springiness);
			_velocity = Vector2.Lerp(_velocity, delta, t);
			Value += _velocity;
			return Value;
		}

		static public Vector2 Update(Vector2 current, Vector2 target, ref Vector2 velocity, float springiness = 0.5f)
		{
			var delta = target - current;
			var t = Mathf.Lerp(0.25f, 0.0625f, springiness * springiness);
			velocity = Vector2.Lerp(velocity, delta, t);
			current += velocity;
			return current;
		}
	
	}
}