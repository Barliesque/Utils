using UnityEngine;

namespace Barliesque.Utils
{
	/// TODO  This should be revamped, for consistency, using the approach found here:  https://theorangeduck.com/page/spring-roll-call
	public class Spring
	{
		public float Value;
		public float Target;
		public float Springiness;

		private float _velocity;

		public void Set(float value)
		{
			Value = Target = value;
		}
	
		public void Set(float value, float target)
		{
			Value = value;
			Target = target;
		}
	
		public float Update()
		{
			var delta = Target - Value;
			_velocity = Mathf.Lerp(_velocity, delta, Mathf.Lerp(0.25f, 0.03125f, Springiness));
			Value += _velocity;
			return Value;
		}

		static public float Update(float current, float target, ref float velocity, float springiness = 1f)
		{
			var delta = target - current;
			velocity = Mathf.Lerp(velocity, delta, Mathf.Lerp(0.25f, 0.03125f, springiness));
			current += velocity;
			return current;
		}
	
	}
}