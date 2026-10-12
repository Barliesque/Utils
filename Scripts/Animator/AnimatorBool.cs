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
			get => _value;
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
			return $"{Name} = {(_value ? "true" : "false")}";
		}

		public bool CheckParameterExists() => CheckParameterExists(animator, Name);

		static public bool CheckParameterExists(Animator animator, string boolParam)
		{
			if (!animator) return false;
			if (string.IsNullOrEmpty(boolParam)) return false;
			
			animator.Update(0f); // Forces parameters list to be updated
			foreach (var param in animator.parameters)
			{
				if (!param.name.Equals(boolParam)) continue;
				// It exists, but is it a bool?
				return param.type == AnimatorControllerParameterType.Bool;
			}
			return false;
		}
		
	}
}