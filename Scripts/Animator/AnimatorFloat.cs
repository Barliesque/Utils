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
			return $"{Name} = {_value}";
		}
		
		public bool CheckParameterExists() => CheckParameterExists(animator, Name);

		static public bool CheckParameterExists(Animator animator, string floatParam)
		{
			if (!animator) return false;
			if (string.IsNullOrEmpty(floatParam)) return false;
			
			animator.Update(0f); // Forces parameters list to be updated
			foreach (var param in animator.parameters)
			{
				if (!param.name.Equals(floatParam)) continue;
				// It exists, but is it a float?
				return param.type == AnimatorControllerParameterType.Float;
			}
			return false;
		}
		
		
	}
}