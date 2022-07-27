using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

#if UNITY_EDITOR
public class EditorObjectTransform : EditorWindow
{

	private Vector3 firstPos;
	private Vector3 distancePos;

	private Vector3 firstRot;
	private Vector3 distancePot;

	private Vector3 firstScale = Vector3.one;
	private Vector3 distanceScale;

	private string objectName;


	[MenuItem("KikiEditor/ObjectTools")]
	public static void ShowWindow()
	{
		GetWindow<EditorObjectTransform>("Transformer");
	}

	void OnGUI()
	{
		GUIStyle gUIStyle = new GUIStyle();
		gUIStyle.fontSize = 15;
		gUIStyle.fontStyle = FontStyle.Bold;
		gUIStyle.normal.textColor = Color.cyan;
		gUIStyle.alignment = TextAnchor.UpperLeft;
		gUIStyle.padding = new RectOffset() { bottom = 20, top = 20 };

		GUILayout.Label("Transform the selected objects!", gUIStyle);

		objectName = EditorGUILayout.TextField("Object Name", objectName);

		firstPos = EditorGUILayout.Vector3Field("First Position", firstPos);
		distancePos = EditorGUILayout.Vector3Field("Distance Position", distancePos);

		if (GUILayout.Button("Set Transform!"))
		{
			TransformObject();
		}

		firstRot = EditorGUILayout.Vector3Field("First Rotation", firstRot);
		distancePot = EditorGUILayout.Vector3Field("Distance Rotation", distancePot);

		if (GUILayout.Button("Set Rotatiton!"))
		{
			RotationObject();
		}

		firstScale = EditorGUILayout.Vector3Field("First Scale", firstScale);
		distanceScale = EditorGUILayout.Vector3Field("Distance Scale", distanceScale);

		
		if (GUILayout.Button("Set Scale!"))
		{
			ScaleObject();
		}

		if (GUILayout.Button("Set All!"))
		{
			TransformObject();
			RotationObject();
			ScaleObject();
		}
	}

	private void TransformObject()
	{
		int i = 0;

		foreach (GameObject obj in Selection.gameObjects)
		{
			obj.transform.localPosition = firstPos;
			firstPos += distancePos;
			obj.transform.SetSiblingIndex(i);

			if (objectName != null)
			{

				obj.name = objectName + i;
				i++;
			}
		}

		firstPos = Vector3.zero;

	}

	private void RotationObject()
	{
		int i = 0;
		foreach (GameObject obj in Selection.gameObjects)
		{
			obj.transform.localEulerAngles = firstRot;
			firstRot += distancePot;
			obj.transform.SetSiblingIndex(i);

			if (objectName != null)
			{
				obj.name = objectName + i;
				i++;
			}

		}
		firstRot = Vector3.zero;
	}
	private void ScaleObject()
	{
		int i = 0;
		foreach (GameObject obj in Selection.gameObjects)
		{
			obj.transform.localScale = firstScale;
			firstScale += distanceScale;
			obj.transform.SetSiblingIndex(i);

			if (objectName != null)
			{

				obj.name = objectName + i;
				i++;
			}
		}

		firstScale = Vector3.one;
	}

}
#endif