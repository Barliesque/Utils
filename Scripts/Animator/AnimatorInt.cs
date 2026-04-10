using System;
using UnityEngine;

namespace Barliesque.Utils
{
	
	[Serializable]
	public class AnimatorInt
	{
		public readonly string Name;
		public readonly int id;
		public readonly Animator animator;
		public bool LogChanges;

		[SerializeField] private int _value;

		public int Value
		{
			get { return _value; }
			set
			{
				_value = value;
				animator.SetInteger(id, _value);
				if (LogChanges) Debug.Log($"{Name} = {value}");
			}
		}

		public AnimatorInt(Animator animator, string name)
		{
			Name = name;
			this.animator = animator;
			id = Animator.StringToHash(name);
		}
		public AnimatorInt(Animator animator, string name, int defaultValue = 0, bool logChanges = false)
		{
			Name = name;
			this.animator = animator;
			id = Animator.StringToHash(name);
			LogChanges = logChanges;
			Value = defaultValue;
		}

		static public implicit operator int(AnimatorInt parameter)
		{
			return parameter._value;
		}

		override public string ToString()
		{
			return string.Format("{0} = {1}", Name, _value);
		}
	}
}