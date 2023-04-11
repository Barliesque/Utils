using System.Collections;
using UnityEngine;


namespace Barliesque.Utils
{

	static public class MonoBehaviourExtensions
	{
		public delegate void Callback(float t);
		
		static public Coroutine Play(this MonoBehaviour owner, float duration, Callback callback)
		{
			return owner.StartCoroutine(PlayFunction(duration, callback, true));
		}
		
		static public Coroutine PlayUnscaled(this MonoBehaviour owner, float duration, Callback callback)
		{
			return owner.StartCoroutine(PlayFunction(duration, callback, false));
		}

		static private IEnumerator PlayFunction(float duration, Callback callback, bool scaledTime)
		{
			var timer = 0f;
			do
			{
				timer += scaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
				callback(Mathf.Clamp01(timer / duration));
				yield return null;
			} while (timer < duration);
		}
		

		public delegate void Callback<T>(float t, T value);

		static public Coroutine Play<T>(this MonoBehaviour owner, float duration, Callback<T> callback, T value)
		{
			return owner.StartCoroutine(PlayFunction(duration, callback, value, true));
		}

		static public Coroutine PlayUnscaled<T>(this MonoBehaviour owner, float duration, Callback<T> callback, T value)
		{
			return owner.StartCoroutine(PlayFunction(duration, callback, value, false));
		}

		static private IEnumerator PlayFunction<T>(float duration, Callback<T> callback, T value, bool scaledTime)
		{
			var timer = 0f;
			do
			{
				timer += scaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
				callback(Mathf.Clamp01(timer / duration), value);
				yield return null;
			} while (timer < duration);
		}
		
		
		public delegate void Callback<in T, in U>(float t, T value1, U value2);
		
		static public Coroutine Play<T,U>(this MonoBehaviour owner, float duration, Callback<T,U> callback, T value1, U value2)
		{
			return owner.StartCoroutine(PlayFunction(duration, callback, value1, value2, true));
		}
		
		static public Coroutine PlayUnscaled<T,U>(this MonoBehaviour owner, float duration, Callback<T,U> callback, T value1, U value2)
		{
			return owner.StartCoroutine(PlayFunction(duration, callback, value1, value2, false));
		}

		static private IEnumerator PlayFunction<T,U>(float duration, Callback<T,U> callback, T value1, U value2, bool scaledTime)
		{
			var timer = 0f;
			do
			{
				timer += scaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
				callback(Mathf.Clamp01(timer / duration), value1, value2);
				yield return null;
			} while (timer < duration);
		}
		
		
	}

}