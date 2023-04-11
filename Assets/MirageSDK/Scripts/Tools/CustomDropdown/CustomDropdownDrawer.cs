using System;
using System.Linq;
using MirageSDK.Data;
using UnityEditor;
using UnityEngine;

namespace MirageSDK.Tools.CustomDropdown
{
#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(CustomDropdown))]
	public class CustomDropdownDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var dropdown = attribute as CustomDropdown;
			var options = dropdown.Options;

			var optionsIndex = FindEnumValueInOptions((Wallet) property.enumValueIndex, options);

			var index = EditorGUI.Popup(position, property.displayName, optionsIndex, options);
			var enumIndex = FindEmunIndexByStringValue(options[index]);
			property.enumValueIndex = enumIndex;
		}

		private int FindEnumValueInOptions(Enum value, string[] options)
		{
			var optionsIndex = options.ToList().IndexOf(value.ToString());
			if (optionsIndex < 0)
			{
				optionsIndex = 0;
			}
			return optionsIndex;
		}

		private int FindEmunIndexByStringValue(string option)
		{
			var value = Enum.Parse(typeof(Wallet), option);
			return (int) value;
		}
	}
#endif
}