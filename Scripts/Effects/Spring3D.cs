using UnityEngine;

namespace Barliesque.Utils
{
	/// TODO  This could be revamped, for consistency, using the approach found here:  https://theorangeduck.com/page/spring-roll-call
	public class Spring3D
	{
		public Vector3 Value;
		public Vector3 Target;
		
		/// <summary>
		/// A value from 0 to 1, where 0 is tight and 1 is loose.
		/// </summary>
		public float Springiness
		{
			get => _springiness;
			set => _springiness = Mathf.Clamp01(value);
		}
		private float _springiness = 0.5f;
		
		private Vector3 _velocity;

		/// <summary>
		/// Set the current and target values the same.
		/// </summary>
		public void Reset(Vector3 value)
		{
			Value = Target = value;
		}

		/// <summary>
		/// Set the current and target values.
		/// </summary>
		/// <param name="value">The current value of the spring.</param>
		/// <param name="target">The target value to spring to.</param>
		public void Set(Vector3 value, Vector3 target)
		{
			Value = value;
			Target = target;
		}
	
		public Vector3 Update()
		{
			var delta = Target - Value;
			_velocity = Vector3.Lerp(_velocity, delta, Mathf.Lerp(0.25f, 0.0625f, _springiness));
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