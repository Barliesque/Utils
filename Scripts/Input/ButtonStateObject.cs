using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Barliesque.Utils
{
	/// <summary>
	/// A ScriptableObject that separates the various types of controller input from the specific usage.
	/// </summary>
	[CreateAssetMenu(fileName = "New Button State", menuName = "XR Controls/Button State")]
	public class ButtonStateObject : ScriptableObject
	{
		//TODO  Add optional support for hold-to-repeat
		//TODO  Implement passthrough referencing to support asset bundles

		[FormerlySerializedAs("_canSetActive"), SerializeField,
		 Tooltip("If selected, PlayerControls.IsRightHanded will be altered based on activity of this Button State -- " +
		         "assuming it is assigned to a Player Controls field in the Inspector.")]
		private bool _setsActiveHand = true;

		[Tooltip("Analog input value must be greater than this threshold for the button to register as pressed.")]
		[SerializeField, Range(0, 1)] private float _threshold = 0.5f;

		[Tooltip("Input values below this threshold will be interpreted as zero.  " +
		         "Increase this to combat the effects of noisy analog inputs.")]
		[SerializeField] private float _deadZone = 1e-4f;

		[Tooltip("The minimum number of frames before a button state change should be registered.  " +
		         "Increasing this value may help to neutralize erratic button inputs.")]
		[SerializeField] private int _minHold = 0;

		[SerializeField] private bool _logChanges = false;


		private void OnValidate() => Reset();

		private void OnEnable() => Reset();

		private void Reset()
		{
			State = new ButtonState(_threshold, _deadZone)
			{
				LogChanges = _logChanges ? name : null
			};
		}

		public bool SetsActiveHand => _setsActiveHand;

		public ButtonState State { get; private set; }
	}
}