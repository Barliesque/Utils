using System.Text;
using Barliesque.EventObjects;
using TMPro;
using UnityEngine;
#if UNITY_ANDROID
#endif

namespace Barliesque.Utils
{
	
	public class ShowBuildInfo : MonoBehaviour
	{
		[Tooltip("Enable to shorten string by removing or abbreviating labels")]
		[SerializeField] private bool _concise;
		[SerializeField] private DateFormat _buildDate = DateFormat.DateOnly;
		[SerializeField] private GaugeString _buildVersion;
		[Tooltip("Android platforms only")]
		[SerializeField] private GaugeInt _buildVersionCode;
		[SerializeField] private GaugeString _buildDateTime;
		[SerializeField] private GaugeString _buildProfileName;
		[SerializeField] private bool _headset = true;

		private enum DateFormat
		{
			None, DateOnly, DateAndTime
		}

		private void Start() => ShowInfo();
		private void OnValidate() => ShowInfo();

#if UNITY_ANDROID
		private const bool _isAndroid = true;
#else
		private const bool _isAndroid = false;
#endif	

		private void ShowInfo()
		{
			var version = _buildVersion?.Value;
			if (string.IsNullOrEmpty(version)) version = "0.0.000";

			var code = _isAndroid ? (_buildVersionCode?.Value ?? 0) : -1;
			
			string dateTime = null;
			if (_buildDateTime && _buildDate != DateFormat.None)
			{
				dateTime = _buildDateTime.Value;
				if (string.IsNullOrEmpty(dateTime)) dateTime = "12/31/2018 00:00";
				dateTime = _buildDate == DateFormat.DateAndTime ? dateTime : dateTime.Split(' ')[0];
			}

			var profileName = _buildProfileName?.Value;
			if (_buildProfileName && string.IsNullOrEmpty(profileName)) profileName = "ProfileName";
			
			var headsetName = _headset ? GetHeadsetName() : null;
			
			var field = GetComponent<TextMeshProUGUI>();
			field.text = AssembleBuildInfo(version, code, dateTime, profileName, headsetName);
		}
		

		private string AssembleBuildInfo(string version, int code, string dateTime, string profile, string headset)
		{
			var data = new StringBuilder();
			if (!string.IsNullOrEmpty(version))
			{
				data.Append(_concise ? "v" : "Version: ");
				data.Append($"{version} ");
			}
			if (code >= 0) data.Append($"({code}) ");
			if (!string.IsNullOrEmpty(dateTime)) data.Append(_concise ? $" {dateTime} " : $" Date: {dateTime} ");
			if (!string.IsNullOrEmpty(profile)) data.Append(_concise ? $" {profile} " : $" Profile: {profile} ");
			if (_headset) data.Append(_concise ? $" - {GetHeadsetName()}" : $" Headset: {GetHeadsetName()}");
			return data.ToString();
		}
		

		private string GetHeadsetName()
		{
			if (!_isAndroid) return "PCVR";
			return (Application.isPlaying) ? SystemInfo.deviceModel : "Meta Quest 3";
			
			//return PlayerTracking.Headset.name; -- In OpenXR this returns nothing meaningful
			//return PlayerTracking.Headset.manufacturer;
			//XRSettings.eyeTextureWidth -- Returns render resolution.  Useless!
		}
		
	}
	
}