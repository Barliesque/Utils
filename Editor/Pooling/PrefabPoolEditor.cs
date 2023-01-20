using UnityEditor;
using Barliesque.InspectorTools.Editor;
using UnityEngine;

namespace Barliesque.Utils.Editor
{
	[CustomEditor(typeof(PrefabPool))]
	public class PrefabPoolEditor : EditorBase<PrefabPool>
	{
		private ListEditor _prefabs;

		override protected void OnEnabled(PrefabPool inst)
		{
			_prefabs = new ListEditor(serializedObject, "_prefabs", "Prefabs");
		}

		override protected void CustomInspector(PrefabPool inst)
		{
			GUI.enabled = !Application.isPlaying;
			_prefabs.DoLayoutList();

			if (!Application.isPlaying) return;
			
			GUI.enabled = true;
			EditorTools.Separator();
			EditorTools.Header("Active Instances");
			EditorTools.BeginInfoBox();
			var count = _prefabs.Count;
			for (int i = 0; i < count; i++)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(_prefabs.GetElement(i).objectReferenceValue.name, GUILayout.Width(120f));
				EditorGUILayout.LabelField($"Active: {inst.GetActiveCount(i)} of {inst.GetCount(i)}  Max: {inst.GetMaxActive(i)}  Limit: {inst.GetMaxCount(i)}");
				EditorGUILayout.EndHorizontal();
			}
			EditorTools.EndInfoBox();
		}
	}
}