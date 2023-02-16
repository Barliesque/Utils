using System;
using System.Collections;
using Barliesque.InspectorTools;
using UnityEngine;

namespace Barliesque.Utils
{
	
	public class PrefabPool : MonoBehaviour
	{
		static private PrefabPool _inst;

		[SerializeField, Prefab] private PoolablePrefab[] _prefabs;

		private Transform _xform;
		private int[] _counts;
		private int[] _maxActive;

#if UNITY_EDITOR
		public int GetMaxCount(int index) => _prefabs[index].MaxInstances;
		public int GetCount(int index) => _counts?[index] ?? 0;
		public int GetPooledCount(int index) => (_xform.childCount <= index) ? 0 : _xform.GetChild(index)?.childCount ?? 0;
		public int GetActiveCount(int index) => (_xform.childCount <= index) ? 0 : (_counts?[index] ?? 0) - (_xform.GetChild(index)?.childCount ?? 0);
		public int GetMaxActive(int index) => _maxActive?[index] ?? 0;
#endif
		

		private void Awake()
		{
			_inst = this;
			_xform = GetComponent<Transform>();
			_counts = new int[_prefabs.Length];
			_maxActive = new int[_prefabs.Length];
			
			for (int i = 0; i < _prefabs.Length; i++)
			{
				var prefab = _prefabs[i];
				if (!prefab) continue;
				var prefabName = prefab.name;
				var pool = new GameObject(prefabName);
				pool.SetActive(false);
				var xform = pool.GetComponent<Transform>();
				xform.SetParent(_xform);

				for (int j = 0; j < prefab.Prewarm; j++)
				{
					InstantiateItem(i, xform);
				}
			}
		}

		/// <summary>
		/// Call PrefabPool.GetInstance() not Instantiate()
		/// </summary>
		/// <exception cref="Exception">This call will throw an Exception!</exception>
		[Obsolete] new static public T Instantiate<T>(T original, Transform parent)
		{
			throw new Exception("Call PrefabPool.GetInstance() not Instantiate()");
		}
		
		private PoolablePrefab InstantiateItem(int index, Transform pool)
		{
			var prefab = _prefabs[index];
			var item = GameObject.Instantiate(prefab, pool);
			item.name = $"{prefab.name} ({++_counts[index]})";
			item._pool = pool;
			return item;
		}


		private Transform FindPool(UnityEngine.Object prefab, out int index)
		{
			for (index = 0; index < _xform.childCount; index++)
			{
				var pool = _xform.GetChild(index);
				if (pool.name == prefab.name) return pool;
			}

			throw new Exception($"No pool exists for prefab: {prefab.name}");
		}

		static public T GetInstance<T>(T prefab) where T : UnityEngine.Object => GetInstance(prefab, Vector3.zero);

		static public T GetInstance<T>(T prefab, Vector3 position, Transform anchor = null) where T : UnityEngine.Object
		{
			if (!_inst) return null;
			
			var pool = _inst.FindPool(prefab, out int index);
			PoolablePrefab item;
			Transform xform;
			if (pool.childCount == 0)
			{
				item = _inst.InstantiateItem(index, pool);
				xform = item.GetComponent<Transform>();
			}
			else
			{
				xform = pool.GetChild(0);
				item = xform.GetComponent<PoolablePrefab>();
			}
			xform.SetParent(anchor);
			xform.position = position;
			
			#if UNITY_EDITOR
			var active = _inst.GetActiveCount(index);
			if (active > _inst._maxActive[index]) _inst._maxActive[index] = active;
			#endif

			return item as T ?? item.GetComponent<T>();
		}
		

		static internal void Recycle(PoolablePrefab item)
		{
			var pool = item._pool;
			var xform = item.GetComponent<Transform>();
			if (pool.childCount >= item.MaxInstances)
			{
				_inst._counts[pool.GetSiblingIndex()]--;
				Destroy(item.gameObject);
			}
			else
			{
				xform.SetParent(pool);
			}
		}

		static public void RecycleNextFrame(PoolablePrefab instance)
		{
			_inst.StopAllCoroutines();
			_inst.StartCoroutine(CRRecycleNextFrame(instance));
		}

		static private IEnumerator CRRecycleNextFrame(PoolablePrefab instance)
		{
			yield return null;
			Recycle(instance);
		}
	}
}