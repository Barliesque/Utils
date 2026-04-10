using System;


namespace Barliesque.Utils
{
	/// <summary>
	/// A fixed-length array that is always kept in sorted order.
	/// </summary>
	public class SortedArray<T>
	{
		private readonly int _capacity;
		private readonly T[] _array;
		private Comparison<T> _comparer;

		public SortedArray(int capacity, Comparison<T> comparer, T defaultValue = default(T))
		{
			_capacity = capacity;
			_array = new T[capacity];
			for (int i = 0; i < capacity; i++) _array[i] = defaultValue;
			_comparer = comparer;
		}

		public T this[int key] => _array[key];

		public int Length => _capacity;


		public bool TryInsert(T value)
		{
			bool inserted = false;
			var other = value;
			for (int i = 0; i < _capacity; i++)
			{
				var item = _array[i];
				if (!inserted)
				{
					if (_comparer.Invoke(item, other) <= 0) continue;
					inserted = true;
				}

				_array[i] = other;
				other = item;
			}

			return inserted;
		}

		
		/// <summary>
		/// Adds a value to the array, and returns whatever value has been pushed off the list.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public T Insert(T value)
		{
			bool inserted = false;
			var other = value;
			for (int i = 0; i < _capacity; i++)
			{
				var item = _array[i];
				if (!inserted)
				{
					if (_comparer.Invoke(item, other) <= 0) continue;
					inserted = true;
				}

				_array[i] = other;
				other = item;
			}

			return other;
		}

		

		public int FindIndex(Predicate<T> match)
		{
			for (int i = 0; i < _capacity; i++)
			{
				if (match(_array[i])) return i;
			}

			return -1;
		}

		public bool Find(Predicate<T> match, out T found)
		{
			for (int i = 0; i < _capacity; i++)
			{
				if (!match(_array[i])) continue;
				found = _array[i];
				return true;
			}

			found = default(T);
			return false;
		}
		
	}
}