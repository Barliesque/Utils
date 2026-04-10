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
		//TODO  Need to add a parameter to control the analog value of touch (versus press)
		//TODO  Add optional support for hold-to-repeat
		//TODO  Implement passthrough referencing to support asset bundles

		[FormerlySerializedAs("_canSetActive"), SerializeField,
		 Tooltip("If selected, PlayerControls.IsRightHanded will be altered based on activity of this Button State -- "
		         + "assuming it is assigned to a Player Controls field in the Inspector.")]
		private bool _setsActiveHand = true;

		[SerializeField] private float _threshold = 0.5f;
		[SerializeField] private float _deadZone = 1e-4f;
		[SerializeField] private bool _logChanges = false;

		private void OnEnable()
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