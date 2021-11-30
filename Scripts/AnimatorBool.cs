using System;
using UnityEngine;

namespace Barliesque.Utils
{
	
	[Serializable]
	public class AnimatorBool
	{
		public readonly string Name;
		public readonly int id;
		public readonly Animator animator;
		public bool LogChanges;

		[SerializeField] private bool _value;

		public bool Value
		{
			get { return _value; }
			set
			{
				_value = value;
				animator.SetBool(id, _value);
				if (LogChanges) Debug.Log($"{Name} = {value}");
			}
		}

		public AnimatorBool(Animator animator, string name)
		{
			Name = name;
			this.animator = animator;
			id = Animator.StringToHash(name);
		}
		
		public AnimatorBool(Animator animator, string name, bool defaultValue, bool logChanges = false)
		{
			Name = name;
			this.animator = animator;
			id = Animator.StringToHash(name);
			LogChanges = logChanges;
			Value = defaultValue;
		}

		static public implicit operator bool(AnimatorBool parameter)
		{
			return parameter._value;
		}

		override public string ToString()
		{
			return string.Format("{0} = {1}", Name, _value ? "true" : "false");
		}
	}
}