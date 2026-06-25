using UnityEngine;
using UnityEngine.Internal;

// Based upon code found at:  https://gist.github.com/maxattack/4c7b4de00f5c1b95a33b
// Original Author: Max Kaufmann (max.kaufmann@gmail.com)

namespace Barliesque.Utils
{
	static public class QuaternionUtils
	{
		
		/// <summary>
		/// Converts a standard, real-world 3D Angular Velocity vector (radians per second
		/// around the X, Y, and Z axes) into a 4D Quaternion derivative (the rate of change
		/// of the quaternion components per second).
		/// </summary>
		/// <param name="orientation">The current orientation</param>
		/// <param name="angularVelocity">Angular velocity per axis, in radians per second</param>
		/// <returns></returns>
		static public Quaternion AngVelToDeriv(Quaternion orientation, Vector3 angularVelocity)
		{
			var spin = new Quaternion(angularVelocity.x, angularVelocity.y, angularVelocity.z, 0f);
			var result = spin * orientation;
			return new Quaternion(0.5f * result.x, 0.5f * result.y, 0.5f * result.z, 0.5f * result.w);
		}


		/// <summary>
		/// Converts the cached velocity used by QuaternionUtils.SmoothDamp to a Vector3 representation, in radians per second. 
		/// </summary>
		/// <param name="current">The current orientation</param>
		/// <param name="deriv">The velocity Quaternion</param>
		/// <returns>The velocity as a Vector3, in radians per second.</returns>
		static public Vector3 DerivToAngVel(Quaternion current, Quaternion deriv)
		{
			var result = deriv * Quaternion.Inverse(current);
			return new Vector3(2f * result.x, 2f * result.y, 2f * result.z);
		}


		/// <summary>
		/// Apply angular velocity to a given orientation.
		/// </summary>
		/// <param name="rotation">The starting orientation.</param>
		/// <param name="angularVelocity">The velocity as radians per second.</param>
		/// <param name="deltaTime">Fraction of a second since the last frame.</param>
		/// <returns></returns>
		static public Quaternion ApplyAngularVelocity(Quaternion rotation, Vector3 angularVelocity, float deltaTime)
		{
			var deriv = AngVelToDeriv(rotation, angularVelocity);
			var pred = new Vector4(
				rotation.x + deriv.x * deltaTime,
				rotation.y + deriv.y * deltaTime,
				rotation.z + deriv.z * deltaTime,
				rotation.w + deriv.w * deltaTime
			).normalized;
			return new Quaternion(pred.x, pred.y, pred.z, pred.w);
		}
		

		/// <summary>
		/// Use this to reset the cached velocity, passed into QuaternionUtils.SmoothDamp
		/// </summary>
		static public Quaternion ZeroVelocity => new (0f, 0f, 0f, 0f);


		[ExcludeFromDocs]
		static public Quaternion SmoothDamp(Quaternion rot, Quaternion target, ref Quaternion velocity, float smoothTime) 
			=> SmoothDamp(rot, target, ref velocity, smoothTime, Time.deltaTime, Mathf.Infinity);

		
		[ExcludeFromDocs]
		static public Quaternion SmoothDamp(Quaternion rot, Quaternion target, ref Quaternion velocity, float smoothTime, float deltaTime) 
			=> SmoothDamp(rot, target, ref velocity, smoothTime, deltaTime, Mathf.Infinity);

		
		/// <summary>
		/// Smoothly damps a Quaternion rotation toward a target rotation over time using an nlerp approximation.
		/// </summary>
		/// <param name="rot">The current rotation of the object.</param>
		/// <param name="target">The target rotation the object is trying to reach.</param>
		/// <param name="velocity">
		/// A reference to a cached Quaternion tracking the 4D rate of change (derivative). 
		/// Note: To manually reset velocity to zero, pass <c>new Quaternion(0, 0, 0, 0)</c>. Do not use Identity.
		/// </param>
		/// <param name="smoothTime">The approximate time in seconds it takes to reach the target destination.</param>
		/// <param name="deltaTime">Time in seconds since the last update.  Default: Time.deltaTime</param>
		/// <param name="maxSpeed">An optional cap on the maximum rotation speed allowed.</param>
		/// <returns>The newly calculated intermediate Quaternion rotation for the current frame.</returns>
		static public Quaternion SmoothDamp(Quaternion rot, Quaternion target, ref Quaternion velocity, float smoothTime,
			[DefaultValue("Time.deltaTime")] float deltaTime,
			[DefaultValue("Mathf.Infinity")] float maxSpeed)
		{
			// Account for double-cover
			if (Quaternion.Dot(rot, target) < 0f)
			{
				target.x *= -1f;
				target.y *= -1f;
				target.z *= -1f;
				target.w *= -1f;
			}

			// Smooth damp (nlerp approx)
			var result = new Vector4(
				Mathf.SmoothDamp(rot.x, target.x, ref velocity.x, smoothTime, maxSpeed, deltaTime),
				Mathf.SmoothDamp(rot.y, target.y, ref velocity.y, smoothTime, maxSpeed, deltaTime),
				Mathf.SmoothDamp(rot.z, target.z, ref velocity.z, smoothTime, maxSpeed, deltaTime),
				Mathf.SmoothDamp(rot.w, target.w, ref velocity.w, smoothTime, maxSpeed, deltaTime)
			).normalized;

			// Compute derivative
			var dtInv = 1f / deltaTime;
			velocity.x = (result.x - rot.x) * dtInv;
			velocity.y = (result.y - rot.y) * dtInv;
			velocity.z = (result.z - rot.z) * dtInv;
			velocity.w = (result.w - rot.w) * dtInv;
			return new Quaternion(result.x, result.y, result.z, result.w);
		}
		

	}
}