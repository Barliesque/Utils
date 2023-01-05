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

		/// <summary>
		/// To enable logging, set this string which will be used as a prefix when logging.
		/// </summary>
		public string LogChanges = null;
		
		private int _lastUpdated = -1;


		/// <summary>
		/// Create a ButtonState to track the current state of a controller button
		/// </summary>
		/// <param name="threshold">The minimum analog value required for the button to be interpreted as active.</param>
		public ButtonState(float threshold = 0.5f)
		{
			_threshold = threshold;
		}


		/// <summary>
		/// To be called once every frame to update the current state of the button.
		/// </summary>
		/// <param name="analog">The analog value of the button, from 0.0 to 1.0</param>
		public void Update(float analog)
		{
			if (_lastUpdated != Time.frameCount)
			{
				// Do not allow _wasActive to be changed multiple times per frame
				_wasActive = IsActive;
				_lastUpdated = Time.frameCount;
			}
			
			_analog = analog;

			if (_wasActive && IsActive)
			{
				_heldTime += Time.unscaledDeltaTime;
			}
			else
			{
				_heldTime = 0f;
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
			if (_lastUpdated != Time.frameCount)
			{
				// Do not allow _wasActive to be changed multiple times per frame
				_wasActive = IsActive;
				_lastUpdated = Time.frameCount;
			}
			
			_analog = isActive ? 1f : 0f;

			if (_wasActive && IsActive)
			{
				_heldTime += Time.unscaledDeltaTime;
			}
			else
			{
				_heldTime = 0f;
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
		public bool Began => IsActive && !_wasActive;

		/// <summary>
		/// Did the button become inactive this frame?
		/// </summary>
		public bool Ended => _wasActive && !IsActive;

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