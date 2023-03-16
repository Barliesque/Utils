using System;
using UnityEngine;
using UnityEngine.Events;

namespace Barliesque.Utils
{

	public class AnimatorEvent : MonoBehaviour
	{
		[Flags] public enum ParamType
		{
			Void = 1, 
			Int = 2, 
			String = 4, 
			Float = 8, 
			Object = 16
		}

		[SerializeField] private ParamType _paramTypes = ParamType.Void;

		public UnityEvent OnFrameEvent;
		public UnityEvent<int> OnFrameEventInt;
		public UnityEvent<float> OnFrameEventFloat;
		public UnityEvent<string> OnFrameEventString;
		public UnityEvent<object> OnFrameEventObject;

		public void FrameEvent()
		{
			if ((_paramTypes & ParamType.Void) == 0) Debug.LogError("Unexpected FrameEvent(void) -- Void not selected in Param Types.", this); 
			else OnFrameEvent?.Invoke();
		}

		public void FrameEventInt(int value)
		{
			if ((_paramTypes & ParamType.Int) == 0) Debug.LogError("Unexpected FrameEvent(Int) -- Int not selected in Param Types.", this); 
			else OnFrameEventInt?.Invoke(value);
		}

		public void FrameEventFloat(float value)
		{
			if ((_paramTypes & ParamType.Float) == 0) Debug.LogError("Unexpected FrameEvent(Float) -- Float not selected in Param Types.", this); 
			else OnFrameEventFloat?.Invoke(value);
		}

		public void FrameEventString(string value)
		{
			if ((_paramTypes & ParamType.String) == 0) Debug.LogError("Unexpected FrameEvent(String) -- String not selected in Param Types.", this); 
			else OnFrameEventString?.Invoke(value);
		}

		public void FrameEventObject(object value)
		{
			if ((_paramTypes & ParamType.Object) == 0) Debug.LogError("Unexpected FrameEvent(Object) -- Object not selected in Param Types.", this); 
			else OnFrameEventObject?.Invoke(value);
		}
	}

}
