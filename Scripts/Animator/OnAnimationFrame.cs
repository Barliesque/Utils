using System;
using UnityEngine;


namespace Barliesque.Utils
{

	public class OnAnimationFrame : StateMachineBehaviour
	{
		[SerializeField] private int _frame;
		[SerializeField] private int _fps = 30;

		[Tooltip("Name of the target game object to send a message to--make sure it's unique!  If unassigned, the Animator GameObject is used.")]
		[SerializeField] private string _targetName;
		
		[Tooltip("The name of a method to be called")]
		[SerializeField] private string _message;

		[Tooltip(
			"If not selected, the message will be sent once, each time this animation state is entered, the first time the specified frame is reached.")]
		[SerializeField] private bool _sendOnEachLoop = true;

		[Tooltip("If the receiver of the message is on the same GameObject as the Animator, this can be deselected.")]
		[SerializeField] private bool _sendUpwards;

		[Tooltip("If no component receives the message, an error will be logged.")]
		[SerializeField] private bool _requireReceiver = true;

		[SerializeField] private ParamType _parameterType;
		[SerializeField] private bool _boolParam;
		[SerializeField] private int _intParam;
		[SerializeField] private float _floatParam;
		[SerializeField] private string _stringParam;

		public enum ParamType
		{
			None,
			Int,
			Float,
			String,
			Animator,
			Bool
		}

		private float _time;
		private bool _hasTriggered;
		private GameObject _target;

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			_time = (float)_frame / _fps;
			_hasTriggered = false;
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			// Have we reached the specified time?
			var currentTime = stateInfo.normalizedTime * stateInfo.length;
			var hasReachedTime = currentTime >= _time;

			// If looping, allow multiple invocation
			if (_sendOnEachLoop && stateInfo.loop && !hasReachedTime) _hasTriggered = false;

			if (_hasTriggered || !hasReachedTime) return;
			_hasTriggered = true;

			var options = _requireReceiver ? SendMessageOptions.RequireReceiver : SendMessageOptions.DontRequireReceiver;
			if (!_target)
			{
				_target = string.IsNullOrEmpty(_targetName) ? animator.gameObject : GameObject.Find(_targetName);
			}

			switch (_parameterType)
			{
				case ParamType.None:
					if (_sendUpwards) _target.SendMessageUpwards(_message, options);
					else _target.SendMessage(_message, options);
					break;
				case ParamType.Int:
					if (_sendUpwards) _target.SendMessageUpwards(_message, _intParam, options);
					else _target.SendMessage(_message, _intParam, options);
					break;
				case ParamType.Float:
					if (_sendUpwards) _target.SendMessageUpwards(_message, _floatParam, options);
					else _target.SendMessage(_message, _floatParam, options);
					break;
				case ParamType.String:
					if (_sendUpwards) _target.SendMessageUpwards(_message, _stringParam, options);
					else _target.SendMessage(_message, _stringParam, options);
					break;
				case ParamType.Animator:
					if (_sendUpwards) _target.SendMessageUpwards(_message, animator, options);
					else _target.SendMessage(_message, animator, options);
					break;
				case ParamType.Bool:
					if (_sendUpwards) _target.SendMessageUpwards(_message, _boolParam, options);
					else _target.SendMessage(_message, _boolParam, options);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}


		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		//{
		//    
		//}

	}
}