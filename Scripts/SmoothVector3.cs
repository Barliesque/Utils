using UnityEngine;

namespace Barliesque.Utils
{

	public class SmoothVector3
	{
		private readonly Vector3[] _samples;
		private readonly Vector3[] _delayedSamples;
		private readonly int _delay;
		private readonly float _sampleWeight;
		private int _sampleIndex = 0;

		public SmoothVector3(int sampleCount = 8, Vector3 initialValue = new Vector3(), int delay = 0)
		{
			_samples = new Vector3[sampleCount];
			_delayedSamples = new Vector3[delay];
			_delay = delay;
			_sampleWeight = 1f / sampleCount;
			for (int i = 0; i < sampleCount; i++) _samples[i] = initialValue * _sampleWeight;
			smoothed = initialValue;
		}

		public void AddSample(Vector3 newSample)
		{
			// Apply sample weight to the current velocity
			var sample = newSample * _sampleWeight;

			// Sample delay?
			if (_delay > 0)
			{
				// Delay the new sample and use a previously delayed sample instead
				var dInd = (_sampleIndex + 1) % _delay;
				(_delayedSamples[dInd], sample) = (sample, _delayedSamples[dInd]);
			}
			
			// The next element to fill has the oldest entry
			int old = (_sampleIndex + 1) % _samples.Length;
			// Add the new velocity, and remove the oldest
			smoothed += sample - _samples[old];
			// Store the new sample and move on to the next
			_sampleIndex = old;
			_samples[_sampleIndex] = sample;
		}

		public void Reset(Vector3 value = default)
		{
			for (int i = 0, count = _samples.Length; i < count; i++)
			{
				_samples[i] = value * _sampleWeight;
			}
			//Array.Fill(_samples, value * _sampleWeight);
			smoothed = value;
		}

		public Vector3 smoothed { get; private set; }
		public Vector3 newest => _samples[_sampleIndex];
		public Vector3 oldest => _samples[(_sampleIndex + 1) % _samples.Length];

	}

}