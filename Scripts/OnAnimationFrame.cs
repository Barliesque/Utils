using System;
using UnityEngine;


public class OnAnimationFrame : StateMachineBehaviour
{
	[SerializeField] private int _frame;
	[SerializeField] private int _fps = 30;
	
	[Tooltip("The name of a method to be called")]
	[SerializeField] private string _message;
	[Tooltip("If not selected, the message will be sent once, each time this animation state is entered, the first time the specified frame is reached.")]
	[SerializeField] private bool _sendOnEachLoop = true;
	[Tooltip("If the receiver of the message is on the same GameObject as the Animator, this can be deselected.")]
	[SerializeField] private bool _sendUpwards;
	[Tooltip("If no component receives the message, an error will be logged.")]
	[SerializeField] private bool _requireReceiver = true;
	
	[SerializeField] private ParamType _parameterType;
	[SerializeField] private int _intParam;
	[SerializeField] private float _floatParam;
	[SerializeField] private string _stringParam;

	public enum ParamType
	{
		None, Int, Float, String, Animator
	}
	
	private float _time;
	private bool _hasTriggered;

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
		
		switch (_parameterType)
		{
			case ParamType.None:
				if (_sendUpwards) animator.gameObject.SendMessageUpwards(_message, options);
				else animator.gameObject.SendMessage(_message, options);
				break;
			case ParamType.Int:
				if (_sendUpwards) animator.gameObject.SendMessageUpwards(_message, _intParam, options);
				else animator.gameObject.SendMessage(_message, _intParam, options);
				break;
			case ParamType.Float:
				if (_sendUpwards) animator.gameObject.SendMessageUpwards(_message, _floatParam, options);
				else animator.gameObject.SendMessage(_message, _floatParam, options);
				break;
			case ParamType.String:
				if (_sendUpwards) animator.gameObject.SendMessageUpwards(_message, _stringParam, options);
				else animator.gameObject.SendMessage(_message, _stringParam, options);
				break;
			case ParamType.Animator:
				if (_sendUpwards) animator.gameObject.SendMessageUpwards(_message, animator, options);
				else animator.gameObject.SendMessage(_message, animator, options);
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

	// OnStateMove is called right after Animator.OnAnimatorMove()
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	//{
	//    // Implement code that processes and affects root motion
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK()
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	//{
	//    // Implement code that sets up animation IK (inverse kinematics)
	//}
}