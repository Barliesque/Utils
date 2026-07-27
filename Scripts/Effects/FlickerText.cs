using System.Collections.Generic;
using MEC;
using TMPro;
using UnityEngine;

namespace Barliesque.Utils
{

	public class FlickerText : MonoBehaviour
	{
		private TMP_Text _text;
		private bool _needsUpdate;

		private readonly bool[] _flicker =
		{
			false, true, false, false, false, false, false,
			true, true, false, false, false,
			true, false, false,
			true, false, false, false, false, false,
			true
		};


		public void Show() => gameObject.SetActive(true);

		private void OnEnable() => _initFlickerIn = true;

		private bool _initFlickerIn;
		private void FlickerIn()
		{
			_text ??= GetComponent<TMP_Text>();
			_text.ForceMeshUpdate(true, true);

			_text.enabled = true;
			var count = _text.textInfo.characterInfo.Length;
			for (int i = 0; i < count; i++)
			{
				var delay = (i % 2) + (i % (count >> 1)) * 3;
				Timing.RunCoroutine(FlickerChar(delay, i, true));
			}
			_initFlickerIn = false;
		}
		

		public void Hide()
		{
			Timing.RunCoroutine(FlickerOut());
		}
		
		private IEnumerator<float> FlickerOut()
		{
			var count = _text.textInfo.characterInfo.Length;
			for (int i = 0; i < count; i++)
			{
				var delay = (i % 2) + (i % (count >> 1)) * 3;
				Timing.RunCoroutine(FlickerChar(delay, i, false));
			}
			
			// Wait for effect to complete, then completely hide
			yield return Timing.WaitForSeconds((count * 3 + _flicker.Length) * _frameTime);
			_text.enabled = false;
		}

		
		private const float _frameTime = 1f / 60f;

		private IEnumerator<float> FlickerChar(int delay, int charIndex, bool flickerIn)
		{
			if (flickerIn)
			{
				// Start hidden
				SetCharacterVisible(charIndex, false);
			}

			// Wait your turn...
			while (delay > 0)
			{
				yield return Timing.WaitForSeconds(_frameTime);
				--delay;
			}

			for (int i = 0; i < _flicker.Length; i++)
			{
				yield return Timing.WaitForSeconds(_frameTime);
				SetCharacterVisible(charIndex, flickerIn ? _flicker[i] : !_flicker[i]);
			}

			if (!flickerIn)
			{
				// End hidden
				SetCharacterVisible(charIndex, false);
			}
		}

		private void SetCharacterVisible(int index, bool visible)
		{
			var mesh = _text.mesh;
			var vertex = _text.textInfo.characterInfo[index].vertexIndex;
			var colors = mesh.colors;
			for (int i = 0; i < 4; i++) colors[vertex + i].a = visible ? 1f : 0f;
			mesh.colors = colors;
			_needsUpdate = true;
		}

		private void LateUpdate()
		{
			if (_initFlickerIn) FlickerIn();
			if (!_needsUpdate) return;
			_text.canvasRenderer.SetMesh(_text.mesh);
			_needsUpdate = false;
		}

	}

}