using System;
using System.Collections;
using UnityEngine;


namespace Barliesque.Utils
{

	static public class MonoBehaviourExtensions
	{
		
		static public Coroutine Play(this MonoBehaviour owner, float duration, Action<float> callback, bool scaledTime = false)
		{
			return owner.StartCoroutine(PlayFunction(duration, callback, scaledTime));
		}

		static private IEnumerator PlayFunction(float duration, Action<float> callback, bool scaledTime)
		{
			var timer = 0f;
			do
			{
				timer += scaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
				callback(Mathf.Clamp01(timer / duration));
				yield return null;
			} while (timer < duration);
		}
		
		
	}

}