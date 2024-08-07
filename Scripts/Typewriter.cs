using System.Collections;
using Barliesque.InspectorTools;
using TMPro;
using UnityEngine;

namespace Barliesque.Utils
{


	[ExecuteAlways]
	public class Typewriter : MonoBehaviour
	{
		[HelpBox(
			"Place this component on the same GameObject as a TextMeshPro (or TextMeshProUGUI) to type out the text.  From code, call <b>Typewriter.Show()</b> or transition the Show property with an Animator component.  Or tick the <b>Show On Enable</b> option to automatically reveal the text.",
			HelpBoxType.Info)]
		[SerializeField, Range(0, 1)] private float _show = 1f;

		[SerializeField] private bool _showOnEnable;

		[HideIf("_showOnEnable", false)]
		[SerializeField] private float _delay = 0f;

		[HideIf("_showOnEnable", false)]
		[SerializeField] private float _duration = 1f;

		private TMP_Text _field;
		private int _totalChars;
		private TMP_TextInfo _textInfo;

		private void OnEnable()
		{
			if (!Application.isPlaying) return;
			if (!_showOnEnable) return;

			if (_delay > 0f) StartCoroutine(DelayedShow());
			else Show(_duration);
		}

		private IEnumerator DelayedShow()
		{
			yield return new WaitForSeconds(_delay);
			Show(_duration);
		}

		private void LateUpdate()
		{
			_field ??= GetComponent<TMP_Text>();
			if (!_field) return;
			if (_textInfo == null || _textInfo.textComponent != _field || _textInfo.characterCount != _totalChars)
			{
				_textInfo = _field.GetTextInfo(_field.text);
				_totalChars = _textInfo.characterCount;
			}

			_field.maxVisibleCharacters = Mathf.CeilToInt((_totalChars - 0.9f) * _show);
		}

		public void Show(float duration = 1f)
		{
			this.Play(duration, (t) => _show = t);
		}

		public void Hide(float duration = 0.6f)
		{
			this.Play(duration, (t) => _show = 1f - t);
		}

	}

}