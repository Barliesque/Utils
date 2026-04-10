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
		
		[Tooltip("Automatically level out roll rotation, as long as the head's z-rotation is within this many degrees.")]
		[SerializeField] private float _levelThreshold = 15f;
		
		[SerializeField] private bool _disableOnMobile = true;

		private Quaternion _rotationVelocity = Quaternion.identity;
		private Vector3 _positionVelocity = Vector3.zero;
		private Transform _xform;
		
		private void Awake()
		{
			if (_disableOnMobile && Application.isMobilePlatform && !Application.isEditor)
			{
				gameObject.SetActive(false);
			}

			_xform = GetComponent<Transform>();
		}
		

		private void LateUpdate()
		{
			if (!_playerCamera) return;

			// Smoothly follow the player camera
			var euler = _playerCamera.eulerAngles;
			var roll = Mathf.Abs(Mathf.DeltaAngle(0f, euler.z));
			if (roll <= _levelThreshold) euler.z = 0f;
			var targetRot = Quaternion.Euler(euler); 
			_xform.rotation = QuaternionUtils.SmoothDamp(_xform.rotation, targetRot, ref _rotationVelocity, _rotationSmoothTime);
			_xform.position = Vector3.SmoothDamp(_xform.position, _playerCamera.position, ref _positionVelocity, _positionSmoothTime);
		}
	}

}