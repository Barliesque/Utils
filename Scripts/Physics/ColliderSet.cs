using System.Collections.Generic;
using UnityEngine;

namespace Barliesque.Utils
{
	/// <summary>
	/// A class to manage the entry and exit of colliders belonging to a Rigidbody, so that only one
	/// enter event and one exit event is triggered for a body that comprises multiple colliders.
	/// </summary>
	public class ColliderSet
	{
		private Dictionary<Rigidbody, int> _colliderCount = new Dictionary<Rigidbody, int>();
		private Dictionary<Collider, BodyData> _bodies = new Dictionary<Collider, BodyData>();


		private struct BodyData
		{
			public Rigidbody Body;
			public bool IsKinematic;

			public BodyData(Rigidbody body)
			{
				Body = body;
				IsKinematic = body.isKinematic;
			}

			public BodyData UpdateKinematic()
			{
				IsKinematic = Body.isKinematic;
				return this;
			}

			public bool KinematicMismatch => IsKinematic != Body.isKinematic;
		}


		/// <summary>
		/// To be called from OnTriggerEnter() or OnCollisionEnter().
		/// If a Rigidbody has just entered the trigger, it will be returned.  Otherwise, null is returned.
		/// </summary>
		/// <param name="other">The collider that has just entered the trigger area, provided by OnTriggerEnter()</param>
		/// <returns>When a Rigidbody first enters the trigger, it will be returned.  Otherwise, null is returned.</returns>
		public Rigidbody Enter(Collider other)
		{
#if UNITY_2019_4_OR_NEWER
			var body = other.attachedRigidbody;
#else
			var body = other.GetComponentInParent<Rigidbody>();
#endif
			if (!body) return null;

			if (!_bodies.ContainsKey(other))
			{
				_bodies.Add(other, new BodyData(body));
			}
			else
			{
				var data = _bodies[other];
				if (data.KinematicMismatch)
				{
					_bodies[other] = data.UpdateKinematic();
				}

				return null;
			}

			if (_colliderCount.ContainsKey(body))
			{
				_colliderCount[body]++;
				return null;
			}
			else
			{
				_colliderCount.Add(body, 1);
				return body;
			}
		}


		/// <summary>
		/// To be called from OnTriggerExit() or OnCollisionEnter().
		/// If a Rigidbody has completely exited the trigger, it will be returned.  Otherwise, null is returned.
		/// </summary>
		/// <param name="other">The collider that has just exited the trigger area, provided by OnTriggerExit()</param>
		/// <returns>If a Rigidbody has completely exited the trigger, it will be returned.   Otherwise, null is returned.</returns>
		public Rigidbody Exit(Collider other)
		{
			if (!_bodies.ContainsKey(other)) return null;
			var data = _bodies[other];
			if (data.KinematicMismatch)
			{
				_bodies[other] = data.UpdateKinematic();
#if UNITY_2019_3_OR_NEWER
				// As of Unity 2019.3 a change of Rigidbody.isKinematic triggers exit and entry events.
				// So, having detected a changed kinematic state, we update our data and then ignore this event.
				return null;
#endif
			}

			var body = data.Body;
			_bodies.Remove(other);

			if (--_colliderCount[body] == 0)
			{
				_colliderCount.Remove(body);
				return body;
			}

			return null;
		}

		public int BodyCount => _colliderCount.Count;

		public bool ContainsBody(Rigidbody body)
		{
			return _colliderCount.ContainsKey(body);
		}

		public bool ContainsTag(string tag)
		{
			foreach (var item in _bodies)
			{
				if (item.Key.CompareTag(tag)) return true;
			}

			return false;
		}

		public void Clear()
		{
			_colliderCount.Clear();
			_bodies.Clear();
		}

		public List<Rigidbody> GetBodies()
		{
			var result = new List<Rigidbody>();
			foreach (var item in _colliderCount)
			{
				if (item.Value > 0) result.Add(item.Key);
			}
			return result;
		}

		public Rigidbody GetAnyBody()
		{
			foreach (var item in _colliderCount)
			{
				if (item.Value > 0) return item.Key;
			}
			return null;
		}
		
	}
}