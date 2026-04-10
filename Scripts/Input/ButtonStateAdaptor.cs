using UnityEngine;


namespace Barliesque.Utils
{

	public class ButtonStateAdaptor : MonoBehaviour
	{
		[SerializeField] private ButtonStateObject[] _inputs;
		[SerializeField] private Condition _condition;
		[SerializeField] private bool _analog = true;
		[SerializeField] private ButtonStateObject _output;

		private enum Condition
		{
			Any, One, All, None
		}

		private void Update()
		{
			if (!_output) return;
			if (_analog)
				UpdateAnalog();
			else
				UpdateDigital();
		}

		private void UpdateAnalog()
		{
			float current = 0f;
			if (_condition == Condition.All)
			{
				current = 1f;
				foreach (var input in _inputs)
				{
					current = Mathf.Min(current, input.State.Analog);
				}
			}
			else if (_condition == Condition.Any)
			{
				foreach (var input in _inputs)
				{
					current = Mathf.Max(current, input.State.Analog);
				}
			}
			else if (_condition == Condition.None)
			{
				foreach (var input in _inputs)
				{
					current = Mathf.Max(current, input.State.Analog);
				}
				current = 1f - current;
			}
			else if (_condition == Condition.One)
			{
				int count = 0;
				foreach (var input in _inputs)
				{
					current = input.State.Analog;
					if (current <= 0f) continue;
					if (++count < 2) continue;
					current = 0f;
					break;
				}
			}

			_output.State.Update(current);
		}

		
		private void UpdateDigital()
		{
			bool current = false;
			if (_condition == Condition.All)
			{
				current = true;
				foreach (var input in _inputs)
				{
					if (input.State.IsActive) continue;
					current = false;
					break;
				}
			}
			else if (_condition == Condition.Any)
			{
				foreach (var input in _inputs)
				{
					if (input.State.IsActive) continue;
					current = true;
					break;
				}
			}
			else if (_condition == Condition.None)
			{
				current = true;
				foreach (var input in _inputs)
				{
					if (!input.State.IsActive) continue;
					current = false;
					break;
				}
			}
			else if (_condition == Condition.One)
			{
				foreach (var input in _inputs)
				{
					if (!input.State.IsActive) continue;
					if (current)
					{
						current = false;
						break;
					}
					current = true;
				}
			}

			_output.State.Update(current ? 1f : 0f);
		}
		
	}


}