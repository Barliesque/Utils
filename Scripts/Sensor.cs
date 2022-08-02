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

		#pragma warning disable 414  /// Assigned but never used -- Except by the SensorEditor
		[SerializeField] private SensorEventType _eventType = (SensorEventType)~0;
		#pragma warning restore 414
		
		[SerializeField] private LayerMask _collisionLayers = ~0;
		
		[Tooltip("If selected, collision with bodies containing multiple colliders will only trigger a single enter/exit event, rather than an event for each collider.")]
		[SerializeField] private bool _oncePerBody = true;
		
		[Flags] public enum SensorEventType { Trigger = 1, Collision = 2 }
		
		public TriggerHandler OnEnterTrigger = new TriggerHandler();
		public TriggerHandler OnStayTrigger = new TriggerHandler();
		public TriggerHandler OnExitTrigger = new TriggerHandler();

		public CollisionHandler OnEnterCollision = new CollisionHandler();
		public CollisionHandler OnStayCollision = new CollisionHandler();
		public CollisionHandler OnExitCollision = new CollisionHandler();

		private ColliderSet _triggered;
		private ColliderSet _collided;

		/// <summary>If true, collision with bodies containing multiple colliders will only trigger a single enter/exit event, rather than an event for each collider.</summary>
		public bool OncePerBody
		{
			get => _oncePerBody;
			set => _oncePerBody = value;
		}

		public LayerMask CollisionLayers
		{
			get => _collisionLayers;
			set => _collisionLayers = value;
		}


		private void Awake()
		{
			if (OncePerBody)
			{
				_triggered = new ColliderSet();
				_collided = new ColliderSet();
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!enabled) return;
			if (!CollisionLayers.Contains(other.gameObject.layer)) return;
			if (OncePerBody && !_triggered.Enter(other)) return;
			OnEnterTrigger?.Invoke(this, other);
		}

		private void OnTriggerStay(Collider other)
		{
			if (!CollisionLayers.Contains(other.gameObject.layer)) return;
			if (enabled) OnStayTrigger?.Invoke(this, other);
		}

		private void OnTriggerExit(Collider other)
		{
			if (!enabled) return;
			if (!CollisionLayers.Contains(other.gameObject.layer)) return;
			if (OncePerBody && !_triggered.Exit(other)) return;
			OnExitTrigger?.Invoke(this, other);
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (!enabled) return;
			if (!CollisionLayers.Contains(collision.body.gameObject.layer)) return;
			if (!OncePerBody)
			{
				OnEnterCollision?.Invoke(this, collision);
				return;
			}

			bool invoked = false;
			foreach (var hit in collision.contacts)
			{
				if (!_collided.Enter(hit.otherCollider)) continue;
				if (invoked) continue;
				OnEnterCollision?.Invoke(this, collision);
				invoked = true;
			}
		}

		private void OnCollisionStay(Collision collision)
		{
			if (!CollisionLayers.Contains(collision.body.gameObject.layer)) return;
			if (enabled) OnStayCollision?.Invoke(this, collision);
		}

		private void OnCollisionExit(Collision collision)
		{
			if (!enabled) return;
			if (!CollisionLayers.Contains(collision.body.gameObject.layer)) return;
			if (!OncePerBody)
			{
				OnExitCollision?.Invoke(this, collision);
				return;
			}

			bool invoked = false;
			foreach (var hit in collision.contacts)
			{
				if (!_collided.Exit(hit.otherCollider)) continue;
				if (invoked) continue;
				OnExitCollision?.Invoke(this, collision);
				invoked = true;
			}
		}
		
	}

}