using System.Collections.Generic;
using Barliesque.InspectorTools.Editor;
using UnityEditor;
using UnityEngine;

namespace Barliesque.Utils.Editor
{
	
	[CustomEditor(typeof(AnimatorEvent))]
	public class AnimatorEventEditor : EditorBase<AnimatorEvent>
	{
		private bool _showEvents = true;

		override protected void CustomInspector(AnimatorEvent inst)
		{
			var animator = inst.GetComponent<Animator>();
			if (!animator) EditorTools.HelpBox("This component must be on the same GameObject as the Animator that will be invoking events.", MessageType.Error);
			
			var paramTypes = PropertyField("_paramTypes").intValue;

			var events = new List<string>();
			if ((paramTypes & (int)AnimatorEvent.ParamType.Void) > 0) events.Add("OnFrameEvent");
			if ((paramTypes & (int)AnimatorEvent.ParamType.Int) > 0) events.Add("OnFrameEventInt");
			if ((paramTypes & (int)AnimatorEvent.ParamType.Float) > 0) events.Add("OnFrameEventFloat");
			if ((paramTypes & (int)AnimatorEvent.ParamType.String) > 0) events.Add("OnFrameEventString");
			if ((paramTypes & (int)AnimatorEvent.ParamType.Object) > 0) events.Add("OnFrameEventObject");
			if (events.Count > 0)
			{
				if (_showEvents) EditorGUILayout.Space();
				EventsGroup("Frame Events", ref _showEvents, events.ToArray());
			}
		}
	}
}