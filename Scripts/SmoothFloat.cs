
using System;

namespace Barliesque.Utils
{

	public class SmoothFloat
	{
		private float[] _samples;
		private float _sampleWeight;
		private int _sampleIndex = 0;

		public SmoothFloat(int sampleCount = 8, float initialValue = 0f)
		{
			_samples = new float[sampleCount];
			_sampleWeight = 1f / sampleCount;
			for (int i = 0; i < sampleCount; i++) _samples[i] = initialValue * _sampleWeight;
			smoothed = initialValue;
		}

		public void AddSample(float newSample)
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

		public void Reset(float value = default)
		{
			Array.Fill(_samples, value * _sampleWeight);
			smoothed = value;
		}

		public float smoothed { get; private set; }

	}

}