using UnityEngine;


namespace Barliesque.Utils
{
	
	/// <summary>
	/// A ScriptableObject that separates the various types of controller input from the specific usage.
	/// </summary>
	[CreateAssetMenu(fileName = "New 2-Axis Button State", menuName = "XR Controls/2-Axis Button State")]
	public class ButtonState2AxisObject : ScriptableObject
	{
		[SerializeField] private bool _canSetActive = true;
		public bool CanSetActive => _canSetActive;
		[SerializeField] private float _threshold = 0.5f;
	
		public ButtonState North { get; private set; } 
		public ButtonState South { get; private set; }
		public ButtonState East { get; private set; }
		public ButtonState West { get; private set; }
		

		private void OnEnable()
		{
			North = new ButtonState(_threshold);
			South = new ButtonState(_threshold);
			East = new ButtonState(_threshold);
			West = new ButtonState(_threshold);
		}

		public void UpdateState(Vector2 value)
		{
			 East.Update(value.x);   // Mathf.Clamp01( )    
			 West.Update(-value.x);  // Mathf.Clamp01( ) 
			North.Update(value.y);   // Mathf.Clamp01( )
   			South.Update(-value.y);  // Mathf.Clamp01( )
		}

		public Vector2 Position => new Vector2(East.Analog, North.Analog);
		public bool IsActive => North.IsActive || South.IsActive || East.IsActive || West.IsActive;
		public bool Began => North.Began || South.Began || East.Began || West.Began;
		public bool Ended => North.Ended || South.Ended || East.Ended || West.Ended;
	}
}