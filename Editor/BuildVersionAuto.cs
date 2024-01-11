using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// Automatically increment the PlayerSettings.BundleVersion just before making a new build
/// </summary>
public class BuildVersionAuto : IPreprocessBuildWithReport
{
	public int callbackOrder => 0;

	public void OnPreprocessBuild(BuildReport report)
	{
		var parts = PlayerSettings.bundleVersion.Split('.');
		if (parts == null || parts.Length == 0) parts = new[] { "0", "0", "0" };
		var digit = parts.Length - 1;
		var success = int.TryParse(parts[digit], out int ver);
		if (success)
		{
			parts[digit] = (ver + 1).ToString();
			PlayerSettings.bundleVersion = string.Join(".", parts);
		}
		else
		{
			PlayerSettings.bundleVersion = $"{PlayerSettings.bundleVersion}.1";
		}

#if UNITY_ANDROID
		++PlayerSettings.Android.bundleVersionCode;
#endif

		Debug.Log($"PlayerSettings.bundleVersion updated to:  {PlayerSettings.bundleVersion}");

		string[] guids = AssetDatabase.FindAssets($"t:{typeof(GaugeString)}");
		foreach (var guid in guids)
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			var buildDateTime = AssetDatabase.LoadAssetAtPath<GaugeString>(path);
			if (buildDateTime.name != "BuildDateTime") continue;
			if (!buildDateTime.IsPersistent) Debug.LogError($"Could not store BuildDateTime because Gauge is not Persistent: {path}");

			var serialized = new SerializedObject(buildDateTime);
			var now = DateTime.Now.ToString("g");
			serialized.FindProperty("_default").stringValue = now;
			serialized.FindProperty("_current").stringValue = now;
			serialized.ApplyModifiedProperties();
			EditorUtility.SetDirty(buildDateTime);
		}
	}
}