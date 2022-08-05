using UnityEngine;


namespace Barliesque.Utils
{
	
	public class SmoothQuaternion
	{
		private readonly Vector4[] _samples;
		private Vector4 _total;
		private int _sampleIndex;
		private bool _dirty;
		private Quaternion _smoothed;

		public SmoothQuaternion(int sampleCount = 8)
		{
			_samples = new Vector4[sampleCount];
			for (int i = 0; i < sampleCount; i++) _samples[i] = Vector4.zero;
			_smoothed = Quaternion.identity;
		}

		public SmoothQuaternion(int sampleCount, Quaternion initialValue)
		{
			_samples = new Vector4[sampleCount];
			var sample = initialValue.ToVector4();
			for (int i = 0; i < sampleCount; i++)
			{
				_samples[i] = sample;
				_total += sample;
			}
			_smoothed = initialValue;
		}
		
		
		public void AddSample(Quaternion newSample)
		{
			// Convert to Vec4 and handle wrap-around
			var sample = newSample.ToVector4();
			var dot = Vector4.Dot(sample, _samples[_sampleIndex]);
			if (dot < 0f) sample = -sample;
			
			// The next element to fill has the oldest entry
			int old = (_sampleIndex + 1) % _samples.Length;
			// Add the new value, and remove the oldest
			_total += sample;
			_total -= _samples[old];
			// Invalidate smoothed value
			_dirty = true;
			// Store the new sample and move on to the next
			_samples[_sampleIndex] = sample;
			_sampleIndex = old;
		}

		public void Reset()
		{
			_samples.Initialize();
			_smoothed = Quaternion.identity;
		}
		
		
		public Quaternion smoothed
		{
			get
			{
				// Recalculate only once after a sample has been added
				if (_dirty)
				{
					_smoothed = _total.normalized.ToQuaternion();
					_dirty = false;
				}
				return _smoothed;
			}
		}

	}
	
}