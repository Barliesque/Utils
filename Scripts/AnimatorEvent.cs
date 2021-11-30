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

#pragma warning disable 414  /// Variable is used only by the Editor
		[SerializeField] private ParamType _paramTypes = ParamType.Void;
#pragma warning restore 414

		public UnityEvent OnFrameEvent;
		public UnityEvent<int> OnFrameEventInt;
		public UnityEvent<float> OnFrameEventFloat;
		public UnityEvent<string> OnFrameEventString;
		public UnityEvent<object> OnFrameEventObject;

		public void FrameEvent() => OnFrameEvent?.Invoke();
		public void FrameEventInt(int value) => OnFrameEventInt?.Invoke(value);
		public void FrameEventFloat(float value) => OnFrameEventFloat?.Invoke(value);
		public void FrameEventString(string value) => OnFrameEventString?.Invoke(value);
		public void FrameEventObject(object value) => OnFrameEventObject?.Invoke(value);
	}

}
