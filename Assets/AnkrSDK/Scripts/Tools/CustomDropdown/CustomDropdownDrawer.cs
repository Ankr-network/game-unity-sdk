using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AnkrSDK.Tools
{
#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(CustomDropdown))]
	public class CustomDropdownDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var dropdown = attribute as CustomDropdown;
			var options = dropdown.Options;

			switch (property.propertyType)
			{
				case SerializedPropertyType.String:
					HandleStringVar(position, property, options);
					break;
				case SerializedPropertyType.Enum:
					HandleEnumVar(position, property, options);
					break;
				case SerializedPropertyType.Integer:
					HandleIntVar(position, property, options);
					break;
				default:
					HandleOtherVars(position, property, label);
					break;
			}
		}

		private void HandleStringVar(Rect position, SerializedProperty property, string[] options)
		{
			var index = Mathf.Max(0, Array.IndexOf(options, property.stringValue));
			index = EditorGUI.Popup(position, property.displayName, index, options);

			property.stringValue = options[index];
		}
		
		private void HandleEnumVar(Rect position, SerializedProperty property, string[] options)
		{
			var index = Mathf.Max(0, property.enumValueIndex);
			index = EditorGUI.Popup(position, property.displayName, index, options);
								
			property.enumValueIndex = index;
		}
		
		private void HandleIntVar(Rect position, SerializedProperty property, string[] options)
		{
			property.intValue = EditorGUI.Popup(position, property.displayName, property.intValue, options);
		}

		private void HandleOtherVars(Rect position, SerializedProperty property, GUIContent label)
		{
			base.OnGUI(position, property, label);
		}
	}
#endif
}