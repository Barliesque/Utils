using System;
using UnityEngine;
using UnityEngine.Events;


namespace Barliesque.Utils
{
	//TODO  BUG: Multiple trigger colliders trigger multiple events
	//TODO  Subscribing to an event that will never be fired ought to log an error - If only when running in the Editor



	/// <summary>
	/// Enables more convenient handling of 3D physics collision and trigger events.
	/// </summary>
	public class Sensor : MonoBehaviour
	{

		[Serializable] public class TriggerHandler : UnityEvent<Sensor, Collider> { }
		[Serializable] public class CollisionHandler : UnityEvent<Sensor, Collision> { }

		#pragma warning disable 414  /// Assigned but never used -- Except by the SensorEditor
		[SerializeField] private SensorEventType _eventType = SensorEventType.Trigger;
		#pragma warning restore 414
		
		[SerializeField] private LayerMask _collisionLayers = ~0;
		
		[Tooltip("If selected, collision with bodies containing multiple colliders will only trigger a single enter/exit event, rather than an event for each collider.")]
		[SerializeField] private bool _oncePerBody = true;
		
		[Flags] public enum SensorEventType { Trigger = 1, Collision = 2 }
		
		public TriggerHandler OnEnterTrigger = new TriggerHandler();
		public TriggerHandler OnExitTrigger = new TriggerHandler();
		public CollisionHandler OnEnterCollision = new CollisionHandler();
		public CollisionHandler OnExitCollision = new CollisionHandler();
		
#if !SENSOR_STAY
		/// <summary> This event will not be invoked unless SENSOR_STAY is added to compiler constants. </summary>
#endif
		public TriggerHandler OnStayTrigger = new TriggerHandler();
#if !SENSOR_STAY
		/// <summary> This event will not be invoked unless SENSOR_STAY is added to compiler constants. </summary>
#endif
		public CollisionHandler OnStayCollision = new CollisionHandler();
		

		public ColliderSet Triggers { get; private set; }
		public ColliderSet Colliders { get; private set; }

		public int TriggerBodyCount => Triggers.BodyCount;
		public int CollisionBodyCount => Colliders.BodyCount;
		
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
				Triggers = new ColliderSet();
				Colliders = new ColliderSet();
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!enabled) return;
			if (!CollisionLayers.Contains(other.gameObject.layer)) return;
			if (OncePerBody && !Triggers.Enter(other)) return;
			OnEnterTrigger?.Invoke(this, other);
		}

		#if SENSOR_STAY
		private void OnTriggerStay(Collider other)
		{
			if (!CollisionLayers.Contains(other.gameObject.layer)) return;
			if (enabled) OnStayTrigger?.Invoke(this, other);
		}
		#endif

		private void OnTriggerExit(Collider other)
		{
			if (!enabled) return;
			if (!CollisionLayers.Contains(other.gameObject.layer)) return;
			if (OncePerBody && !Triggers.Exit(other)) return;
			OnExitTrigger?.Invoke(this, other);
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (!enabled) return;
			
#if UNITY_2021_1_OR_NEWER
			var bodyGO = collision.body ? collision.body.gameObject : collision.collider.gameObject;
#else
			var bodyGO = collision.gameObject.GetComponentInParent<Rigidbody>().gameObject;
#endif
			
			if (!CollisionLayers.Contains(bodyGO.layer)) return;
			if (!OncePerBody)
			{
				OnEnterCollision?.Invoke(this, collision);
				return;
			}

			bool invoked = false;
			foreach (var hit in collision.contacts)
			{
				if (!Colliders.Enter(hit.otherCollider)) continue;
				if (invoked) continue;
				OnEnterCollision?.Invoke(this, collision);
				invoked = true;
			}
		}

#if SENSOR_STAY
		private void OnCollisionStay(Collision collision)
		{
#if UNITY_2021_1_OR_NEWER
			var bodyGO = collision.body ? collision.body.gameObject : collision.collider.gameObject;
#else
			var bodyGO = collision.gameObject.GetComponentInParent<Rigidbody>().gameObject;
#endif
			
			if (!CollisionLayers.Contains(bodyGO.layer)) return;
			if (enabled) OnStayCollision?.Invoke(this, collision);
		}
#endif

		private void OnCollisionExit(Collision collision)
		{
			if (!enabled) return;
			
#if UNITY_2021_1_OR_NEWER
			var bodyGO = collision.body ? collision.body.gameObject : collision.collider.gameObject;
#else
			var bodyGO = collision.gameObject.GetComponentInParent<Rigidbody>().gameObject;
#endif
			if (!CollisionLayers.Contains(bodyGO.layer)) return;
			if (!OncePerBody)
			{
				OnExitCollision?.Invoke(this, collision);
				return;
			}

			bool invoked = false;
			foreach (var hit in collision.contacts)
			{
				if (!Colliders.Exit(hit.otherCollider)) continue;
				if (invoked) continue;
				OnExitCollision?.Invoke(this, collision);
				invoked = true;
			}
		}
		
	}

}