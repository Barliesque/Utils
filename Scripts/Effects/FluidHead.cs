using Barliesque.InspectorTools;
using UnityEngine;


namespace Barliesque.Utils
{
	/// <summary>
	/// Drive a monitor-only camera to smoothly follow the movements of the VR-controlled camera
	/// </summary>
	public class FluidHead : MonoBehaviour
	{
		[HelpBox("<b>To set up the fluid camera</b>\nMake a duplicate base camera, and set its Priority to 1.  Also set the Output >> Target Eye to None.")]
		[SerializeField] private Transform _playerCamera;
		[SerializeField] private float _rotationSmoothTime = 0.25f;
		[SerializeField] private float _positionSmoothTime = 0.5f;
		
		[Tooltip("Up to this many degrees of head roll are removed. Smaller tilts come out level; larger tilts are reduced by this amount.")]
		[SerializeField] private float _levelThreshold = 15f;
		
		[SerializeField] private bool _disableOnMobile = true;

		private Quaternion _rotationVelocity = QuaternionUtils.ZeroVelocity;
		private Vector3 _positionVelocity = Vector3.zero;
		private Quaternion _rotation = Quaternion.identity;
		private Transform _xform;
		
		private void Awake()
		{
			_xform = GetComponent<Transform>();

			if (_disableOnMobile && Application.isMobilePlatform && !Application.isEditor)
			{
				gameObject.SetActive(false);
			}
		}


		private void OnEnable() => Snap();


		/// <summary>
		/// Jump straight to the player's head and clear any accumulated velocity.
		/// </summary>
		public void Snap()
		{
			if (!_playerCamera || !_xform) return;
			_rotation = Level(_playerCamera.rotation);
			_rotationVelocity = QuaternionUtils.ZeroVelocity;
			_positionVelocity = Vector3.zero;
			_xform.SetPositionAndRotation(_playerCamera.position, _rotation);
		}
		

		private void LateUpdate()
		{
			if (!_playerCamera) return;

			// Unscaled, so spectators still see smooth motion while the game is paused
			var dt = Time.unscaledDeltaTime;

			var targetRot = Level(_playerCamera.rotation);
			_rotation = QuaternionUtils.SmoothDamp(_rotation, targetRot, ref _rotationVelocity, _rotationSmoothTime, dt);
			var position = Vector3.SmoothDamp(_xform.position, _playerCamera.position, ref _positionVelocity, _positionSmoothTime, Mathf.Infinity, dt);

			_xform.SetPositionAndRotation(position, _rotation);
		}


		/// <summary>
		/// Remove up to _levelThreshold degrees of roll, continuously (no snap at the threshold),
		/// fading the effect out as the head looks straight up or down, where roll is ill-defined.
		/// </summary>
		private Quaternion Level(Quaternion head)
		{
			var forward = head * Vector3.forward;
			var up = head * Vector3.up;

			// World up, flattened onto the plane perpendicular to the view direction.
			// Its length is cos(pitch): 1 when looking at the horizon, 0 when looking straight up/down.
			var levelUp = Vector3.ProjectOnPlane(Vector3.up, forward);
			var horizontalness = levelUp.magnitude;
			if (horizontalness < 1e-4f) return head;

			var roll = Vector3.SignedAngle(levelUp, up, forward);
			var correction = Mathf.Clamp(roll, -_levelThreshold, _levelThreshold);
			correction *= Mathf.InverseLerp(0.05f, 0.3f, horizontalness);

			return Quaternion.AngleAxis(-correction, forward) * head;
		}
	}

}