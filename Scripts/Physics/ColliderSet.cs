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
		private readonly Dictionary<Rigidbody, int> _colliderCount = new ();
		private readonly Dictionary<Collider, BodyData> _bodies = new ();


		private struct BodyData
		{
			public readonly Rigidbody Body;
			private bool _isKinematic;

			public BodyData(Rigidbody body)
			{
				Body = body;
				_isKinematic = body.isKinematic;
			}

			public BodyData UpdateKinematic()
			{
				_isKinematic = Body.isKinematic;
				return this;
			}

			public bool KinematicMismatch => _isKinematic != Body.isKinematic;
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

			if (_colliderCount.TryAdd(body, 1)) return body;
			_colliderCount[body]++;
			return null;
		}


		/// <summary>
		/// To be called from OnTriggerExit() or OnCollisionEnter().
		/// If a Rigidbody has completely exited the trigger, it will be returned.  Otherwise, null is returned.
		/// </summary>
		/// <param name="other">The collider that has just exited the trigger area, provided by OnTriggerExit()</param>
		/// <returns>If a Rigidbody has completely exited the trigger, it will be returned.   Otherwise, null is returned.</returns>
		public Rigidbody Exit(Collider other)
		{
			if (!_bodies.TryGetValue(other, out var data)) return null;
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
			foreach (var col in _bodies.Keys)
			{
				if (col.CompareTag(tag)) return true;
			}

			return false;
		}

		public void Clear()
		{
			_colliderCount.Clear();
			_bodies.Clear();
		}

		/// <summary>
		/// Get a list of rigidbodies in the collection.
		/// </summary>
		/// <returns></returns>
		public List<Rigidbody> GetBodies()
		{
			var result = new List<Rigidbody>();
			foreach (var (body, count) in _colliderCount)
			{
				if (count > 0) result.Add(body);
			}
			return result;
		}

		/// <summary>
		/// Append rigidbodies in the collection to an existing list.
		/// </summary>
		/// <param name="bodies">The specified List will not be cleared.  All bodies in the collection will simply be added.</param>
		/// <returns></returns>
		public void GetBodies(List<Rigidbody> bodies)
		{
			foreach (var (body, count) in _colliderCount)
			{
				if (count > 0) bodies.Add(body);
			}
		}

		public Rigidbody GetAnyBody()
		{
			foreach (var (body, count) in _colliderCount)
			{
				if (count > 0) return body;
			}
			return null;
		}
		
		private readonly List<Collider> _stale = new();

		/// <summary> Remove colliders that were disabled, deactivated or destroyed without an exit event. </summary>
		public void PruneInactive(System.Action<Collider> onBodyExited = null)
		{
			_stale.Clear();
			foreach (var col in _bodies.Keys)
			{
				if (!col || !col.enabled || !col.gameObject.activeInHierarchy) _stale.Add(col);
			}

			for (int i = 0, len = _stale.Count; i < len; i++)
			{
				var col = _stale[i];
				var body = _bodies[col].Body;
				_bodies.Remove(col);
				if (!_colliderCount.TryGetValue(body, out var count)) continue;
				if (--count > 0)
				{
					_colliderCount[body] = count;
					continue;
				}

				_colliderCount.Remove(body);
				if (col) onBodyExited?.Invoke(col);
			}
		}
	}
}