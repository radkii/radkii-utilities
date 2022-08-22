using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System;
using Radkii.Dialogue;
using Action = Radkii.Dialogue.Action;

[CustomPropertyDrawer(typeof(Condition))]
public class ConditionDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return 16f;
	}
	public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
	{
		var target = property.serializedObject.targetObject;

		var nameProperty = property.FindPropertyRelative("par");
		var operatorProperty = property.FindPropertyRelative("op");
		var thresholdProperty = property.FindPropertyRelative("threshold");

		EditorGUIUtility.wideMode = true;
		EditorGUIUtility.labelWidth = 30;
		EditorGUI.indentLevel = 0;
		rect.width /= 3;
		nameProperty.stringValue = EditorGUI.TextField(rect, nameProperty.stringValue);
		rect.x += rect.width;
		//EditorGUILayout.PropertyField(operatorProperty);
		operatorProperty.enumValueIndex = (int)(Operator)EditorGUI.EnumPopup(rect, (Operator)Enum.GetValues(typeof(Operator)).GetValue(operatorProperty.enumValueIndex));
		//operatorProperty.enum = EditorGUI.EnumFlagsField(rect, operatorProperty.enumValueIndex);
		rect.x += rect.width;
		thresholdProperty.stringValue = EditorGUI.TextField(rect, thresholdProperty.stringValue);
	}
}

[CustomPropertyDrawer(typeof(Action))]
public class ActionDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return 16f;
	}
	public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
	{
		var target = property.serializedObject.targetObject;

		var nameProperty = property.FindPropertyRelative("par");
		var fnProperty = property.FindPropertyRelative("fn");
		var valProperty = property.FindPropertyRelative("value");

		EditorGUIUtility.wideMode = true;
		EditorGUIUtility.labelWidth = 30;
		EditorGUI.indentLevel = 0;
		rect.width /= 3;
		nameProperty.stringValue = EditorGUI.TextField(rect, nameProperty.stringValue);
		rect.x += rect.width;
		//EditorGUILayout.PropertyField(operatorProperty);
		fnProperty.enumValueIndex = (int)(Function)EditorGUI.EnumPopup(rect, (Function)Enum.GetValues(typeof(Function)).GetValue(fnProperty.enumValueIndex));
		//operatorProperty.enum = EditorGUI.EnumFlagsField(rect, operatorProperty.enumValueIndex);
		rect.x += rect.width;
		valProperty.intValue = EditorGUI.IntField(rect, valProperty.intValue);
	}
}

[CustomPropertyDrawer(typeof(DialogueField))]
public class DialogueFieldDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		//label.
		
		var typeProperty = property.FindPropertyRelative("fieldType");
		//DialogueFieldType type = (DialogueFieldType)EditorGUI.EnumPopup(rect, (DialogueFieldType)Enum.GetValues(typeof(DialogueFieldType)).GetValue(typeProperty.enumValueIndex));
		DialogueFieldType type = (DialogueFieldType)Enum.GetValues(typeof(DialogueFieldType)).GetValue(typeProperty.enumValueIndex);
		//DialogueFieldType type = (DialogueFieldType)typeProperty.en;
		
		float lines = 2f;
		switch (type)
		{
			case DialogueFieldType.Talk:
				lines = 6f;
				break;
			case DialogueFieldType.CustomMethod:
				lines = 3f;
				break;
		}
		return lines * EditorGUIUtility.singleLineHeight;
	}
	public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
	{
		var target = property.serializedObject.targetObject;

		var typeProperty = property.FindPropertyRelative("fieldType");
		var inputProperty = property.FindPropertyRelative("waitForInput");
		var characterProperty = property.FindPropertyRelative("character");
		var nameProperty = property.FindPropertyRelative("line");
		var methodProperty = property.FindPropertyRelative("customMethodName");
		var paramProperty = property.FindPropertyRelative("customMethodParam");
		//var actionProperty = property.FindPropertyRelative("action");

		EditorGUIUtility.wideMode = true;
		EditorGUIUtility.labelWidth = 30;
		EditorGUI.indentLevel = 0;

		rect.height = EditorGUIUtility.singleLineHeight;
		rect.width /= 3f;
		inputProperty.boolValue = EditorGUI.Toggle(rect, "Wait:", inputProperty.boolValue);
		rect.x += rect.width;
		rect.width *= 2f;

		DialogueFieldType type = (DialogueFieldType)EditorGUI.EnumPopup(rect, (DialogueFieldType)Enum.GetValues(typeof(DialogueFieldType)).GetValue(typeProperty.enumValueIndex));
		typeProperty.enumValueIndex = (int)type;
		rect.x -= rect.width / 2f;
		rect.width *= 1.5f;

		rect.y += EditorGUIUtility.singleLineHeight;
		switch (type)
		{
			case DialogueFieldType.Talk:
				rect.width /= 2f;
				EditorGUI.LabelField(rect, "Character Name: ");
				rect.x += rect.width;
				characterProperty.stringValue = EditorGUI.TextField(rect, characterProperty.stringValue);
				rect.y += EditorGUIUtility.singleLineHeight;
				rect.x -= rect.width;
				rect.width *= 2f;
				rect.height = EditorGUIUtility.singleLineHeight * 4f;
				GUIStyle style = new GUIStyle(EditorStyles.textField);
				style.wordWrap = true;
				style.fixedHeight = EditorGUIUtility.singleLineHeight * 4f;
				nameProperty.stringValue = EditorGUI.TextArea(rect, nameProperty.stringValue, style);
				break;
			case DialogueFieldType.CustomMethod:
				rect.height = EditorGUIUtility.singleLineHeight;
				rect.width /= 2f;
				EditorGUI.LabelField(rect, "Method Name: ");
				rect.x += rect.width;
				methodProperty.stringValue = EditorGUI.TextField(rect, methodProperty.stringValue);
				rect.y += EditorGUIUtility.singleLineHeight;
				rect.x -= rect.width;
				EditorGUI.LabelField(rect, "Parameter: ");
				rect.x += rect.width;
				paramProperty.stringValue = EditorGUI.TextField(rect, paramProperty.stringValue);
				break;
		}
	}
}

[CustomPropertyDrawer(typeof(NodeNameFormat))]
public class NodeNameFormatDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUIUtility.singleLineHeight * 2;
	}
	public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
	{
		Rect GetNewRect(float mult)
		{
			return new Rect(rect.x, rect.y, rect.width * mult, rect.height);
		}

		var target = property.serializedObject.targetObject;

		var methodProperty = property.FindPropertyRelative("nodeName");
		var argProperty = property.FindPropertyRelative("displayName");

		EditorGUIUtility.wideMode = true;
		EditorGUIUtility.labelWidth = 30;
		EditorGUI.indentLevel = 0;
		rect.width /= 6f;
		rect.height /= 2f;

		EditorGUI.LabelField(rect, "Node");
		rect.x += rect.width;

		methodProperty.stringValue = EditorGUI.TextField(GetNewRect(2.4f), methodProperty.stringValue);
		rect.x += rect.width * 3;

		EditorGUI.LabelField(GetNewRect(2f), "displays as:");
		rect.x += rect.width * 2;

		rect.x -= rect.width * 6f;
		rect.y += EditorGUIUtility.singleLineHeight;
		argProperty.stringValue = EditorGUI.TextField(GetNewRect(6f), argProperty.stringValue);
	}
}
