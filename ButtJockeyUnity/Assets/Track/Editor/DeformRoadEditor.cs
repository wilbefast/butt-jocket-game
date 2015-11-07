using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DeformRoad))]
class DeformRoadEditor : Editor 
{
	public override void OnInspectorGUI() 
	{
		base.OnInspectorGUI();
		
		if(GUILayout.Button("Reset"))
			((DeformRoad)target).reset();
		
		if(GUILayout.Button("Split"))
			((DeformRoad)target).split();
		
		if(GUILayout.Button("Collapse"))
			((DeformRoad)target).collapse();
	}
}