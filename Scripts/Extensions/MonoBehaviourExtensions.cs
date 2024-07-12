using System.Collections.Generic;
using MEC;
using UnityEngine;


namespace Barliesque.Utils
{

	static public class MonoBehaviourExtensions
	{
		public delegate void Callback(float t);

		static public CoroutineHandle Play(this MonoBehaviour owner, float duration, Callback callback) => Play(owner, duration, true, callback);
		static public CoroutineHandle Play(this MonoBehaviour owner, float duration, bool cancelOnDisable, Callback callback)
		{
			if (!cancelOnDisable) return Timing.RunCoroutine(_PlayFunction(duration, callback, true));
			if (!owner.isActiveAndEnabled) return new CoroutineHandle();
			return Timing.RunCoroutine(_PlayFunction(duration, callback, true).CancelWith(owner));
		}

		static public CoroutineHandle PlayUnscaled(this MonoBehaviour owner, float duration, Callback callback) => PlayUnscaled(owner, duration, true, callback);
		static public CoroutineHandle PlayUnscaled(this MonoBehaviour owner, float duration, bool cancelOnDisable, Callback callback)
		{
			if (!cancelOnDisable) return Timing.RunCoroutine(_PlayFunction(duration, callback, false));
			if (!owner.isActiveAndEnabled) return new CoroutineHandle();
			return Timing.RunCoroutine(_PlayFunction(duration, callback, false).CancelWith(owner));
		}

		static private IEnumerator<float> _PlayFunction(float duration, Callback callback, bool scaledTime)
		{
			var timer = 0f;
			do
			{
				timer += scaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
				callback(Mathf.Clamp01(timer / duration));
				yield return Timing.WaitForOneFrame;
			} while (timer < duration);
		}
		

		public delegate void Callback<T>(float t, T value);

		static public CoroutineHandle Play<T>(this MonoBehaviour owner, float duration, Callback<T> callback, T value) => Play(owner, duration, true, callback, value);
		static public CoroutineHandle Play<T>(this MonoBehaviour owner, float duration, bool cancelOnDisable, Callback<T> callback, T value)
		{
			if (!cancelOnDisable) return Timing.RunCoroutine(_PlayFunction(duration, callback, value, true));
			if (!owner.isActiveAndEnabled) return new CoroutineHandle();
			return Timing.RunCoroutine(_PlayFunction(duration, callback, value, true).CancelWith(owner));
		}

		static public CoroutineHandle PlayUnscaled<T>(this MonoBehaviour owner, float duration, Callback<T> callback, T value) => PlayUnscaled(owner, duration, true, callback, value);
		static public CoroutineHandle PlayUnscaled<T>(this MonoBehaviour owner, float duration, bool cancelOnDisable, Callback<T> callback, T value)
		{
			if (!cancelOnDisable) return Timing.RunCoroutine(_PlayFunction(duration, callback, value, false));
			if (!owner.isActiveAndEnabled) return new CoroutineHandle();
			return Timing.RunCoroutine(_PlayFunction(duration, callback, value, false).CancelWith(owner));
		}

		static private IEnumerator<float> _PlayFunction<T>(float duration, Callback<T> callback, T value, bool scaledTime)
		{
			var timer = 0f;
			do
			{
				timer += scaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
				callback(Mathf.Clamp01(timer / duration), value);
				yield return Timing.WaitForOneFrame;
			} while (timer < duration);
		}
		
		
		public delegate void Callback<in T, in U>(float t, T value1, U value2);

		static public CoroutineHandle Play<T, U>(this MonoBehaviour owner, float duration, Callback<T, U> callback, T value1, U value2) => Play(owner, duration, true, callback, value1, value2);
		static public CoroutineHandle Play<T,U>(this MonoBehaviour owner, float duration, bool cancelOnDisable, Callback<T,U> callback, T value1, U value2)
		{
			if (!cancelOnDisable) return Timing.RunCoroutine(_PlayFunction(duration, callback, value1, value2, true));
			if (!owner.isActiveAndEnabled) return new CoroutineHandle();
			return Timing.RunCoroutine(_PlayFunction(duration, callback, value1, value2, true).CancelWith(owner));
		}
		
		static public CoroutineHandle PlayUnscaled<T,U>(this MonoBehaviour owner, float duration, Callback<T,U> callback, T value1, U value2) => PlayUnscaled(owner, duration, true, callback, value1, value2);
		static public CoroutineHandle PlayUnscaled<T,U>(this MonoBehaviour owner, float duration, bool cancelOnDisable, Callback<T,U> callback, T value1, U value2)
		{
			if (!cancelOnDisable) return Timing.RunCoroutine(_PlayFunction(duration, callback, value1, value2, false));
			if (!owner.isActiveAndEnabled) return new CoroutineHandle();
			return Timing.RunCoroutine(_PlayFunction(duration, callback, value1, value2, false).CancelWith(owner));
		}

		static private IEnumerator<float> _PlayFunction<T,U>(float duration, Callback<T,U> callback, T value1, U value2, bool scaledTime)
		{
			var timer = 0f;
			do
			{
				timer += scaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
				callback(Mathf.Clamp01(timer / duration), value1, value2);
				yield return Timing.WaitForOneFrame;
			} while (timer < duration);
		}
		
	}

}