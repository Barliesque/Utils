using UnityEngine;

namespace Barliesque.Utils
{
	/// TODO  This could be revamped, for consistency, using the approach found here:  https://theorangeduck.com/page/spring-roll-call
	public class Spring2D
	{
		public Vector2 Value;
		public Vector2 Target;
		
		/// <summary>
		/// A value from 0 to 1, where 0 is tight and 1 is loose.
		/// </summary>
		public float Springiness
		{
			get => _springiness;
			set => _springiness = Mathf.Clamp01(value);
		}
		private float _springiness = 0.5f;

		private Vector2 _velocity;

		/// <summary>
		/// Set the current and target values the same.
		/// </summary>
		public void Reset(Vector2 value)
		{
			Value = Target = value;
		}
		
		/// <summary>
		/// Set the current and target values.
		/// </summary>
		/// <param name="value">The current value of the spring.</param>
		/// <param name="target">The target value to spring to.</param>
		public void Set(Vector2 value, Vector2 target)
		{
			Value = value;
			Target = target;
		}
	
		public Vector2 Update()
		{
			var delta = Target - Value;
			_velocity = Vector2.Lerp(_velocity, delta, Mathf.Lerp(0.25f, 0.0625f, _springiness * _springiness));
			Value += _velocity;
			return Value;
		}

		static public Vector2 Update(Vector2 current, Vector2 target, ref Vector2 velocity, float springiness = 0.5f)
		{
			var delta = target - current;
			velocity = Vector2.Lerp(velocity, delta, Mathf.Lerp(0.25f, 0.0625f, springiness * springiness));
			current += velocity;
			return current;
		}
	
	}
}