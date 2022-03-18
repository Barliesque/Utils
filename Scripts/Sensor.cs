using System;
using UnityEngine;
using UnityEngine.Events;


namespace Barliesque.Utils
{

	/// <summary>
	/// Enables more convenient handling of 3D physics collision and trigger events.
	/// </summary>
	public class Sensor : MonoBehaviour
	{

		[Serializable] public class TriggerHandler : UnityEvent<Sensor, Collider> { }
		[Serializable] public class CollisionHandler : UnityEvent<Sensor, Collision> { }

		[SerializeField] private bool _collisionOrTrigger;
		
		//TODO  Add option to ignore repeated events for the same other body in any given frame
		//TODO    - Use a coroutine to clear a list of Rigidbodies on the next frame

		public TriggerHandler OnEnterTrigger;
		public TriggerHandler OnStayTrigger;
		public TriggerHandler OnExitTrigger;

		public CollisionHandler OnEnterCollision;
		public CollisionHandler OnStayCollision;
		public CollisionHandler OnExitCollision;


		private void OnTriggerEnter(Collider other)
		{
			if (enabled) OnEnterTrigger?.Invoke(this, other);
		}

		private void OnTriggerStay(Collider other)
		{
			if (enabled) OnStayTrigger?.Invoke(this, other);
		}

		private void OnTriggerExit(Collider other)
		{
			if (enabled) OnExitTrigger?.Invoke(this, other);
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (enabled) OnEnterCollision?.Invoke(this, collision);
		}

		private void OnCollisionStay(Collision collision)
		{
			if (enabled) OnStayCollision?.Invoke(this, collision);
		}

		private void OnCollisionExit(Collision collision)
		{
			if (enabled) OnExitCollision?.Invoke(this, collision);
		}

	}

}