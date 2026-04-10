using UnityEngine;

// Found at:  https://gist.github.com/maxattack/4c7b4de00f5c1b95a33b
// Original Author: Max Kaufmann (max.kaufmann@gmail.com)

namespace Barliesque.Utils
{
	static public class QuaternionUtils
	{
		static public Quaternion AngVelToDeriv(Quaternion current, Vector3 angularVelocity)
		{
			var spin = new Quaternion(angularVelocity.x, angularVelocity.y, angularVelocity.z, 0f);
			var result = spin * current;
			return new Quaternion(0.5f * result.x, 0.5f * result.y, 0.5f * result.z, 0.5f * result.w);
		}


		static public Vector3 DerivToAngVel(Quaternion current, Quaternion deriv)
		{
			var result = deriv * Quaternion.Inverse(current);
			return new Vector3(2f * result.x, 2f * result.y, 2f * result.z);
		}


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


		static public Quaternion SmoothDamp(Quaternion rot, Quaternion target, ref Quaternion velocity, float smoothTime, float maxSpeed = Mathf.Infinity)
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
				Mathf.SmoothDamp(rot.x, target.x, ref velocity.x, smoothTime, maxSpeed),
				Mathf.SmoothDamp(rot.y, target.y, ref velocity.y, smoothTime, maxSpeed),
				Mathf.SmoothDamp(rot.z, target.z, ref velocity.z, smoothTime, maxSpeed),
				Mathf.SmoothDamp(rot.w, target.w, ref velocity.w, smoothTime, maxSpeed)
			).normalized;

			// Compute deriv
			var dtInv = 1f / Time.deltaTime;
			velocity.x = (result.x - rot.x) * dtInv;
			velocity.y = (result.y - rot.y) * dtInv;
			velocity.z = (result.z - rot.z) * dtInv;
			velocity.w = (result.w - rot.w) * dtInv;
			return new Quaternion(result.x, result.y, result.z, result.w);
		}

		static public Quaternion SmoothDampUnscaled(Quaternion rot, Quaternion target, ref Quaternion velocity, float smoothTime, float maxSpeed = Mathf.Infinity)
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
				Mathf.SmoothDamp(rot.x, target.x, ref velocity.x, smoothTime, maxSpeed, Time.unscaledDeltaTime),
				Mathf.SmoothDamp(rot.y, target.y, ref velocity.y, smoothTime, maxSpeed, Time.unscaledDeltaTime),
				Mathf.SmoothDamp(rot.z, target.z, ref velocity.z, smoothTime, maxSpeed, Time.unscaledDeltaTime),
				Mathf.SmoothDamp(rot.w, target.w, ref velocity.w, smoothTime, maxSpeed, Time.unscaledDeltaTime)
			).normalized;

			// Compute deriv
			var dtInv = 1f / Time.deltaTime;
			velocity.x = (result.x - rot.x) * dtInv;
			velocity.y = (result.y - rot.y) * dtInv;
			velocity.z = (result.z - rot.z) * dtInv;
			velocity.w = (result.w - rot.w) * dtInv;
			return new Quaternion(result.x, result.y, result.z, result.w);
		}
	}
}