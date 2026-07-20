using System;
using UnityEngine;


namespace Barliesque.Utils
{
	
	//TODO  Add options to allow for hold-to-repeat:  Enabled, InitialTime, RepeatTime
	
	[Serializable]  // Enables the Inspector panel in debug mode to monitor state values
	public class ButtonState
	{
		private float _analog;
		private bool _wasActive;
		private float _threshold;
		private float _heldTime;
		private int _lastUpdated = -1;
		private int _heldFrames;
		private float _deadZone;

		
		/// <summary>
		/// To enable logging, set this string which will be used as a prefix when logging.
		/// </summary>
		public string LogChanges = null;


		/// <summary>
		/// Create a ButtonState to track the current state of a controller button
		/// </summary>
		/// <param name="threshold">The minimum analog value required for the button to be interpreted as active.</param>
		/// <param name="deadZone">Input values less than this will be stored as zero, to eliminate noise.</param>
		public ButtonState(float threshold = 0.5f, float deadZone = 1e-4f)
		{
			_threshold = threshold;
			_deadZone = deadZone;
		}

		/// <summary>
		/// The minimum number of frames before a button state change should be registered.
		/// Increasing this value may help to neutralize erratic button inputs.
		/// </summary>
		public int MinHold = 0;


		/// <summary>
		/// To be called once every frame to update the current state of the button.
		/// </summary>
		/// <param name="analog">The analog value of the button, from 0.0 to 1.0</param>
		public void Update(float analog)
		{
			// Do not allow _wasActive to be changed multiple times per frame
			var newFrame = _lastUpdated != Time.frameCount; 
			if (newFrame)
			{
				_wasActive = IsActive;
				_lastUpdated = Time.frameCount;
				_heldTime += Time.unscaledDeltaTime;
				++_heldFrames;
			}
			
			// Note:  This will change the value of IsActive
			// Apply a dead zone to avoid small noise values
			_analog = (Mathf.Abs(analog) <= _deadZone) ? 0f : analog;

			if (_wasActive != IsActive)
			{
				_heldTime = 0f;
				_heldFrames = 0;
			}
			
			if (!string.IsNullOrEmpty(LogChanges) && IsActive != _wasActive)
			{
				Debug.Log($"ButtonState [{LogChanges}] changed to {IsActive}");
			}
		}

		
		/// <summary>
		/// To be called once every frame to update the current state of the button.
		/// </summary>
		/// <param name="isActive">The current state of the button.</param>
		public void Update(bool isActive)
		{
			// Do not allow _wasActive to be changed multiple times per frame
			var newFrame = _lastUpdated != Time.frameCount; 
			if (newFrame)
			{
				_wasActive = IsActive;
				_lastUpdated = Time.frameCount;
				if (_wasActive) _heldTime += Time.unscaledDeltaTime;
				++_heldFrames;
			}
			
			// Note:  This will change the value of IsActive
			_analog = isActive ? 1f : 0f;

			if (_wasActive != IsActive)
			{
				_heldTime = 0f;
				_heldFrames = 0;
			}
			
			if (!string.IsNullOrEmpty(LogChanges) && IsActive != _wasActive)
			{
				Debug.Log($"ButtonState [{LogChanges}] changed to {IsActive}");
			}
		}


		/// <summary>
		/// Is the button currently pressed sufficiently to activate?
		/// </summary>
		public bool IsActive => (_analog >= _threshold);

		/// <summary>
		/// Did the button become active this frame?
		/// </summary>
		public bool Began
		{
			get
			{
				if (MinHold > 0) return IsActive && _heldFrames == MinHold;
				return IsActive && !_wasActive;
			}
		}

		/// <summary>
		/// Did the button become inactive this frame?
		/// </summary>
		public bool Ended
		{
			get
			{
				if (MinHold > 0) return !IsActive && _heldFrames == MinHold;
				return _wasActive && !IsActive;
			}
		}

		/// <summary>
		/// How long has the button been active?  Zero is returned if the button is not currently active.
		/// </summary>
		public float HeldTime => _heldTime;

		/// <summary>
		/// The current analog value of the button.
		/// </summary>
		public float Analog => _analog;
	}

}