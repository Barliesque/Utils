using System;
using UnityEngine;


namespace Barliesque.Utils
{
	/// <summary>
	/// Drive a monitor-only camera to smoothly follow the movements of the VR-controlled camera
	/// </summary>
	public class FluidHead : MonoBehaviour
	{
		[SerializeField] private Transform _playerCamera;
		[SerializeField] private float _rotationSmoothTime = 0.7f;
		[SerializeField] private float _positionSmoothTime = 0.7f;
		[SerializeField] private bool _disableOnMobile = true;

		private Quaternion _rotationVelocity = Quaternion.identity;
		private Vector3 _positionVelocity = Vector3.zero;

		
		private void Awake()
		{
			if (_disableOnMobile && Application.isMobilePlatform && !Application.isEditor)
			{
				gameObject.SetActive(false);
			}
		}
		

		private void LateUpdate()
		{
			if (!_playerCamera) return;

			// Smoothly follow the player camera
			transform.rotation = QuaternionUtils.SmoothDamp(transform.rotation, _playerCamera.rotation, ref _rotationVelocity, _rotationSmoothTime);
			transform.position = Vector3.SmoothDamp(transform.position, _playerCamera.position, ref _positionVelocity, _positionSmoothTime);
		}
	}

}