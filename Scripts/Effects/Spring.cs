using UnityEngine;

namespace Barliesque.Utils
{
	/// TODO  This could be revamped, for consistency, using the approach found here:  https://theorangeduck.com/page/spring-roll-call
	public class Spring
	{
		public float Value;
		public float Target;
		
		/// <summary>
		/// A value from 0 to 1, where 0 is tight and 1 is loose.
		/// </summary>
		public float Springiness
		{
			get => _springiness;
			set => _springiness = Mathf.Clamp01(value);
		}
		private float _springiness = 0.5f;

		private float _velocity;

		/// <summary>
		/// Set the current and target values the same.
		/// </summary>
		public void Reset(float value)
		{
			Value = Target = value;
		}

		/// <summary>
		/// Set the current and target values.
		/// </summary>
		/// <param name="value">The current value of the spring.</param>
		/// <param name="target">The target value to spring to.</param>
		public void Set(float value, float target)
		{
			Value = value;
			Target = target;
		}
	
		public float Update()
		{
			var delta = Target - Value;
			_velocity = Mathf.Lerp(_velocity, delta, Mathf.Lerp(0.25f, 0.03125f, _springiness));
			Value += _velocity;
			return Value;
		}

		static public float Update(float current, float target, ref float velocity, float stiffness = 0.5f)
		{
			var delta = target - current;
			velocity = Mathf.Lerp(velocity, delta, Mathf.Lerp(0.25f, 0.03125f, stiffness));
			current += velocity;
			return current;
		}
	
	}
}