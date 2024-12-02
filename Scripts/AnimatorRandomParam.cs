using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Barliesque.Utils
{
	public class AnimatorRandomParam : MonoBehaviour
	{
		[SerializeField] private string _parameter;
		[SerializeField] private AnimatorControllerParameterType _type;
		[SerializeField] private LinearLimits _floatRange;
		[SerializeField] private IntRange _intRange;

		[Tooltip("Should a new random value be applied each time this object is enabled?")]
		[SerializeField] private bool _randomizeOnEnable = true;
		

		private void Start()
		{
			Randomize();
		}

		private void OnEnable()
		{
			if (_randomizeOnEnable) Randomize();
		}

		public void Randomize()
		{
			var animator = GetComponent<Animator>();

			switch (_type)
			{
				case AnimatorControllerParameterType.Bool:
					animator.SetBool(_parameter, Random.value > 0.5f);
					break;
				case AnimatorControllerParameterType.Trigger:
					if (Random.value > 0.5f) animator.SetTrigger(_parameter);
					break;
				case AnimatorControllerParameterType.Float:
					animator.SetFloat(_parameter, _floatRange.Random());
					break;
				case AnimatorControllerParameterType.Int:
					animator.SetInteger(_parameter, _intRange.Random());
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}