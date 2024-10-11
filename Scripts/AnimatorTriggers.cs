using UnityEngine;

namespace Barliesque.Utils
{
	public class AnimatorTriggers
	{
		public readonly string[] Names;
		public readonly int[] ids;
		public readonly Animator animator;
		public bool LogChanges;
		
		private int _lastIndex;

		public AnimatorTriggers(Animator animator, params string[] names)
		{
			Names = names;
			this.animator = animator;
			var count = names.Length;
			ids = new int[count];
			for(int i = 0; i < count; i++) ids[i] = Animator.StringToHash(names[i]);
		}

		public void SetNext()
		{
			Set((_lastIndex + 1) % ids.Length);
		}

		public void SetRandom()
		{
			var index = Random.Range(0, ids.Length);
			// Reduce likelihood of repeat
			if (index == _lastIndex) index = Random.Range(0, ids.Length);
			Set(index);
		}
		
		public void Set(int index)
		{
			if (LogChanges) Debug.Log($"{Names[index]}.Set()");
			animator.SetTrigger(ids[index]);
			_lastIndex = index;
		}

		public void Reset()
		{
			if (LogChanges) Debug.Log($"({string.Join(',', Names)}).Reset()");
			foreach (var id in ids) animator.ResetTrigger(id);
			_lastIndex = -1;
		}

		override public string ToString()
		{
			return $"[AnimatorTriggers] {string.Join(',', Names)}";
		}
	}
}