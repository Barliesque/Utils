using System;
using UnityEngine;

namespace Barliesque.Utils
{
	[Serializable]
	public class AnimatorFloat
	{
		public readonly string Name;
		public readonly int id;
		public readonly Animator animator;
		public bool LogChanges;

		[SerializeField] private float _value;

		public float Value
		{
			get { return _value; }
			set
			{
				_value = value;
				animator.SetFloat(id, _value);
				if (LogChanges) Debug.Log($"{Name} = {value}");
			}
		}

		public AnimatorFloat(Animator animator, string name)
		{
			Name = name;
			this.animator = animator;
			id = Animator.StringToHash(name);
		}
		public AnimatorFloat(Animator animator, string name, float defaultValue = 0f, bool logChanges = false)
		{
			Name = name;
			this.animator = animator;
			id = Animator.StringToHash(name);
			LogChanges = logChanges;
			Value = defaultValue;
		}

		static public implicit operator float(AnimatorFloat parameter)
		{
			return parameter._value;
		}

		override public string ToString()
		{
			return string.Format("{0} = {1}", Name, _value);
		}
	}
}