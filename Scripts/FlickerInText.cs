using System.Collections;
using System.Collections.Generic;
using MEC;
using TMPro;
using UnityEngine;

namespace Barliesque.Utils
{

	public class FlickerInText : MonoBehaviour
	{
		private TMP_Text _text;
		private bool _needsUpdate;

		private readonly bool[] _flicker =
		{
			true, false, false, false, false, false,
			true, true, false, false,
			true, false, false,
			true, false, false, false, false,
			true
		};

		private void OnEnable()
		{
			StartCoroutine(Show());
		}

		private IEnumerator Show()
		{
			yield return new WaitForEndOfFrame();
			_text ??= GetComponent<TMP_Text>();
			_text.ForceMeshUpdate(true, true);

			var count = _text.textInfo.characterInfo.Length;
			for (int i = 0; i < count; i++)
			{
				var delay = (i % 2) + (i % (count >> 1)) * 2;
				Timing.RunCoroutine(FlickerChar(delay, i));
			}
		}

		private const float _frameTime = 1f / 60f;

		private IEnumerator<float> FlickerChar(int delay, int charIndex)
		{
			// Start hidden
			SetCharacterVisible(charIndex, false);

			// Wait your turn...
			while (delay > 0)
			{
				yield return Timing.WaitForSeconds(_frameTime);
				--delay;
			}

			for (int i = 0; i < _flicker.Length; i++)
			{
				yield return Timing.WaitForSeconds(_frameTime);
				SetCharacterVisible(charIndex, _flicker[i]);
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

		private void Update()
		{
			if (!_needsUpdate) return;
			_text.canvasRenderer.SetMesh(_text.mesh);
			_needsUpdate = false;
		}

	}

}