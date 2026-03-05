using System;
using UnityEngine;

namespace Barliesque.Utils
{
	public class CopyTransform : MonoBehaviour
	{

		[SerializeField] private Transform _copyFrom;
		[SerializeField] private Match _match = Match.Position | Match.Rotation;
		[SerializeField] private Axis _axes = Axis.X | Axis.Y | Axis.Z;
		[SerializeField] private Space _coordinateSpace;
		[SerializeField] private UpdateTime _applyWhen = UpdateTime.Update;
		
		
		public enum Space
		{
			Local,
			World
		}

		[Flags]
		public enum UpdateTime
		{
			Update = 1,
			FixedUpdate = 2,
			LateUpdate = 4,
			OnBeforeRender = 8
		}

		[Flags]
		public enum Match
		{
			Position = 1,
			Rotation = 2,
			Scale = 4,
		}

		private Transform _xform;

		private void Awake()
		{
			_xform = GetComponent<Transform>();
		}
		

		private bool ApplyWhen(UpdateTime query)
		{
			return ((int)_applyWhen & (int)query) != 0;
		}

		private void OnEnable()
		{
			if (ApplyWhen(UpdateTime.OnBeforeRender)) Application.onBeforeRender += Apply;
		}

		private void OnDisable()
		{
			if (ApplyWhen(UpdateTime.OnBeforeRender)) Application.onBeforeRender -= Apply;
		}

		private bool MatchPos => ((int)_match & (int)Match.Position) != 0;
		private bool MatchRot => ((int)_match & (int)Match.Rotation) != 0;
		private bool MatchScale => ((int)_match & (int)Match.Scale) != 0;

		
		public void Apply()
		{
			if (_coordinateSpace == Space.Local)
			{
				if (MatchPos) _xform.localPosition = _xform.localPosition.CopyAxes(_axes, _copyFrom.localPosition);
				if (MatchRot) _xform.localEulerAngles = _xform.localEulerAngles.CopyAxes(_axes, _copyFrom.localEulerAngles);
				if (MatchScale) _xform.localScale = _xform.localScale.CopyAxes(_axes, _copyFrom.localScale);
			}
			else
			{
				if (MatchPos) _xform.position = _copyFrom.position;
				if (MatchRot) _xform.rotation = _copyFrom.rotation;
				if (!MatchScale) return;
				
				var worldScale = _copyFrom.lossyScale;
				if (_xform.parent)
				{
					var parentScale = _xform.parent.lossyScale;
					worldScale.x /= parentScale.x;
					worldScale.y /= parentScale.y;
					worldScale.z /= parentScale.z;
				}
				_xform.localScale = worldScale;
			}
		}


		private void Update()
		{
			if (ApplyWhen(UpdateTime.Update)) Apply();
		}

		private void LateUpdate()
		{
			if (ApplyWhen(UpdateTime.LateUpdate)) Apply();
		}

		private void FixedUpdate()
		{
			if (ApplyWhen(UpdateTime.FixedUpdate)) Apply();
		}
		
	}
}