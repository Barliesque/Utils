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

		static private GUIContent _helpLabel = new ("Help", "Events not firing?  Expand to show helpful hints to fix the problem.");
		static private string _helpText;

		static private GUIContent _bothLabel = new("Both Collisions & Triggers", "Force both types of events to be shown.  This does not affect which events are actually invoked.");
		
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

			var both = PropertyField("_collisionOrTrigger", _bothLabel).boolValue;

			if (!body || _colliders.Count == 0)
			{
				EditorTools.HelpBox("Sensor can only respond if Colliders and a Rigidbody are on <b>this</b> GameObject.", MessageType.Error);
			}
			else
			{
				if (gotTrigger && gotCollision)
				{
					EditorTools.HelpBox(
						"This Sensor will invoke all Trigger and Collision events because there is a mix of solid and trigger colliders.",
						MessageType.Info);
				}
				else
				{
					if (gotTrigger)
					{
						EditorTools.HelpBox("This Sensor cannot invoke collision events because it has no non-trigger colliders!", MessageType.Warning);
						EditorTools.HelpBox("This Sensor will invoke events: <b>OnEnterTrigger</b>, <b>OnExitTrigger</b> and <b>OnStayTrigger</b>",
							MessageType.Info);
					}
					else if (gotCollision)
					{
						if (both)
						{
							EditorTools.HelpBox(
								"This non-trigger object will invoke collision events or trigger events, based upon whether the other object is a trigger.",
								MessageType.Info);
						}
						else
						{
							EditorTools.HelpBox(
								"This Sensor will invoke events: <b>OnEnterCollision</b>, <b>OnExitCollision</b> and <b>OnStayCollision</b>",
								MessageType.Info);
						}
					}
				}
				if (gotCollision || both) EventsGroup("Collision Events", ref _unfoldCollisionEvents, new [] {"OnEnterCollision", "OnExitCollision", "OnStayCollision"});
				if (gotTrigger || both) EventsGroup("Trigger Events", ref _unfoldTriggerEvents, new [] {"OnEnterTrigger", "OnExitTrigger", "OnStayTrigger"});
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
					help.Append("• If either collider is a trigger, then trigger events are fired--not collision events.\n");
					help.Append("• MeshColliders can sometimes fail to fire events when colliding with other MeshColliders.\n");
					help.Append("• Make sure their layers are enabled for collision in Project Settings > Physics.\n");
					help.Append("• Note that this component handles 3D physics events, not 2D.");
					_helpText = help.ToString();
				}
				
				EditorTools.HelpBox(_helpText, MessageType.Info);
			}

		}
	}
}