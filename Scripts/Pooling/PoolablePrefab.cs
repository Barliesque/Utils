using System.Collections;
using UnityEngine;

namespace Barliesque.Utils
{
	public class PoolablePrefab : MonoBehaviour
	{
		[Tooltip("Total number of instances to pre-instantiate.")]
		[SerializeField] private int _prewarm = 8;
		[Tooltip("Maximum number of instances allowed to be in the pool.")]
		[SerializeField] private int _maxInstances = 16;
		[Tooltip("How will instances be recycled?")]
		[SerializeField] private Recycling _recycling;
		[SerializeField] private float _recycleAfter = 2f;

		public enum Recycling
		{
			Callback,
			Timed,
			OnDisable
		}

		internal PrefabPool _manager;
		internal Transform _pool;

		public int Prewarm => _prewarm;
		public int MaxInstances => _maxInstances;
		
		new public Transform transform { get; private set; }
		

		private void Awake()
		{
			transform = GetComponent<Transform>();
		}

		private void OnEnable()
		{
			if (_recycling == Recycling.Timed)
			{
				StartCoroutine(TimedRecycle());
			}
		}

		private void OnDisable()
		{
			if (_recycling is Recycling.Timed or Recycling.OnDisable) StopAllCoroutines();
			if (_recycling == Recycling.OnDisable) RecycleNextFrame();
		}
		
		private IEnumerator TimedRecycle()
		{
			yield return new WaitForSeconds(_recycleAfter);
			Recycle();
		}

		public void Recycle()
		{
			PrefabPool.Recycle(this);
		}

		public void RecycleNextFrame()
		{
			PrefabPool.RecycleNextFrame(this);
		}
		
	}
}