using System;
using UnityEditor;
using Barliesque.InspectorTools.Editor;

namespace Barliesque.Utils.Editor
{
	
	[CustomEditor(typeof(OnAnimationFrame))]
	public class OnAnimationFrameEditor : EditorBase<OnAnimationFrame>
	{
		override protected bool ShowScriptField => false;

		override protected void CustomInspector(OnAnimationFrame inst)
		{
			var frame = PropertyField("_frame").intValue;
			var fps = PropertyField("_fps", "FPS").intValue;
			var seconds = frame / fps;
			var frames = frame - (fps * seconds);
			EditorGUILayout.LabelField("Time", $"{seconds}:{frames:00}");

			EditorGUILayout.Space();
			PropertyField("_message");

			EditorGUI.indentLevel++;
			var paramType = (OnAnimationFrame.ParamType)PropertyField("_parameterType").intValue;
			switch (paramType)
			{
				case OnAnimationFrame.ParamType.None:
					break;
				case OnAnimationFrame.ParamType.Int:
					PropertyField("_intParam", "Int Value");
					break;
				case OnAnimationFrame.ParamType.Float:
					PropertyField("_floatParam", "Float Value");
					break;
				case OnAnimationFrame.ParamType.String:
					PropertyField("_stringParam", "String Value");
					break;
				case OnAnimationFrame.ParamType.Animator:
					EditorTools.HelpBox("A reference to this Animator will be passed.", MessageType.Info);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			EditorGUI.indentLevel--;
			
			EditorGUILayout.Space();
			PropertyField("_sendOnEachLoop");
			PropertyField("_sendUpwards");
			PropertyField("_requireReceiver");
		}
	}
}