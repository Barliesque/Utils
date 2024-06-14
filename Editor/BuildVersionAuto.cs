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

	private const string _menuPath = "Tools/Auto Increment Build Number";
	private const string _editorPrefsKey = "AutoIncrementBuildNumber";


	static private bool IsEnabled => EditorPrefs.GetBool(_editorPrefsKey);

	[MenuItem(_menuPath, false, 10)]
	static private void ToggleEnabled()
	{
		var toggled = !IsEnabled;
		EditorPrefs.SetBool(_editorPrefsKey, toggled);
		Menu.SetChecked(_menuPath, toggled);
	}

	[MenuItem(_menuPath, true)]
	static private bool SettingValidate()
	{
		var enabled = EditorPrefs.GetBool(_editorPrefsKey, true);
		Menu.SetChecked(_menuPath, enabled);
		return true;
	}

	[InitializeOnLoadMethod]
	static private void AlertIfDisabled()
	{
		if (IsEnabled) return;
		Debug.Log($"<color=yellow>Auto Incrementing Build Number is disabled.</color>  To enable, go to: {_menuPath}");
	}
	

	public void OnPreprocessBuild(BuildReport report)
	{
		if (!IsEnabled)
		{
			Debug.Log($"<color=yellow>Auto Incrementing Build Number is disabled.</color>  To enable, go to: {_menuPath}");
			return;
		}
		
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