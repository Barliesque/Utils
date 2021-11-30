using System;
using UnityEngine;
using UnityEngine.Events;


namespace Barliesque.Utils
{

	/// <summary>
	/// Enables handling of collision events.
	/// </summary>
	public class Sensor : MonoBehaviour
	{

		[Serializable] public class TriggerHandler : UnityEvent<Sensor, Collider> { }
		[Serializable] public class CollisionHandler : UnityEvent<Sensor, Collision> { }

		public TriggerHandler OnEnterTrigger;
		public TriggerHandler OnStayTrigger;
		public TriggerHandler OnExitTrigger;

		public CollisionHandler OnEnterCollision;
		public CollisionHandler OnStayCollision;
		public CollisionHandler OnExitCollision;


		private void OnTriggerEnter(Collider other)
		{
			OnEnterTrigger?.Invoke(this, other);
		}

		private void OnTriggerStay(Collider other)
		{
			OnStayTrigger?.Invoke(this, other);
		}

		private void OnTriggerExit(Collider other)
		{
			OnExitTrigger?.Invoke(this, other);
		}

		private void OnCollisionEnter(Collision collision)
		{
			OnEnterCollision?.Invoke(this, collision);
		}

		private void OnCollisionStay(Collision collision)
		{
			OnStayCollision?.Invoke(this, collision);
		}

		private void OnCollisionExit(Collision collision)
		{
			OnExitCollision?.Invoke(this, collision);
		}

	}

}