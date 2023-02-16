using System.Collections;
using UnityEngine;


namespace Barliesque.Utils
{

	static public class MonoBehaviourExtensions
	{
		public delegate void Callback(float t);
		
		static public Coroutine Play(this MonoBehaviour owner, float duration, Callback callback, bool scaledTime = false)
		{
			return owner.StartCoroutine(PlayFunction(duration, callback, scaledTime));
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

		static public Coroutine Play<T>(this MonoBehaviour owner, float duration, Callback<T> callback, T value, bool scaledTime = false)
		{
			return owner.StartCoroutine(PlayFunction(duration, callback, value, scaledTime));
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
		
		static public Coroutine Play<T,U>(this MonoBehaviour owner, float duration, Callback<T,U> callback, T value1, U value2, bool scaledTime = false)
		{
			return owner.StartCoroutine(PlayFunction(duration, callback, value1, value2, scaledTime));
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
		
		
		public delegate void Callback<in T, in U, in V>(float t, T value1, U value2, V value3);
		
		static public Coroutine Play<T,U,V>(this MonoBehaviour owner, float duration, Callback<T,U,V> callback, T value1, U value2, V value3, bool scaledTime = false)
		{
			return owner.StartCoroutine(PlayFunction(duration, callback, value1, value2, value3, scaledTime));
		}

		static private IEnumerator PlayFunction<T,U,V>(float duration, Callback<T,U,V> callback, T value1, U value2, V value3, bool scaledTime)
		{
			var timer = 0f;
			do
			{
				timer += scaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
				callback(Mathf.Clamp01(timer / duration), value1, value2, value3);
				yield return null;
			} while (timer < duration);
		}
		
		
	}

}