using System.Collections.Generic;
using System.Text;
using Barliesque.InspectorTools.Editor;
using UnityEditor;
using UnityEngine;


namespace Barliesque.Utils.Editor
{
	[CustomEditor(typeof(Sensor))]
	public class SensorEditor : EditorBase<Sensor>
	{
		private readonly List<Collider> _colliders = new List<Collider>();
		static private bool _unfoldCollisionEvents = false;
		static private bool _unfoldTriggerEvents = false;
		static private bool _showHelp = false;

		static private GUIContent _helpLabel = new GUIContent("Help", "Events not firing?  Expand to show helpful hints to fix the problem.");
		static private string _helpText;

		static private GUIContent _eventTypeLabel = new GUIContent("Event Type(s)", "What type of event(s) to show in this inspector -- does not affect which events are actually invoked.");

		#if SENSOR_STAY
		private const bool _stayEvents = true;
		#else
		private const bool _stayEvents = false;
		#endif
		
		override protected void CustomInspector(Sensor inst)
		{
			inst.GetComponents<Collider>(_colliders);
			bool body = inst.GetComponent<Rigidbody>();
			bool gotTrigger = false;
			bool gotCollision = false;
			for (int i = 0; i < _colliders.Count; i++)
			{
				gotTrigger |= (_colliders[i].isTrigger);
				gotCollision |= (!_colliders[i].isTrigger);
			}

			var eventTypes = (Sensor.SensorEventType)PropertyField("_eventType", _eventTypeLabel).intValue;
			bool wantCollision = (eventTypes & Sensor.SensorEventType.Collision) != 0;
			bool wantTrigger = (eventTypes & Sensor.SensorEventType.Trigger) != 0;
			
			PropertyField("_collisionLayers");
			PropertyField("_oncePerBody");

			if (_stayEvents && !Physics.reuseCollisionCallbacks) 
			{
				EditorTools.HelpBox("SENSOR_STAY is enabled.  Consider enabling Physics.reuseCollisionCallbacks", MessageType.Warning);
			}
			
			if (_colliders.Count == 0)
			{
				EditorTools.HelpBox("Sensor can only respond if Colliders are on <b>this</b> GameObject.", MessageType.Error);
			}
			else
			{
				if (!body)
				{
					EditorTools.HelpBox("This Sensor will only respond to colliders that have a Rigidbody.  To respond to all colliders, add a Rigidbody to <b>this</b> GameObject.", MessageType.Info);
				}
				if (gotTrigger && gotCollision)
				{
					EditorTools.HelpBox("This Sensor will invoke Trigger and Collision events because there is a mix of solid and trigger colliders.",
						MessageType.Info);
				}
				else
				{
					if (gotTrigger)
					{
						if (wantCollision) EditorTools.HelpBox("This Sensor cannot invoke collision events because it has no non-trigger colliders!", MessageType.Warning);
						EditorTools.HelpBox(_stayEvents ? 
								"This Sensor will invoke events: <b>OnEnterTrigger</b>, <b>OnExitTrigger</b> and <b>OnStayTrigger</b>" : 
								"This Sensor will invoke events: <b>OnEnterTrigger</b> and <b>OnExitTrigger</b>.",
							MessageType.Info);
					}
					else if (gotCollision)
					{
						if (eventTypes == (Sensor.SensorEventType.Collision & Sensor.SensorEventType.Trigger))
						{
							EditorTools.HelpBox(
								"This non-trigger object will invoke collision events or trigger events, based upon whether the other object is a trigger.",
								MessageType.Info);
						}
						else
						{
							EditorTools.HelpBox(_stayEvents ?
								"This Sensor will invoke events: <b>OnEnterCollision</b>, <b>OnExitCollision</b> and <b>OnStayCollision</b>" :
								"This Sensor will invoke events: <b>OnEnterCollision</b> and <b>OnExitCollision</b>.",
								MessageType.Info);
						}
					}
				}
				if (wantCollision) EventsGroup("Collision Events", ref _unfoldCollisionEvents, _stayEvents ? new [] {"OnEnterCollision", "OnExitCollision", "OnStayCollision"} : new [] {"OnEnterCollision", "OnExitCollision"});
				if (wantTrigger) EventsGroup("Trigger Events", ref _unfoldTriggerEvents, _stayEvents ? new [] {"OnEnterTrigger", "OnExitTrigger", "OnStayTrigger"} : new [] {"OnEnterTrigger", "OnExitTrigger"});
			}
			
			EditorGUILayout.Space();
			_showHelp = EditorGUILayout.Foldout(_showHelp,  _helpLabel);
			if (_showHelp)
			{
				if (string.IsNullOrEmpty(_helpText))
				{
					var help = new StringBuilder();
					help.Append("• Both objects must have at least one collider.\n");
					help.Append("• At least one of the objects must have a Rigidbody, and be non-kinematic and non-static.\n");
					help.Append("• Enable collision between two kinematic bodies: Project Settings » Physics » Contact Pairs Mode.\n");
					help.Append("• Make sure their layers are enabled for collision in Project Settings » Physics.\n");
					help.Append("• If either collider is a trigger, then trigger events are fired, not collision events.\n");
					help.Append("• MeshColliders can sometimes fail to fire events when colliding with other MeshColliders.\n");
					help.Append("• Note that this component handles 3D physics events, not 2D.\n");
					help.Append("• To enable OnStayTrigger or OnStayCollision add SENSOR_STAY to the Scripting Define Symbols.");
					_helpText = help.ToString();
				}
				
				EditorTools.HelpBox(_helpText, MessageType.Info);
			}

		}
	}
}