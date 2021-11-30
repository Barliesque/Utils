using UnityEngine;

namespace Barliesque.Utils
{
	
	public class SmoothVector2
	{
		private Vector2[] _samples;
		private float _sampleWeight;
		private int _sampleIndex = 0;

		public SmoothVector2(int sampleCount = 8, Vector2 initialValue = new Vector2())
		{
			_samples = new Vector2[sampleCount];
			for (int i = 0; i < sampleCount; i++) _samples[i] = initialValue;
			smoothed = initialValue;
			_sampleWeight = 1f / sampleCount;
		}

		public void AddSample(Vector2 newSample)
		{
			// Apply sample weight to the current velocity
			var sample = newSample * _sampleWeight;
			// The next element to fill has the oldest entry
			int old = (_sampleIndex + 1) % _samples.Length;
			// Add the new velocity, and remove the oldest
			smoothed += sample - _samples[old];
			// Store the new sample and move on to the next
			_samples[_sampleIndex] = sample;
			_sampleIndex = old;
		}

		public Vector2 smoothed { get; private set; }
	}
}