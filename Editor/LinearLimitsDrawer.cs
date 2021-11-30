using Barliesque.Utils;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace HandsOnVR.Editor
{
	[CustomPropertyDrawer(typeof(LinearLimits))]
	public class LinearLimitsDrawer : PropertyDrawer
	{
		override public VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			// Create property container element.
			var container = new VisualElement();

			// Create property fields.
			var startField = new PropertyField(property.FindPropertyRelative("Start"));
			var endField = new PropertyField(property.FindPropertyRelative("End"));

			// Add fields to the container.
			container.Add(startField);
			container.Add(endField);

			return container;
		}

		override public void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Using BeginProperty / EndProperty on the parent property means that
			// prefab override logic works on the entire property.
			EditorGUI.BeginProperty(position, label, property);

			// Draw label
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			// Don't make child fields be indented
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			// Calculate rects
			var width = position.width * 0.5f - 26f;
			var lowRect = new Rect(position.x, position.y, width, position.height);
			var toRect = new Rect(position.x + width + 6f, position.y, 14f, position.height);
			var highRect = new Rect(position.x + width + 26, position.y, width, position.height);

			// Draw fields - pass GUIContent.none to each so they are drawn without labels
			EditorGUI.PropertyField(lowRect, property.FindPropertyRelative("Start"), GUIContent.none);
			EditorGUI.LabelField(toRect, "to");
			EditorGUI.PropertyField(highRect, property.FindPropertyRelative("End"), GUIContent.none);

			// Set indent back to what it was
			EditorGUI.indentLevel = indent;

			EditorGUI.EndProperty();
		}
	}
}